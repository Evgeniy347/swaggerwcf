using Newtonsoft.Json;
using SwaggerWcf.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SwaggerWcf.Support
{
    internal static class Serializer
    {
        internal static string SerializeJson(this Service service)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                service.Serialize(writer);
            }
            return sb.ToString();
        }
    }
}
