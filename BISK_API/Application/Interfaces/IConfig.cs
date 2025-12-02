using GardeningAPI.Model;

namespace GardeningAPI.Application.Interfaces
{
    public interface IConfig
    {
        Task<ConfigurationData?> GetConfiguration();
    }
}
