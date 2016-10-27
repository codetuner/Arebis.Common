using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arebis.Globalization
{
    /// <summary>
    /// Withing a CultureScope, CurrentCulture and CurrentUICulture can be overriden. When disposing
    /// the scope, the original cultures are restored.
    /// </summary>
    public class CultureScope : IDisposable
    {
        private CultureInfo originalCulture;
        private CultureInfo originalUICulture;

        /// <summary>
        /// Creates as CultureScope with invariant culture.
        /// </summary>
        public static CultureScope Invariant
        {
            get
            {
                return new CultureScope(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Overrides only the CurrentCulture within this scope.
        /// </summary>
        /// <param name="cultureId">LCID of a CultureInfo or a Windows-only culture identifier to set as CurrentCulture.</param>
        public CultureScope(int cultureId)
            : this(new CultureInfo(cultureId))
        { }

        /// <summary>
        /// Overrides the CurrentCulture and/or CurrentUICulture within this scope.
        /// </summary>
        /// <param name="cultureId">LCID of a CultureInfo or a Windows-only culture identifier, or null to leave the CurrentCulture untouched.</param>
        /// <param name="uiCultureId">LCID of a CultureInfo or a Windows-only culture identifier, or null to leave the CurrentUICulture untouched.</param>
        public CultureScope(int? cultureId, int? uiCultureId)
            : this((cultureId.HasValue) ? new CultureInfo(cultureId.Value) : null, (uiCultureId.HasValue) ? new CultureInfo(uiCultureId.Value) : null)
        { }

        /// <summary>
        /// Overrides only the CurrentCulture within this scope.
        /// </summary>
        /// <param name="cultureName">CultureInfo name to set as CurrentCulture.</param>
        public CultureScope(string cultureName)
            : this((cultureName != null) ? new CultureInfo(cultureName) : null)
        { }

        /// <summary>
        /// Overrides the CurrentCulture and/or CurrentUICulture within this scope.
        /// </summary>
        /// <param name="cultureName">CultureInfo name to set as CurrentCulture, or null to leave the CurrentCulture untouched.</param>
        /// <param name="uiCultureName">CultureInfo name to set as CurrentUICulture, or null to leave the CurrentUICulture untouched.</param>
        public CultureScope(string cultureName, string uiCultureName)
            : this((cultureName != null) ? new CultureInfo(cultureName) : null, (uiCultureName != null) ? new CultureInfo(uiCultureName) : null)
        { }

        /// <summary>
        /// Overrides only the CurrentCulture within this scope.
        /// </summary>
        /// <param name="culture">CultureInfo to set as CurrentCulture.</param>
        public CultureScope(CultureInfo culture)
            : this(culture, null)
        { }

        /// <summary>
        /// Overrides the CurrentCulture and/or CurrentUICulture within this scope.
        /// </summary>
        /// <param name="culture">CultureInfo to set as CurrentCulture, or null to leave the CurrentCulture untouched.</param>
        /// <param name="uiCulture">CultureInfo to set as CurrentUICulture, or null to leave the CurrentUICulture untouched.</param>
        public CultureScope(CultureInfo culture, CultureInfo uiCulture)
        {
            // Store original values:
            this.originalCulture = CultureInfo.CurrentCulture;
            this.originalUICulture = CultureInfo.CurrentUICulture;

            // Set scope values:
            System.Threading.Thread.CurrentThread.CurrentCulture = culture ?? System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = uiCulture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Disposes the CultureScope, restoring the original CurrentCulture and CurrentUICulture.
        /// </summary>
        public void Dispose()
        {
            // Restore original values:
            System.Threading.Thread.CurrentThread.CurrentCulture = this.originalCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = this.originalUICulture;
        }
    }
}
