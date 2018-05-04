using System;
using Newtonsoft.Json.Linq;
using Thorium.Config;
using Thorium.Net;
using Thorium.Net.ServiceHost;
using Thorium.Services.Shared;

namespace Thorium.Services.Client
{
    public class ThoriumServicesClient
    {
        string host;

        const string ErrorInterfaceNotFound = "Interface Could not be found";

        HttpServiceInvoker invoker;

        public ThoriumServicesClient(string host = "localhost", int port = 8080)
        {
            this.host = host;
            invoker = new HttpServiceInvoker(host, port);
        }

        public ThoriumServicesClient(string configName)
        {
            dynamic c = ConfigFile.GetConfig(configName);

            string host = c.host;
            int port = c.remotePort;

            invoker = new HttpServiceInvoker(host, port);
        }

        public void RegisterService(ServiceDefinition serviceDefinition)
        {
            JObject arg = new JObject
            {
                ["serviceDefinition"] = serviceDefinition.ToJSON()
            };

            invoker.Invoke("registerService", arg);
        }

        public ServiceDefinition GetServiceDefinition(string id)
        {
            JObject arg = new JObject
            {
                ["id"] = id
            };
            var result = invoker.Invoke("getServiceInfo", arg);
            if(result != null)
            {
                ServiceDefinition serviceDefinition = new ServiceDefinition();
                serviceDefinition.FromJSON(result);
                return serviceDefinition;
            }
            return null;
        }

        public T GetServiceInvoker<T>(string serviceId) where T : IServiceInvoker
        {
            return (T)GetServiceInvoker(serviceId, typeof(T));
        }

        public IServiceInvoker GetServiceInvoker(string serviceId, Type preferredInterfaceType = null)
        {
            ServiceDefinition serviceDefinition = GetServiceDefinition(serviceId);

            ServiceInterfaceDefinition sid = null;

            if(serviceDefinition.InterfaceDefinitions.Length == 0)
            {
                throw new Exception(ErrorInterfaceNotFound);
            }

            if(preferredInterfaceType != null)
            {
                foreach(var definition in serviceDefinition.InterfaceDefinitions)
                {
                    if(definition.Type.Equals(preferredInterfaceType))
                    {
                        sid = definition;
                        break;
                    }
                }
            }
            if(sid == null) //if we didnt find one for the interface or none was specified lets use the first one
            {
                sid = serviceDefinition.InterfaceDefinitions[0];
            }
            return ServiceInvokerFactory.CreateFrom(sid, host);
        }
    }
}
