// ------------------------------------------------------------------------
// Company:					T&TSistemi s.r.l.
// Date:					2013/05/13, 10:38
// Project:					11STD3562_Copenhagen_Ring
// Developer:				Francesco Betti
// Software Module Name:	Copenhagen.Commons.dll
// ------------------------------------------------------------------------
namespace WB.Commons.Helpers
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Class Regex
    /// </summary>
    public static class Regex
    {
        #region Methods

        /// <summary>
        /// Evaluates complexity of a Regex
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal RegexComplexity(System.Text.RegularExpressions.Regex regex)
        {
            if (regex == null)
                return 0;

            decimal numer = regex.GetGroupNames().Length*regex.GetGroupNames().Rank
                            + regex.GetGroupNumbers().Length*regex.GetGroupNumbers().Rank;
            decimal denom = regex.ToString().Length;
            decimal complexity = numer/denom;

            // https://www.desmos.com/calculator
            // formula: $\ln \left(\left(\frac{x}{2}+1\right)^{\frac{1}{\frac{x}{2}+1}}\right)^{\frac{1}{\frac{x}{2}+1}}+1$
            double x = regex.GetGroupNames().Length*regex.GetGroupNames().Rank
                       + regex.GetGroupNumbers().Length*regex.GetGroupNumbers().Rank;
            x = 1 + x/5 + .0000000001;
            complexity = 1 + (decimal) Math.Log(Math.Pow(Math.Pow(x, 1/x), 1/x));

            return complexity;
        }

        /// <summary>
        /// Return a Regex object from a string with wildcards
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="ignorecase">if set to <c>true</c> [ignorecase].</param>
        /// <returns>System.Text.RegularExpressions.Regex.</returns>
        public static System.Text.RegularExpressions.Regex WCard2Regex(string pattern, bool ignorecase = true)
        {
            if (string.IsNullOrEmpty(pattern))
                return null;
            pattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                                  .Replace("\\*", ".*")
                                  .Replace("\\?", ".") + "$";
            if (ignorecase)
                return new System.Text.RegularExpressions.Regex(pattern, RegexOptions.IgnoreCase);
            return new System.Text.RegularExpressions.Regex(pattern);
        }

        #endregion Methods
    }
}