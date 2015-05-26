using System.Configuration;
using System.Runtime.Remoting.Channels;

namespace CodeVision
{
    public class CodeVisionConfigurationSection : ConfigurationSection, IConfiguration
    {
        const string IndexPathProperty = "IndexPath";
        const string ContentRootProperty = "ContentRoot";
        
        [ConfigurationProperty(IndexPathProperty, IsRequired = true)]
        public string IndexPath
        {
            get
            {
                return (string)this[IndexPathProperty];
            }
        }

        [ConfigurationProperty(ContentRootProperty, IsRequired = true)]
        public string ContentRootPath
        {
            get
            {
                return (string)this[ContentRootProperty];
            }
        }

        public static CodeVisionConfigurationSection Load()
        {
            return ConfigurationManager.GetSection("CodeVision") as CodeVisionConfigurationSection;
        }
    }
}
