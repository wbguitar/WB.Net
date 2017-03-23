// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/15, 08:17
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Class Factory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Factory<T>
        where T : new()
    {
        #region Methods

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns>`0.</returns>
        public virtual T CreateInstance()
        {
            return new T();
        }

        #endregion Methods
    }

    /// <summary>
    /// Class FactoryWithParams
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FactoryWithParams<T>
    {
        #region Fields

        /// <summary>
        /// The assemblies
        /// </summary>
        private readonly IEnumerable<Assembly> assemblies;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryWithParams{T}" /> class.
        /// </summary>
        public FactoryWithParams()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryWithParams{T}" /> class.
        /// </summary>
        /// <param name="_assemblies">The _assemblies.</param>
        public FactoryWithParams(IEnumerable<Assembly> _assemblies = null)
        {
            if (_assemblies == null)
                _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            assemblies = _assemblies;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="parms">The parms.</param>
        /// <returns>`0.</returns>
        public virtual T CreateInstance(params object[] parms)
        {
            var ptypes = parms.Select(p=>p.GetType()).ToArray();
            var typeFound = assemblies
                .SelectMany(ass=>ass.GetTypes())
                .Where(t=> { return typeof (T).Equals(t) && (t.GetConstructor(ptypes) != null); }).FirstOrDefault();

            if (typeFound == null)
                return default(T);

            return (T) typeFound.GetConstructor(ptypes).Invoke(parms);
        }

        #endregion Methods
    }
}