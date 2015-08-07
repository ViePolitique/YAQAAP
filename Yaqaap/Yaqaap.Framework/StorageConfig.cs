using Microsoft.Azure;

namespace Yaqaap.Framework
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