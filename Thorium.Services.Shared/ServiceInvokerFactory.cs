using System;
using Thorium.Net;
using Thorium.Net.ServiceHost;

namespace Thorium.Services.Shared
{
    public static class ServiceInvokerFactory
    {
        /// <summary>
        /// create service invoker for definition using the specified host
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IServiceInvoker CreateFrom(ServiceInterfaceDefinition sid, string host = "localhost")
        {
            Type compatibleInvoker = null;

            var attributes = sid.Type.GetCustomAttributes(typeof(CompatibleInvokerAttribute), true);
            if(attributes.Length > 0)
            {
                var att = attributes[0] as CompatibleInvokerAttribute;
                compatibleInvoker = att.Type;
            }
            else
            {
                throw new Exception("Cant find a compatible invoker for this service interface");
            }


            Config.Config config = new Config.Config(sid.Configuration);
            config.Values["host"] = host;

            return (IServiceInvoker)Activator.CreateInstance(compatibleInvoker, config);
        }
    }
}
