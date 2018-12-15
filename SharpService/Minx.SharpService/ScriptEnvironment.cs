using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;

namespace Minx.SharpService
{
    public class ScriptEnvironment
    {
        private ScriptState scriptState;

        public ScriptEnvironment(object globals)
        {
            var task = CSharpScript.RunAsync(
                code: "",
                options: ScriptOptions.Default,
                globals: globals,
                globalsType: globals.GetType());

            task.Wait();

            scriptState = task.Result;
        }

        public ScriptEnvironment()
        {
            var task = CSharpScript.RunAsync(
                code: "",
                options: ScriptOptions.Default);

            task.Wait();

            scriptState = task.Result;
        }

        public string Execute(string code)
        {
            try
            {
                var task = scriptState.ContinueWithAsync(code);

                task.Wait();

                scriptState = task.Result;

                return task.Result.ReturnValue?.ToString() ?? "[no result]";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
