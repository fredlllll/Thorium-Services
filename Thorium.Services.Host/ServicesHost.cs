using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Thorium.Net;
using Thorium.Services.Host.Storage;
using Thorium.Services.Shared;

namespace Thorium.Services.Host
{
    public class ServicesHost
    {
        const string ErrorExpectedJObject = "This method expects a JObject as argument";
        const string ErrorNotFound = "No service was found matching your criteria";

        ServiceHost serviceHost;

        IServiceDefinitionStorage local, remote;

        public ServicesHost()
        {
            serviceHost = new ServiceHost();

            Routine registerService = new Routine(nameof(registerService), RegisterService);
            Routine getServiceInfo = new Routine(nameof(getServiceInfo), GetServiceInfo);

            serviceHost.RegisterRoutine(registerService);
            serviceHost.RegisterRoutine(getServiceInfo);

            HttpServiceInvokationReceiver invokationReceiver = new HttpServiceInvokationReceiver("serviceshost");

            serviceHost.RegisterInvokationReceiver(invokationReceiver);

            //TODO: make configurable in future if necessary
            local = new MemoryServiceDefinitionStorage();
            remote = new MemoryServiceDefinitionStorage();
        }

        JToken RegisterService(JToken arg)
        {
            if(arg is JObject jo)
            {
                ServiceTenancy tenancy = (ServiceTenancy)Enum.Parse(typeof(ServiceTenancy), jo.Get<string>("serviceTenancy"));

                JObject serviceDefinitionData = (JObject)arg["serviceDefinition"];

                ServiceDefinition serviceDefinition = new ServiceDefinition();
                serviceDefinition.FromJSON(serviceDefinitionData);

                if(tenancy.HasFlag(ServiceTenancy.Local))
                {
                    RegisterLocal(serviceDefinition);
                }
                if(tenancy.HasFlag(ServiceTenancy.Remote))
                {
                    RegisterRemote(serviceDefinition);
                }

                return true;
            }
            else
            {
                throw new ArgumentException(ErrorExpectedJObject);
            }
        }

        JToken GetServiceInfo(JToken arg)
        {
            if(arg is JObject jo)
            {
                ServiceTenancy tenancy = (ServiceTenancy)Enum.Parse(typeof(ServiceTenancy), jo.Get<string>("serviceTenancy"));
                string id = jo.Get<string>("id");

                ServiceDefinition definition = null;
                if(tenancy.HasFlag(ServiceTenancy.Local))
                {
                    if((definition = local.Load(id)) != null)
                    {
                        return definition.ToJSON();
                    }
                }
                if(tenancy.HasFlag(ServiceTenancy.Remote))
                {
                    if((definition = remote.Load(id)) != null)
                    {
                        return definition.ToJSON();
                    }
                }
                throw new KeyNotFoundException(ErrorNotFound);
            }
            else
            {
                throw new ArgumentException(ErrorExpectedJObject);
            }
        }

        void RegisterLocal(ServiceDefinition definition)
        {
            local.Store(definition);
        }

        void RegisterRemote(ServiceDefinition definition)
        {
            remote.Store(definition);
        }

        public void Start()
        {
            serviceHost.Start();
        }

        public void Stop()
        {
            serviceHost.Stop();
        }
    }
}
