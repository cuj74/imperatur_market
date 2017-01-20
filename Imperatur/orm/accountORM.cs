using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperatur.account;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace Imperatur.orm
{
    public class accountORM
    {
        public List<Account> ReadAccounts()
        {
            if (File.Exists(@"C:\Users\urbajoha\Documents\imperatur\accounts.json"))
            {
                List<Account> oListA = new List<Account>();
                using (StreamReader file = File.OpenText(@"C:\Users\urbajoha\Documents\imperatur\accounts.json"))
                {
                    
                    var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
                    settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    try
                    {
                        oListA = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Account>>(file.ReadToEnd(), settings);
                    }
                    catch (Exception ex)
                    {
                        int gg = 0;
                    }
                    return oListA;// Newtonsoft.Json.JsonConvert.DeserializeObject<List<Account>>(file.ReadToEnd(), settings);
                }
                /*
                JArray AccountsFromFile;
                using (StreamReader file = File.OpenText(@"C:\Users\urbajoha\Documents\imperatur\accounts.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    AccountsFromFile = (JArray)JToken.ReadFrom(reader);

                }
                if (AccountsFromFile != null)
                {
                    //var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
                    //settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Account>>(AccountsFromFile,
                        new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() }
                        )


                    // return Newtonsoft.Json.JsonConvert.SerializeObject(o, settings);
                    JsonSerializer oJ = new JsonSerializer();
                    //oJ.Deserialize
                    
                    
                      //  return AccountsFromFile.ToObject<List<Account>>();
                }
                */
            }

            return new List<Account>();
        }

        public bool SaveAllAccounts(List<Account> oAccounts)
        {
            //var SerializeSettings = new JsonSerializerSettings() { ContractResolver = new JsonContractResolver() };
            //var json = JsonConvert.SerializeObject(obj, settings);

            using (FileStream fs = File.Open(@"C:\Users\urbajoha\Documents\imperatur\accounts.json", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(SerializeAllFields.Dump(oAccounts, true));
            }
            /*
            using (JsonTextWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                JsonSerializer serializer = new JsonSerializer();
                serializer.s
                //serializer.Serialize(jw, oAccounts);
            }*/
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
