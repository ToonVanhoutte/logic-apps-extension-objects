using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using TRex.Metadata;
using Microsoft.Azure;
using TVH.BlobStorage;
using TVH.ApiApps.Transform.Models;
using TVH.Transform;
using System.Text;
using System.Threading.Tasks;

namespace TVH.ApiApps.Transform.Controllers
{
    public class TransformController : ApiController
    {
        private string _blobStorageConnString = CloudConfigurationManager.GetSetting("ConnectionString.BlobStorage");
        private string _mapsContainerName = CloudConfigurationManager.GetSetting("ContainerName.Maps");
        private string _extensionObjectsContainerName = CloudConfigurationManager.GetSetting("ContainerName.ExtensionObjects");
        private string _assembliesContainerName = CloudConfigurationManager.GetSetting("ContainerName.Assemblies");      


        [HttpPost, Route("api/transform/execute")]
        [Metadata("Transform", "Transforms the input XML using the configured XSLT", VisibilityType.Default)]
        public async Task<HttpResponseMessage> ExecuteTransform([FromBody]TransformRequest transformRequest)
        {
            try
            {
                // Get map and extension object content from blob storage
                var blobStorageClient = new BlobStorageClient(_blobStorageConnString);
                if (transformRequest.MapName == null)
                    throw new ArgumentNullException("There is no map name specified.");

                byte[] mapContent = await blobStorageClient.GetBlob(_mapsContainerName, transformRequest.MapName);
                byte[] extensionObjectContent = transformRequest.ExtensionObjectName == null ? null : await blobStorageClient.GetBlob(_extensionObjectsContainerName, transformRequest.ExtensionObjectName);

                // Get and load extension objects from blob storage
                var transformClient = new TransformClient();
                var extensionObjects = new List<ExtensionObject>();

                if (extensionObjectContent != null)
                {
                    extensionObjects = await transformClient.GetExtensionObjects(Encoding.Default.GetString(extensionObjectContent));
                    foreach (var extensionObject in extensionObjects)
                    {
                        extensionObject.RawAssembly = await blobStorageClient.GetBlob(_assembliesContainerName, extensionObject.AssemblyFileName);
                    }
                }

                // Execute the transform
                var output = await transformClient.ExecuteTransform(
                    transformRequest.InputXml,
                    mapContent,
                    extensionObjects);

                return Request.CreateResponse(HttpStatusCode.OK, new TransformResponse { OutputXml = output });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
