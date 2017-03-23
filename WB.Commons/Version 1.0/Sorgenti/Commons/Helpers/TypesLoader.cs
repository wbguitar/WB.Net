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

    /// <summary>
    /// Helper static class that uses reflection to load all instances of a given type
    /// scanning the referenced assemblies, and stores them in a dictionary
    /// </summary>
    public static class TypesLoader
    {
        #region Fields

        /// <summary>
        /// The types
        /// </summary>
        private static readonly Dictionary<Type, List<Type>> Types = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// The non sys assemblies
        /// </summary>
        private static IEnumerable<Assembly> nonSysAssemblies;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Retrieves a collection of non system assemblies referenced by the current app domain
        /// </summary>
        /// <param name="rescan">if set to <c>true</c> [rescan].</param>
        /// <returns>IEnumerable{Assembly}.</returns>
        public static IEnumerable<Assembly> GetNonSysAssemblies(bool rescan = false)
        {
            if (rescan)
                nonSysAssemblies = null;

            if (nonSysAssemblies != null)
                return nonSysAssemblies;

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var referencedAssemblies = loadedAssemblies.SelectMany(ass => ass.GetReferencedAssemblies()).Distinct().ToArray();
            //var paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            nonSysAssemblies = referencedAssemblies.Select(
                    ProxyDomain.LoadAssembly)
                                .Where(ass => ass != null && !String.IsNullOrEmpty(ass.Location))
                                .Where(ass =>
                                       ass != null
                                           //&& !ass.Location.Contains(Environment.GetFolderPath(Environment.SpecialFolder.Windows)) // funziona solo con il framework >= 4.0
                                       && !ass.Location.Contains(Environment.GetEnvironmentVariable("SYSTEMROOT"))
                                       && !ass.Location.Contains("GAC_MSIL")
                                       && !ass.Location.Contains("GAC_32")
                                       && ass.ManifestModule.Name != "<In Memory Module>"
                                       && !ass.FullName.Contains("System")
                                       && !ass.FullName.Contains("Microsoft")
                                       && !ass.FullName.Contains("mscorlib")
                                       && !ass.Location.Contains("App_Web")
                                       && !ass.Location.Contains("App_global")
                                       && !ass.FullName.Contains("CppCodeProvider")
                                       && !ass.FullName.Contains("WebMatrix")
                                       && !ass.FullName.Contains("SMDiagnostics")
                                       && !String.IsNullOrEmpty(ass.Location))
                                .Distinct();
            return nonSysAssemblies;
        }

        /// <summary>
        /// Retrieves a type by type name searching in the app domain assemblies or in a collection of assemblies passed
        /// </summary>
        /// <param name="typeName">the typename to search, can be either the type's fullname or the assemblyqualified name</param>
        /// <param name="assemblies">list of assemblies to search into, defaults to current app domain's assemblies</param>
        /// <param name="onlyNonSysAssemblies">if true the search is performed only on non system assemblies</param>
        /// <returns>Type.</returns>
        public static Type GetType(string typeName, IEnumerable<Assembly> assemblies = null,
            bool onlyNonSysAssemblies = false, bool isFullName = true)
        {
            Type tfound = null;
            // se typename e` un assemblyqualifiedname cosi` lo becco subito
            tfound = Type.GetType(typeName);

            if (tfound == null)
            {
                // provo a guardare nei tipi cachati
                tfound = Types.Values.SelectMany(list =>
                    //list.Where(t => t.FullName.Equals(typeName) || t.AssemblyQualifiedName.Equals(typeName))
                                                 list.Where(t =>
                                                 {
                                                     var ok = isFullName
                                                                  ? t.FullName.Equals(typeName)
                                                                  : t.Name.Equals(typeName);
                                                     if (!ok)
                                                     {
                                                         //var aname = t.AssemblyQualifiedName.GetAssemblyName();
                                                         var aname =
                                                             t.AssemblyQualifiedName.AssQualNameSplit()
                                                              .Name;
                                                         ok = aname != null && aname.Equals(typeName);
                                                     }
                                                     return ok;
                                                 })).FirstOrDefault();
            }
            if (tfound == null)
            {
                // richiamo questo metodo in modo tale da forzare il caricamento degli assembly non ancora caricati
                var tempNonSysAssemblies = GetNonSysAssemblies().ToArray();
                if (assemblies == null)
                    assemblies = new Assembly[] { };

                assemblies = assemblies.Concat(tempNonSysAssemblies).Distinct();

                if (!onlyNonSysAssemblies)
                    assemblies = assemblies.Concat(AppDomain.CurrentDomain.GetAssemblies()).Distinct();

                tfound = assemblies
                    .Select(ass =>
                    {
                        //
                        try
                        {
                            var type = ass.GetTypes()
                                          .Where(t =>
                                          {

                                              var name = isFullName
                                                             ? t.FullName
                                                             : t.Name;

                                              // prendo solo i tipi pubblici e non anonimi
                                              if (!t.IsPublic && string.IsNullOrEmpty(name))
                                                  return false;

                                              var tmatch = isFullName
                                                               ? t.FullName.Equals(typeName)
                                                               : t.Name.Equals(typeName);

                                              if (!tmatch)
                                              {
                                                  //var aname = t.AssemblyQualifiedName.GetAssemblyName();
                                                  var aname = t
                                                      .AssemblyQualifiedName
                                                      .AssQualNameSplit().Name;
                                                  tmatch = aname != null &&
                                                           aname.Equals(typeName);
                                              }
                                              //var tmatch = t.FullName.Equals(typeName) ||
                                              //    t.AssemblyQualifiedName.AssQualNameSplit().Name.Equals(typeName);
                                              return tmatch;
                                          })
                                          .FirstOrDefault();
                            return type;
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                            return null;
                        }
                    }).FirstOrDefault(t => t != null);
            }

            //System.Diagnostics.Debug.Assert(tfound != null);

            if (tfound != null)
            {
                // guarda se tra i tipi inseriti esiste la base di quello attuale e poi inserisco
                var basetype = Types.Keys.FirstOrDefault(t => tfound.IsAssignableFrom(t));
                if (basetype != null)
                {
                    if (!Types[basetype].Contains(tfound))
                        Types[basetype].Add(tfound);
                }
                else
                    Types[tfound] = new List<Type> { tfound };
            }

            return tfound;
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <param name="baseclass">The baseclass.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="forcerescan">if set to <c>true</c> [forcerescan].</param>
        /// <returns>List{Type}.</returns>
        public static List<Type> GetTypes(Type baseclass, IEnumerable<Assembly> assemblies = null,
            bool forcerescan = false)
        {
            if (assemblies == null)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (!Types.ContainsKey(baseclass))
                Types[baseclass] = new List<Type>();
            else if (!forcerescan)
                return Types[baseclass];

            var bctypes = Types[baseclass];
            foreach (var item in assemblies)
            {
                foreach (var a in item.GetReferencedAssemblies())
                {
                    try
                    {
                        foreach (var tp in Assembly.Load(a).GetTypes())
                        {
                            if (bctypes.Contains(tp))
                                continue;

                            if (baseclass.IsInterface)
                            {
                                if (baseclass.IsAssignableFrom(tp)
                                    //(item.Name != "IMessage" && item.Name.ToLower() != "object")
                                    && !tp.IsInterface
                                    //&& !(item.IsInstanceOfType(typeof(object)))
                                    && !tp.IsAbstract
                                    && tp.IsPublic
                                    )
                                {
                                    bctypes.Add(tp);
                                }
                            }
                            else
                            {
                                if (tp.IsSubclassOf(baseclass)
                                    //(item.Name != "IMessage" && item.Name.ToLower() != "object")
                                    && !tp.IsInterface
                                    //&& !(item.IsInstanceOfType(typeof(object)))
                                    && !tp.IsAbstract
                                    && tp.IsPublic
                                    )
                                {
                                    bctypes.Add(tp);
                                }
                            }
                        }
                    }
                    catch (TypeLoadException exc)
                    {
                    }
                    catch (Exception exc)
                    {
                    }
                }
            }

            return Types[baseclass];
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Class ProxyDomain
        /// </summary>
        private class ProxyDomain : MarshalByRefObject
        {
            #region Fields

            /// <summary>
            /// The proxy
            /// </summary>
            private static ProxyDomain proxy;

            #endregion Fields

            #region Methods

            /// <summary>
            /// Loads the assembly.
            /// </summary>
            /// <param name="aname">The aname.</param>
            /// <returns>Assembly.</returns>
            public static Assembly LoadAssembly(AssemblyName aname)
            {
                if (proxy == null)
                    proxy = new ProxyDomain();
                return proxy.GetAssembly(aname);
            }

            /// <summary>
            /// Gets the assembly.
            /// </summary>
            /// <param name="aname">The aname.</param>
            /// <returns>Assembly.</returns>
            private Assembly GetAssembly(AssemblyName aname)
            {
                try
                {
                    return Assembly.Load(aname);
                }
                catch (Exception ex)
                {
                    //throw new InvalidOperationException("", ex);
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return null;
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}