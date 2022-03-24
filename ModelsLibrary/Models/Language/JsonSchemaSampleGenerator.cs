using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models.Language
{
    class JsonSchemaSampleGenerator
    {
        public static JToken Generate(JSchema schema)
        {
            JToken output;
            switch (schema.Type)
            {
                case JSchemaType.Object:
                    var jObject = new JObject();
                    if (schema.Properties != null)
                    {
                        foreach (var prop in schema.Properties)
                        {
                            jObject.Add(TranslateNameToJson(prop.Key), Generate(prop.Value));
                        }
                    }
                    output = jObject;
                    break;
                case JSchemaType.Array:
                    var jArray = new JArray();
                    foreach (var item in schema.Items)
                    {
                        jArray.Add(Generate(item));
                    }
                    output = jArray;
                    break;

                case JSchemaType.String:
                    output = new JValue("sample");
                    break;
                case JSchemaType.Number:
                    output = new JValue(1.0);
                    break;
                case JSchemaType.Integer:
                    output = new JValue(1);
                    break;
                case JSchemaType.Boolean:
                    output = new JValue(false);
                    break;
                case JSchemaType.Null:
                    output = JValue.CreateNull();
                    break;

                default:
                    output = null;
                    break;

            }


            return output;
        }

        public static string TranslateNameToJson(string name)
        {
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }
    }
}