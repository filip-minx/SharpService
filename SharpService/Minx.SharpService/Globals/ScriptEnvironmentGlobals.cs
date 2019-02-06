using System.IO;
using System.Reflection;

namespace Minx.SharpService
{
    public class ScriptEnvironmentGlobals
    {
        public ScriptEnvironment ScriptEnvironment { get; set; }

        public void AddReference(string assemblyPath)
        {
            var fullPath = Path.GetFullPath(assemblyPath);
            var assembly = Assembly.LoadFile(fullPath);

            ScriptEnvironment.AddReference(assembly);
        }
    }
}
