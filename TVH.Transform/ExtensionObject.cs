using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVH.Transform
{
    public class ExtensionObject
    {
        public string Namespace { get; set; }
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string AssemblyFileName { get; set; }
        public string AssemblyQualifiedName { get; set; }
        public byte[] RawAssembly { get; set; }
    }
}
