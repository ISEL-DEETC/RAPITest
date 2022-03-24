using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.IO;

namespace ModelsLibrary.Models.Language
{
	class XMLLanguage : ALanguage
	{

		public override string GenerateWithSchema(string schema)
		{
			JSchema jschema = JSchema.Parse(schema);
			string jsonObj = JsonSchemaSampleGenerator.Generate(jschema).ToString();
			return JsonConvert.DeserializeXmlNode(jsonObj,"root").OuterXml;
		}

		public override string GetValue(string path, string obj)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(obj);

			XmlNode node = xmlDoc.SelectSingleNode(path);
			if (node != null)
			{
				return node.InnerText;
			}
			return null;
		}

		public override bool ValidateSchema(string schema, string obj)
		{
			string description = null;
			string xsdContent = schema;
			string xmlContent = obj;
			try
			{
				XmlSchemaSet xmlschema;
				XDocument xmlDoc;
				using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xsdContent)))
				{
					var xsc = XmlSchema.Read(ms, (o, e) =>
					{
						description = "fail";
					});
					xmlschema = new XmlSchemaSet();
					xmlschema.Add(xsc);
					xmlDoc = XDocument.Parse(xmlContent, LoadOptions.SetLineInfo);
				}

				xmlDoc.Validate(xmlschema, (o, e) =>
				{
					description = "fail";
				});
			}
			catch (Exception) { }

			if (description == null) return true;
			return false;
		}
	}
}
