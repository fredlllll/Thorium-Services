using System;
using Newtonsoft.Json.Linq;
using Thorium.Extensions.JSON;

namespace Thorium.Services.Shared
{
    public class ServiceDefinition : IJSONConvertable
    {
        public string Id { get; private set; }
        public ServiceInterfaceDefinition[] InterfaceDefinitions { get; private set; }

        public ServiceDefinition()
        {

        }

        public ServiceDefinition(string id, ServiceInterfaceDefinition[] interfaceDefinitions)
        {
            Id = id;
            InterfaceDefinitions = interfaceDefinitions;
        }

        public void FromJSON(JToken json)
        {
            if(json is JObject jo)
            {
                Id = jo.Get<string>("id");
                JArray interfaceDefinitions = jo["interfaceDefinitions"] as JArray;

                InterfaceDefinitions = new ServiceInterfaceDefinition[interfaceDefinitions.Count];
                for(int i = 0; i < interfaceDefinitions.Count; i++)
                {
                    var id = new ServiceInterfaceDefinition();
                    InterfaceDefinitions[i] = id;
                    id.FromJSON(interfaceDefinitions[i]);
                }
            }
            else
            {
                throw new ArgumentException("i expected a jobject");
            }
        }

        public JToken ToJSON()
        {
            JArray interfaceDefinitions = new JArray();
            foreach(var sid in InterfaceDefinitions)
            {
                interfaceDefinitions.Add(sid.ToJSON());
            }

            JObject jo = new JObject()
            {
                ["id"] = Id,
                ["interfaceDefinitions"] = interfaceDefinitions
            };
            return jo;
        }
    }
}
