using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class ParameterSchema : ParameterBase
    {
        public string SchemaRef { get; set; }

        public override void Serialize(JsonWriter writer)
        {
            Serialize(writer, false);
        }
        public override void Serialize(JsonWriter writer, bool skipStartEndObject)
        {
            if (!skipStartEndObject)
                writer.WriteStartObject();

            if (!Extensions.IsNullOrWhiteSpace(Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(Name);
            }
            if (In != InType.Unknown)
            {
                writer.WritePropertyName("in");
                writer.WriteValue(In.ToString().ToLower());
            }
            if (!Extensions.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }
            if (Required)
            {
                writer.WritePropertyName("required");
                writer.WriteValue(Required);
            }
            if (!Extensions.IsNullOrWhiteSpace(SchemaRef))
            {
                writer.WritePropertyName("schema");
                writer.WriteStartObject();

                writer.WritePropertyName("$ref");
                writer.WriteValue(string.Format("#/definitions/{0}", SchemaRef));

                writer.WriteEndObject();
            }

            if (!skipStartEndObject)
                writer.WriteEndObject();
        }
    }
}
