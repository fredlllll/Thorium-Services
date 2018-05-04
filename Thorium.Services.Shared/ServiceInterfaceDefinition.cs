using System;
using Newtonsoft.Json.Linq;
using Thorium.Extensions.JSON;
using Thorium.Reflection;

namespace Thorium.Services.Shared
{
    public class ServiceInterfaceDefinition : IJSONConvertable
    {
        public Type Type { get; private set; }
        public JObject Configuration { get; private set; }

        public ServiceInterfaceDefinition() { }

        public ServiceInterfaceDefinition(Type type, JObject configuration)
        {
            Type = type;
            Configuration = configuration;
        }

        public void FromJSON(JToken json)
        {
            if(json is JObject jo)
            {
                Type = ReflectionHelper.GetType(jo.Get<string>("type"));
                Configuration = (JObject)jo["configuration"];
            }
            else
            {
                throw new ArgumentException("im expecting a jobject");
            }
        }

        public JToken ToJSON()
        {
            JObject jo = new JObject
            {
                ["type"] = Type.AssemblyQualifiedName,
                ["configuration"] = Configuration
            };

            return jo;
        }
    }
}
