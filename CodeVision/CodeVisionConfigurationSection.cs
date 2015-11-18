using System.Configuration;
using System.Runtime.Remoting.Channels;

namespace CodeVision
{
    public class CodeVisionConfigurationSection : ConfigurationSection, IConfiguration
    {
        const string IndexPathProperty = "IndexPath";
        const string ContentRootProperty = "ContentRoot";
        const string DependencyGraphConnectionStringProperty = "DependencyGraphConnectionString";
        const string TargetDatabaseConnectionStringProperty = "TargetDatabaseConnectionString";
        
        [ConfigurationProperty(IndexPathProperty, IsRequired = true)]
        public string IndexPath => (string)this[IndexPathProperty];

        [ConfigurationProperty(ContentRootProperty, IsRequired = true)]
        public string ContentRootPath => (string)this[ContentRootProperty];

        [ConfigurationProperty(DependencyGraphConnectionStringProperty, IsRequired = false)]
        public string DependencyGraphConnectionString => (string)this[DependencyGraphConnectionStringProperty];

        [ConfigurationProperty(TargetDatabaseConnectionStringProperty, IsRequired = false)]
        public string TargetDatabaseConnectionString => (string)this[TargetDatabaseConnectionStringProperty];

        public static CodeVisionConfigurationSection Load()
        {
            return ConfigurationManager.GetSection("CodeVision") as CodeVisionConfigurationSection;
        }
    }
}
