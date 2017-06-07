using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TRex.Metadata;

namespace TVH.ApiApps.Transform.Models
{
    public class TransformResponse
    {
        [Metadata("Output XML", "Returns the transformed output XML", VisibilityType.Default)]
        public byte[] OutputXml { get; set; }
    }
}