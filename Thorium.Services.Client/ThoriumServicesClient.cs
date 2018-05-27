using System;
using Newtonsoft.Json.Linq;
using Thorium.Config;
using Thorium.Net;
using Thorium.Net.ServiceHost;
using Thorium.Net.ServiceHost.Invokers;
using Thorium.Services.Shared;

namespace Thorium.Services.Client
{
    public class ThoriumServicesClient
    {
        string host;

        const string ErrorInterfaceNotFound = "Interface could not be found";
        const string ErrorServiceDefinitionNotFound = "Service Definition could not be found";

        HttpServiceInvoker invoker;

        public ThoriumServicesClient(string host = "localhost", int port = 8080)
        {
            this.host = host;
            invoker = new HttpServiceInvoker(host, port);
        }

        ThoriumServicesClient(Config.Config config)
        {
            dynamic c = config;

            string host = c.host;
            int port = c.remotePort;

            invoker = new HttpServiceInvoker(host, port);
        }

        public ThoriumServicesClient(string configName) : this(ConfigFile.GetConfig(configName)) { }

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

        public T GetServiceInvoker<T>(string serviceId) where T : IInvoker
        {
            return (T)GetServiceInvoker(serviceId, typeof(T));
        }

        ServiceInterfaceDefinition GetServiceInterfaceDefinition(ServiceDefinition serviceDefinition, Type preferredInterfaceType = null)
        {
            if(preferredInterfaceType != null)
            {
                foreach(var definition in serviceDefinition.InterfaceDefinitions)
                {
                    if(definition.Type.Equals(preferredInterfaceType))
                    {
                        return definition;
                    }
                }
            }

            return serviceDefinition.InterfaceDefinitions[0];
        }

        public IInvoker GetServiceInvoker(string serviceId, Type preferredInterfaceType = null)
        {
            ServiceDefinition serviceDefinition = GetServiceDefinition(serviceId);

            if(serviceDefinition == null)
            {
                throw new Exception(ErrorServiceDefinitionNotFound);
            }

            if(serviceDefinition.InterfaceDefinitions.Length == 0)
            {
                throw new Exception(ErrorInterfaceNotFound);
            }

            ServiceInterfaceDefinition interfaceDefinition = GetServiceInterfaceDefinition(serviceDefinition, preferredInterfaceType);

            return ServiceInvokerFactory.CreateFrom(interfaceDefinition, host);
        }
    }
}
