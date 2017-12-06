using System.Configuration;

namespace TimeReport.Core.Configuration
{
    public class ToggleConfigurationSection : XmlConfigurationSection
    {
        public readonly static ToggleConfigurationSection Current;

        static ToggleConfigurationSection()
        {
            Current = ConfigurationManager.GetSection("toggleProfile") as ToggleConfigurationSection;
        }

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

        [ConfigurationProperty("login", IsKey = true, IsRequired = true)]
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

        [ConfigurationProperty("password", IsKey = true, IsRequired = true)]
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

        [ConfigurationProperty("workspace", IsKey = true, IsRequired = true)]
        public string Workspace
        {
            get
            {
                return (string)base["workspace"];
            }
            set
            {
                base["workspace"] = value;
            }
        }

        [ConfigurationProperty("url", IsKey = true, IsRequired = true)]
        public string Url
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
        //public class ToggleProfileSettings: ConfigurationElement
        //{
            
        //}

        //[ConfigurationCollection(typeof(ToggleProfileSettings), AddItemName = "toggleProfile")]
        //public class Collection: ConfigurationElementCollection
        //{
        //    protected override ConfigurationElement CreateNewElement() { return new ToggleProfileSettings(); }

        //    protected override object GetElementKey(ConfigurationElement element) { return ((ToggleProfileSettings)element).Profile; }
        //}

        //[ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        //public ToggleProfileSettings Setting
        //{
        //    get { return (ToggleProfileSettings)this[""]; }
        //    set { this[""] = value; }
        //}

        //[ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        //public Collection List
        //{
        //    get { return (Collection)this[""]; }
        //    set { this[""] = value; }
        //}
    }
}