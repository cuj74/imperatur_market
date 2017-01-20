using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Imperatur_v2.shared;

namespace Imperatur_v2.json
{
    public static class SerializeJSONdata
    {
        public static bool SerializeObject(object ObjectToSerialize, string FileName)
        {
            try
            {
                using (FileStream fs = File.Open(FileName, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(SerializeAllFields.Dump(ObjectToSerialize, true));
                }
            }
            catch(Exception ex)
            {
                ImperaturGlobal.GetLog().Error(string.Format("Could not SerializeObject {0}", FileName), ex);
            }
            return true;
        }

    }

    public static class SerializeAllFields
    {
        public static string Dump(object o, bool indented = true)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver(), TypeNameHandling = TypeNameHandling.All }; //, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
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
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f=>!f.Name.Equals("SaveAccountEvent"))
            .Select(f => base.CreateProperty(f, memberSerialization))
            .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });

            return props;
        }
    }
}
