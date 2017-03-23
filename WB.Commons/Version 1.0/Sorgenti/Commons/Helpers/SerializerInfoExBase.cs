// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/01/16, 12:26 PM
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhaghen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;

    using  WB.Commons.Serialization;

    using WB.IIIParty.Commons.Protocol;
    using WB.IIIParty.Commons.Protocol.Serialization;

    /// <summary>
    /// Class SerializerInfo
    /// </summary>
    public class SerializerInfoExBase : IXmlMessageSerializerInfoEx
    {
        #region Fields

        /// <summary>
        /// The registered types
        /// </summary>
        protected static readonly List<Type> registeredTypes = new List<Type>();
        private static readonly List<Type> xmlIncludedTypes = new List<Type>();
        public static Type[] XmlIncludedTypes
        {
            get
            {
                return xmlIncludedTypes.ToArray();
            }
        }

        /// <summary>
        /// The _instance
        /// </summary>
        protected static SerializerInfoExBase _instance;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SerializerInfoExBase"/> class from being created.
        /// </summary>
        protected SerializerInfoExBase()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static SerializerInfoExBase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SerializerInfoExBase();
                return _instance;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typesToInclude"></param>
        public static void XmlInclude(IEnumerable<Type> typesToInclude)
        {
            foreach (Type type in typesToInclude)
            {
                if (!xmlIncludedTypes.Contains(type))
                    xmlIncludedTypes.Add(type);
            }
        }
        
        /// <summary>
        /// Determines whether the specified t is registered.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns><c>true</c> if the specified t is registered; otherwise, <c>false</c>.</returns>
        public static bool IsRegistered(Type t)
        {
            bool res;
            lock (registeredTypes)
            {
                res = registeredTypes.Contains(t);
            }
            return res;
        }

        /// <summary>
        /// Registers all the types derived from a based type searching in a collection of assemblies
        /// </summary>
        /// <param name="baseTypes">The base types.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>System.Int32.</returns>
        public static int RegisterBatch(IEnumerable<Type> baseTypes, IEnumerable<Assembly> assemblies = null)
        {
            int count = 0;

            try
            {
                List<Type> filteredTypes = new List<Type>();

                // se non ho passato gli assembly in cui  ricercare li tira su tutti
                if (assemblies == null)
                    assemblies = AppDomain.CurrentDomain.GetAssemblies().AsEnumerable();

                // estraggo tutti i tipi
                var types = assemblies.SelectMany(ass=>
                                                      {
                                                          var ts = new Type[] {};

                                                          try
                                                          {
                                                              ts = ass.GetTypes();
                                                          }
                                                          catch (System.Reflection.ReflectionTypeLoadException exc)
                                                          {
                                                              System.Diagnostics.Debug.WriteLine(exc.ToString());
                                                          }
                                                          catch (Exception exc)
                                                          {
                                                              System.Diagnostics.Debug.WriteLine(exc.ToString());
                                                          }
                                                          return ts;
                                                      });

                // filtro quelli che matchano il tipo base passato
                // che non siano interfacce o classi astratti e che siano pubblici
                filteredTypes = types.Where(t=>
                                                {
                                                    foreach (var baseType in baseTypes)
                                                    {
                                                        if (baseType.IsAssignableFrom(t)
                                                            //(item.Name != "IMessage" && item.Name.ToLower() != "object")
                                                            && !t.IsInterface
                                                            //&& !(item.IsInstanceOfType(typeof(object)))
                                                            && !t.IsAbstract
                                                            && t.IsPublic
                                                            )
                                                            return true;
                                                    }
                                                    return false;
                                                }).ToList();

                // registro quelli non registrati
                foreach (var imsg in filteredTypes)
                {
                    if (!IsRegistered(imsg))
                    {
                        SerializerInfoExBase.RegisterType(imsg, typeof(IMessage));
                        count++;
                    }
                }
            }
            catch (System.Reflection.ReflectionTypeLoadException exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.ToString());
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.ToString());
            }

            return count;
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="typeToRegister">The type to register.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void RegisterType(Type typeToRegister, Type baseType)
        {
            if (!baseType.IsAssignableFrom(typeToRegister))
                throw new ArgumentException(
                    string.Format("{0} is not a valid type, should be of type IMessage",
                                  typeToRegister));

            if (!IsRegistered(typeToRegister))
            {
                lock (registeredTypes)
                {
                    registeredTypes.Add(typeToRegister);
                }
            }
        }

        /// <summary>
        /// Registers the types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <returns>System.Int32.</returns>
        public static int RegisterTypes(IEnumerable<Type> types, Type baseType)
        {
            return types.Select(s=>
                                    {
                                        RegisterType(s, baseType);
                                        return true;
                                    }).Count();
        }

        /// <summary>
        /// Uns the type of the register.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool UnRegisterType(Type t)
        {
            if (!registeredTypes.Contains(t))
                return false;

            lock (registeredTypes)
            {
                registeredTypes.Remove(t);
            }

            return true;
        }

        /// <summary>
        /// Unregisters the types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>IEnumerable{System.Boolean}.</returns>
        public static IEnumerable<bool> UnregisterTypes(IEnumerable<Type> types)
        {
            return types.Select(s=>
                                    {
                                        UnRegisterType(s);
                                        return true;
                                    });
        }

        /// <summary>
        /// Ritorna il valore del tag xml specificato
        /// </summary>
        /// <param name="xmlReader">The XML reader.</param>
        /// <param name="xmlTag">The XML tag.</param>
        /// <returns>System.String.</returns>
        /// <remarks>Da sovrascrivere nelle classi derivate</remarks>
        public virtual string GetSerializeTypeByXmlTag(XmlReader xmlReader, string xmlTag)
        {
            string serializeType = string.Empty;
            string messageId = string.Empty;
            while (xmlReader.Read())
            {
                // first element is the root element
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == xmlTag)
                    {
                        xmlReader.Read();
                        messageId = xmlReader.Value;
                        break;
                    }
                }
            }
            switch (messageId)
            {
                case "1":
                    serializeType = "ProcessLogonRequest";
                    break;
                case "2":
                    serializeType = "ProcessLogonResponse";
                    break;
                case "3":
                    serializeType = "KeepAliveRequest";
                    break;
                case "4":
                    serializeType = "KeepaliveResponse";
                    break;
                case "5":
                    serializeType = "Ack";
                    break;
                case "6":
                    serializeType = "Nack";
                    break;
                case "7":
                    serializeType = "ConfigPlanNotification";
                    break;
                case "8":
                    serializeType = "NewAudioMessageNotification";
                    break;
                case "9":
                    serializeType = "StartAudioMessage";
                    break;
                case "10":
                    serializeType = "ErrorFromPA";
                    break;
                case "11":
                    serializeType = "EndAudioMessage";
                    break;
            }
            return serializeType;
        }

        /// <summary>
        /// Ritorna il serializer del messaggio corrispondente
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Xml.Serialization.XmlSerializer.</returns>
        public XmlSerializer GetXmlSerializer(string type)
        {
            try
            {
                Type typeFound = null;
                lock (registeredTypes)
                {
                    typeFound =
                        (from entry in registeredTypes where entry.Name.Equals(type) select entry).FirstOrDefault();
                }
                return typeFound != null
                           ? new XmlSerializer(typeFound, XmlIncludedTypes)
                           : null;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion Methods
    }
}