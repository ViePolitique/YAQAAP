using Microsoft.Azure;

namespace Yaqaap.ServiceInterface.TableRepository
{
    public static class StorageConfig
    {
        static StorageConfig()
        {
            ConnexionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            BlobUrl = CloudConfigurationManager.GetSetting("BlobUrl");
        }

        public static string BlobUrl { get; private set; }

        public static string ConnexionString { get; set; }
    }
}