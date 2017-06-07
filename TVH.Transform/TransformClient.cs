using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace TVH.Transform
{
    public class TransformClient
    {
        public async Task<byte[]> ExecuteTransform(byte[] inputXml, byte[] transformXslt, List<ExtensionObject> extensionObjects)
        {
            return await TransformXslt(
                XmlReader.Create(new MemoryStream(inputXml)),
                XmlReader.Create(new MemoryStream(transformXslt)),
                extensionObjects);
        }

        private async Task<byte[]> TransformXslt(XmlReader inputDocument, XmlReader stylesheet, List<ExtensionObject> extensionObjects)
        {
            XslCompiledTransform transform = new XslCompiledTransform(true);
            transform.Load(stylesheet, new XsltSettings(true, true), null);

            XsltArgumentList arguments = new XsltArgumentList();

            // Add an argument for each defined extension object
            foreach (var extensionObject in extensionObjects)
            {
                var assembly = Assembly.Load(extensionObject.RawAssembly);
                var type = assembly.GetType(extensionObject.ClassName);                    

                if (type == null)
                    throw new Exception(String.Format("Cannot get type {0}", extensionObject.AssemblyName));

                arguments.AddExtensionObject(extensionObject.Namespace, Activator.CreateInstance(type));
            }

            var outputStream = new MemoryStream();

            // Execute the transformation, whilst passing the arguments
            using (XmlWriter writer = XmlWriter.Create(outputStream, transform.OutputSettings))
                transform.Transform(inputDocument, arguments, writer);

            return outputStream.ToArray();
        }

        public async Task<List<ExtensionObject>> GetExtensionObjects(string extensionObjectXml)
        {
            if (String.IsNullOrEmpty(extensionObjectXml))
                return null;

            var extensionObjects = new List<ExtensionObject>();

            // Load the extension object into XML
            XmlDocument document = new XmlDocument();
            document.LoadXml(extensionObjectXml);

            ArrayList extensions = new ArrayList();

            // Extract the required info for each defined extension object
            foreach (XmlNode node in document.SelectNodes("/ExtensionObjects/ExtensionObject"))
            {
                string extension_namespace = node.Attributes["Namespace"].Value;
                string extension_assembly = node.Attributes["AssemblyName"].Value;
                string extension_class = node.Attributes["ClassName"].Value;
                string assembly_qualified_name = String.Format("{0}, {1}"
                  , extension_class
                  , extension_assembly
                  );

                extensionObjects.Add(new ExtensionObject
                {
                    AssemblyName = extension_assembly,
                    AssemblyQualifiedName = assembly_qualified_name,
                    AssemblyFileName = String.Format("{0}.dll", extension_assembly.Split(',')[0]),
                    Namespace = extension_namespace,
                    ClassName = extension_class
                });
            }

            return extensionObjects;
        }
    }
}
