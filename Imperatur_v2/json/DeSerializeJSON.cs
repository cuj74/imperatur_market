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
    public static class DeserializeJSON
    {
        public static object DeserializeObject(string Object)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            object ReturnObject = null;
            try
            {
                ReturnObject = Newtonsoft.Json.JsonConvert.DeserializeObject(Object, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Object couldn't be deserialized");
            }
            return ReturnObject;
        }
        public static object DeserializeObjectFromFile(string FileName)
        {
            if (File.Exists(@FileName))
            {
                object ReturnObject = null;
                using (StreamReader file = File.OpenText(@FileName))
                {

                    var settings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new AllFieldsContractResolver() };
                    settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    try
                    {
                        ReturnObject = Newtonsoft.Json.JsonConvert.DeserializeObject(file.ReadToEnd(), settings);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("File {0} couldn't be deserialized", FileName));
                    }
                    return ReturnObject;

                }
            }
            throw new Exception(string.Format("File {0} doesn't exists", FileName));
        }
    }
}
