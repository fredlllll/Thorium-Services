using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using Thorium.Net;
using Thorium.Net.ServiceHost;
using Thorium.Services.Host.Storage;
using Thorium.Services.Shared;
using Thorium.Threading;

namespace Thorium.Services.Host
{
    public class ServicesHost : RestartableThreadClass
    {
        const string ErrorExpectedJObject = "This method expects a JObject as argument";
        const string ErrorNotFound = "No service was found matching your criteria";

        ServiceHost serviceHost;

        IServiceDefinitionStorage serviceDefinitionStorage;

        public ServicesHost(string configName = "serviceshost") : base(false)
        {
            serviceHost = new ServiceHost();

            Routine putServiceDefinition = new Routine(nameof(putServiceDefinition), PutServiceDefinition);
            Routine getServiceDefinition = new Routine(nameof(getServiceDefinition), GetServiceDefinition);

            serviceHost.RegisterRoutine(putServiceDefinition);
            serviceHost.RegisterRoutine(getServiceDefinition);

            HttpServiceInvokationReceiver invokationReceiver = new HttpServiceInvokationReceiver(configName);

            serviceHost.RegisterInvokationReceiver(invokationReceiver);

            //make configurable which storage to use (memory is probably fine for all eternity though)
            serviceDefinitionStorage = new MemoryServiceDefinitionStorage();
        }

        JToken PutServiceDefinition(JToken arg)
        {
            if(arg is JObject jo)
            {
                JObject serviceDefinitionData = (JObject)arg["serviceDefinition"];

                ServiceDefinition serviceDefinition = new ServiceDefinition();
                serviceDefinition.FromJSON(serviceDefinitionData);

                serviceDefinitionStorage.Store(serviceDefinition);

                return true;
            }
            else
            {
                throw new ArgumentException(ErrorExpectedJObject);
            }
        }

        JToken GetServiceDefinition(JToken arg)
        {
            if(arg is JObject jo)
            {
                string id = jo.Get<string>("id");

                ServiceDefinition definition = serviceDefinitionStorage.Load(id);
                if(definition != null)
                {
                    return definition.ToJSON();
                }
                throw new KeyNotFoundException(ErrorNotFound);
            }
            else
            {
                throw new ArgumentException(ErrorExpectedJObject);
            }
        }

        public override void Start()
        {
            base.Start();
            serviceHost.Start();
        }

        public override void Stop()
        {
            base.Stop();
            serviceHost.Stop();
        }

        protected override void Run()
        {
            try
            {
                Thread.Sleep(-1);//sleep forever, but prevent the application from shutting down if there are no other threads keeping it alive
            }
            catch(ThreadInterruptedException)
            {

            }
        }
    }
}
