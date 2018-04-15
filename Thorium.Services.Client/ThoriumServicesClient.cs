using System;
using Newtonsoft.Json.Linq;
using Thorium.Net;
using Thorium.Services.Shared;

namespace Thorium.Services.Client
{
    public class ThoriumServicesClient
    {
        const string ErrorInterfaceNotFound = "Could not find an interface matching your criteria";

        HttpServiceInvoker invoker;

        public ThoriumServicesClient(string host = "localhost")
        {
            invoker = new HttpServiceInvoker(host, 8080); //TODO: make configurable
        }

        public void RegisterService(ServiceDefinition serviceDefinition, ServiceTenancy tenancy = ServiceTenancy.Local)
        {
            JObject arg = new JObject
            {
                ["serviceTenancy"] = tenancy.ToString(),
                ["serviceDefinition"] = serviceDefinition.ToJSON()
            };

            invoker.Invoke("registerService", arg);
        }

        public ServiceDefinition GetServiceInfo(string id, ServiceTenancy tenancy = ServiceTenancy.Both)
        {
            JObject arg = new JObject
            {
                ["serviceTenancy"] = tenancy.ToString(),
                ["id"] = id
            };
            ServiceDefinition definition = new ServiceDefinition();
            definition.FromJSON(invoker.Invoke("getServiceInfo", arg));
            return definition;
        }

        public IServiceInvoker GetServiceInvoker(string serviceId, ServiceTenancy tenancy = ServiceTenancy.Both, Type wantedInterfaceType = null)
        {
            ServiceDefinition def = GetServiceInfo(serviceId, tenancy);

            ServiceInterfaceDefinition sid = null;

            if(wantedInterfaceType == null)
            {
                sid = def.InterfaceDefinitions[0];
            }
            else
            {
                foreach(var id in def.InterfaceDefinitions)
                {
                    if(id.Equals(wantedInterfaceType.Name))
                    {
                        sid = id;
                        break;
                    }
                }
            }
            if(sid == null)
            {
                throw new Exception(ErrorInterfaceNotFound);
            }
            return ServiceInvokerFactory.CreateFrom(def.InterfaceDefinitions[0]);
        }
    }
}
