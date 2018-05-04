using System;
using Thorium.Net;
using Thorium.Net.ServiceHost;

namespace Thorium.Services.Shared
{
    public static class ServiceInterfaceDefinitionFactory
    {
        public static ServiceInterfaceDefinition CreateFrom(IServiceInvokationReceiver sir)
        {
            return new ServiceInterfaceDefinition(sir.GetType(), sir.Configuration.Values);
        }
    }
}
