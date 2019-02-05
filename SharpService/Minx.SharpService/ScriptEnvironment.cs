using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Minx.SharpService
{
    public class ScriptEnvironment
    {
        private ScriptState scriptState;

        private List<ScriptExecution> executions = new List<ScriptExecution>();

        public IReadOnlyList<ScriptExecution> Executions => executions;

        public ScriptEnvironment(object globals)
        {
            var task = CSharpScript.RunAsync(
                code: "",
                options: GetOptions(),
                globals: globals,
                globalsType: globals.GetType());

            task.Wait();

            scriptState = task.Result;
        }

        public ScriptEnvironment()
        {
            var task = CSharpScript.RunAsync(
                code: "",
                options: GetOptions());

            task.Wait();

            scriptState = task.Result;
        }

        public ScriptExecution Execute(string code)
        {
            var execution = CreateExecution(code);

            try
            {
                using (var interceptor = ConsoleInterceptor.Get())
                {
                    var task = scriptState.ContinueWithAsync(code);

                    task.Wait();

                    scriptState = task.Result;

                    execution.Result = task.Result.ReturnValue?.ToString() ?? "[no result]";
                    execution.ConsoleOutput = interceptor.OutputText;
                }
            }
            catch (Exception e)
            {
                execution.Result = e.Message;
                execution.Error = true;
            }

            return execution;
        }

        private ScriptExecution CreateExecution(string code)
        {
            var execution = new ScriptExecution()
            {
                Code = code,
                Id = executions.Count
            };

            executions.Add(execution);

            return execution;
        }

        private ScriptOptions GetOptions()
        {
            return ScriptOptions.Default
                .WithReferences(typeof(Process).Assembly);
        }
    }
}
