using System.Configuration;

namespace TimeReport.Core.Configuration
{
    public class JiraProfilesConfigurationSection: XmlConfigurationSection
    {
        public readonly static JiraProfilesConfigurationSection Current;

        static JiraProfilesConfigurationSection()
        {
            Current = ConfigurationManager.GetSection("jiraProfiles") as JiraProfilesConfigurationSection;
        }

        public class JiraProfileSettings: ConfigurationElement
        {
            [ConfigurationProperty("profile", IsKey = true, IsRequired = true)]
            public string Profile
            {
                get
                {
                    return (string)base["profile"];
                }
                set
                {
                    base["profile"] = value;
                }
            }

            [ConfigurationProperty("login", IsRequired = true)]
            public string Login
            {
                get
                {
                    return (string)base["login"];
                }
                set
                {
                    base["login"] = value;
                }
            }

            [ConfigurationProperty("password", IsRequired = true)]
            public string Password
            {
                get
                {
                    return (string)base["password"];
                }
                set
                {
                    base["password"] = value;
                }
            }

            [ConfigurationProperty("name", IsRequired = true)]
            public string Name
            {
                get
                {
                    return (string)base["name"];
                }
                set
                {
                    base["name"] = value;
                }
            }

            [ConfigurationProperty("keyPrefixes", IsRequired = false)]
            public string KeyPrefix
            {
                get
                {
                    return (string)base["keyPrefixes"];
                }
                set
                {
                    base["keyPrefixes"] = value;
                }
            }

            [ConfigurationProperty("url", IsRequired = true)]
            public string FormattedUrl
            {
                get
                {
                    return (string)base["url"];
                }
                set
                {
                    base["url"] = value;
                }
            }
        }

        [ConfigurationCollection(typeof(JiraProfileSettings), AddItemName = "jiraProfile")]
        public class Collection: ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement() { return new JiraProfileSettings(); }

            protected override object GetElementKey(ConfigurationElement element) { return ((JiraProfileSettings)element).Profile; }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public Collection List
        {
            get { return (Collection)this[""]; }
            set { this[""] = value; }
        }
    }
}
