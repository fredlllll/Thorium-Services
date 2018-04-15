using System.Collections.Generic;
using Thorium.Services.Shared;

namespace Thorium.Services.Host.Storage
{
    public class MemoryServiceDefinitionStorage : IServiceDefinitionStorage
    {
        Dictionary<string, ServiceDefinition> definitions = new Dictionary<string, ServiceDefinition>();

        public void Store(ServiceDefinition definition)
        {
            definitions[definition.Id] = definition;
        }

        /// <summary>
        /// returns null if not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceDefinition Load(string id)
        {
            if(definitions.TryGetValue(id, out ServiceDefinition retval))
            {
                return retval;
            }
            return null;
        }
    }
}
