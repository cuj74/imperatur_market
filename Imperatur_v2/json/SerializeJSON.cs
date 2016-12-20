using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.Reflection;
using System.IO;

namespace Imperatur_v2.json
{
    public static class SerializeJSONdata
    {
        public static bool SerializeObject(object ObjectToSerialize, string FileName)
        {
            using (FileStream fs = File.Open(FileName, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(SerializeAllFields.Dump(ObjectToSerialize, true));
            }
            return true;
        }

    }

    public static class SerializeAllFields
    {
        public static string Dump(object o, bool indented = true)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
            if (indented)
            {
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, settings);
        }
    }

    public class AllFieldsContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var props = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(p => base.CreateProperty(p, memberSerialization))
                .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(f => base.CreateProperty(f, memberSerialization)))
                .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });
            return props;
        }
    }
}
