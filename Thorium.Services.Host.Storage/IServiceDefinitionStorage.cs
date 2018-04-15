using Thorium.Services.Shared;

namespace Thorium.Services.Host.Storage
{
    public interface IServiceDefinitionStorage
    {
        void Store(ServiceDefinition definition);

        ServiceDefinition Load(string id);
    }
}
