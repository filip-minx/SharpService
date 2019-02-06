using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Minx.SharpService
{
    public class ScriptEnvironment
    {
        private ScriptState scriptState;

        private List<ScriptExecution> executions = new List<ScriptExecution>();

        public IReadOnlyList<ScriptExecution> Executions => executions;

        public ScriptOptions ScriptOptions { get; set; } = ScriptOptions.Default
            .WithReferences(typeof(Process).Assembly);

        public ScriptEnvironment(object globals)
        {
            var task = CSharpScript.RunAsync(
                code: "",
                options: ScriptOptions,
                globals: globals);

            task.Wait();

            scriptState = task.Result;
        }

        public ScriptEnvironment()
        {
            var task = CSharpScript.RunAsync(
                code: "",
                options: ScriptOptions);

            task.Wait();

            scriptState = task.Result;
        }

        public void AddReference(Assembly assembly)
        {
            ScriptOptions = ScriptOptions.WithReferences(assembly);
        }

        public ScriptExecution Execute(string code)
        {
            var execution = CreateExecution(code);

            try
            {
                using (var interceptor = ConsoleInterceptor.Get())
                {
                    var task = scriptState.ContinueWithAsync(
                        code: code,
                        options: ScriptOptions);

                    task.Wait();

                    scriptState = task.Result;

                    execution.Result = task.Result.ReturnValue?.ToString() ?? "[no result]";
                    execution.ConsoleOutput = interceptor.OutputText;
                }
            }
            catch (Exception e)
            {
                if (e is AggregateException aggregateException)
                {
                    var messageBuilder = new StringBuilder();

                    messageBuilder.AppendLine($"AggregateException: {aggregateException.Message}");

                    aggregateException.Handle((inner) =>
                    {
                        messageBuilder.AppendLine($" - {inner.GetType().Name}: {inner.Message}");
                        return true;
                    });

                    execution.Result = messageBuilder.ToString();
                }
                else
                {
                    execution.Result = e.Message;
                }

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
    }
}
