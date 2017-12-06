using System.Configuration;

namespace TimeReport.Core.Configuration
{
    public class XmlConfigurationSection: ConfigurationSection
    {
        private const string XmlXsiNamespaceAttributeName = "xmlns:xsi";

        private const string XmlXsiNoNamespaceSchemaLocationAttributeName = "xsi:noNamespaceSchemaLocation";

        [ConfigurationProperty(XmlXsiNamespaceAttributeName, IsRequired = false)]
        public string XmlXsiNamespace
        {
            get
            {
                return (string)this[XmlXsiNamespaceAttributeName];
            }
            set
            {
                this[XmlXsiNamespaceAttributeName] = value;
            }
        }

        [ConfigurationProperty(XmlXsiNoNamespaceSchemaLocationAttributeName, IsRequired = false)]
        public string XmlXsiNoNamespaceSchemaLocation
        {
            get
            {
                return (string)this[XmlXsiNoNamespaceSchemaLocationAttributeName];
            }
            set
            {
                this[XmlXsiNoNamespaceSchemaLocationAttributeName] = value;
            }
        }
    }
}