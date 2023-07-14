using System.Web.Profile;

namespace CentralAplicaciones.CodeFile
{

    public class CAProfile
        {

            private ProfileBase _profileBase;

            public CAProfile()
            {
                this._profileBase = new ProfileBase();
            }

            public CAProfile(ProfileBase profileBase)
            {
                this._profileBase = profileBase;
            }

            public virtual int Names
            {
                get
                {
                    return ((int)(this.GetPropertyValue("Names")));
                }
                set
                {
                    this.SetPropertyValue("Names", value);
                }
            }

            public virtual string Country
            {
                get
                {
                    return ((string)(this.GetPropertyValue("Country")));
                }
                set
                {
                    this.SetPropertyValue("Country", value);
                }
            }

            public static CAProfile Current
            {
                get
                {
                    return new CAProfile(System.Web.HttpContext.Current.Profile);
                }
            }

            public virtual ProfileBase ProfileBase
            {
                get
                {
                    return this._profileBase;
                }
            }

            public virtual object this[string propertyName]
            {
                get
                {
                    return this._profileBase[propertyName];
                }
                set
                {
                    this._profileBase[propertyName] = value;
                }
            }

            public virtual string UserName
            {
                get
                {
                    return this._profileBase.UserName;
                }
            }

            public virtual bool IsAnonymous
            {
                get
                {
                    return this._profileBase.IsAnonymous;
                }
            }

            public virtual bool IsDirty
            {
                get
                {
                    return this._profileBase.IsDirty;
                }
            }

            public virtual System.DateTime LastActivityDate
            {
                get
                {
                    return this._profileBase.LastActivityDate;
                }
            }

            public virtual System.DateTime LastUpdatedDate
            {
                get
                {
                    return this._profileBase.LastUpdatedDate;
                }
            }

            public virtual System.Configuration.SettingsProviderCollection Providers
            {
                get
                {
                    return this._profileBase.Providers;
                }
            }

            public virtual System.Configuration.SettingsPropertyValueCollection PropertyValues
            {
                get
                {
                    return this._profileBase.PropertyValues;
                }
            }

            public virtual System.Configuration.SettingsContext Context
            {
                get
                {
                    return this._profileBase.Context;
                }
            }

            public virtual bool IsSynchronized
            {
                get
                {
                    return this._profileBase.IsSynchronized;
                }
            }

            public static System.Configuration.SettingsPropertyCollection Properties
            {
                get
                {
                    return ProfileBase.Properties;
                }
            }

            public static CAProfile GetProfile(string username)
            {
                return new CAProfile(ProfileBase.Create(username));
            }

            public static CAProfile GetProfile(string username, bool authenticated)
            {
                return new CAProfile(ProfileBase.Create(username, authenticated));
            }

            public virtual object GetPropertyValue(string propertyName)
            {
                return this._profileBase.GetPropertyValue(propertyName);
            }

            public virtual void SetPropertyValue(string propertyName, object propertyValue)
            {
                this._profileBase.SetPropertyValue(propertyName, propertyValue);
            }

            public virtual ProfileGroupBase GetProfileGroup(string groupName)
            {
                return this._profileBase.GetProfileGroup(groupName);
            }

            public virtual void Initialize(string username, bool isAuthenticated)
            {
                this._profileBase.Initialize(username, isAuthenticated);
            }

            public virtual void Save()
            {
                this._profileBase.Save();
            }

            public virtual void Initialize(System.Configuration.SettingsContext context, System.Configuration.SettingsPropertyCollection properties, System.Configuration.SettingsProviderCollection providers)
            {
                this._profileBase.Initialize(context, properties, providers);
            }

            public static System.Configuration.SettingsBase Synchronized(System.Configuration.SettingsBase settingsBase)
            {
                return ProfileBase.Synchronized(settingsBase);
            }

            public static ProfileBase Create(string userName)
            {
                return ProfileBase.Create(userName);
            }

            public static ProfileBase Create(string userName, bool isAuthenticated)
            {
                return ProfileBase.Create(userName, isAuthenticated);
            }
        }
}