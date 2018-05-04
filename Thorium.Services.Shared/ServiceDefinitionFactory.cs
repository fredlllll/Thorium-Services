using System;
using System.Collections.Generic;
using Thorium.Net;
using Thorium.Net.ServiceHost;

namespace Thorium.Services.Shared
{
    public static class ServiceDefinitionFactory
    {
        public static ServiceDefinition CreateFrom(ServiceHost host, string id)
        {
            List<ServiceInterfaceDefinition> sids = new List<ServiceInterfaceDefinition>();

            var ir = host.InvokationReceivers;

            for(int i = 0; i < ir.Count; i++)
            {
                var r = ir[i];
                sids.Add(ServiceInterfaceDefinitionFactory.CreateFrom(r));
            }

            return new ServiceDefinition(id, sids.ToArray());
        }
    }
}
