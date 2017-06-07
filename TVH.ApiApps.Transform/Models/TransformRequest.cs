using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TRex.Metadata;

namespace TVH.ApiApps.Transform.Models
{
    public class TransformRequest
    {
        [Metadata("Input XML", "Provide the content of the XML that must be transformed", VisibilityType.Default)]
        public byte[] InputXml { get; set; }

        [Metadata("Map Name", "Provide the blob name of the XSLT map to be executed", VisibilityType.Default)]
        public string MapName { get; set; }

        [Metadata("Extension Object Name", "Provide the blob name of the extension object to be used", VisibilityType.Advanced)]
        public string ExtensionObjectName { get; set; }


    }
}