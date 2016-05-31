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
    /// A scope during which the given cultures are set as current on the thread.
    /// </summary>
    public class CultureScope : IDisposable
    {
        private CultureInfo previousCulture;
        private CultureInfo previousUiCulture;

        public CultureScope(string cultureName)
            : this(new CultureInfo(cultureName), null)
        { }

        public CultureScope(string cultureName, string uiCultureName)
            : this(new CultureInfo(cultureName), new CultureInfo(uiCultureName))
        { }

        public CultureScope(CultureInfo culture)
            : this(culture, null)
        { }

        public CultureScope(CultureInfo culture, CultureInfo uiCulture)
        {
            this.previousCulture = Thread.CurrentThread.CurrentCulture;
            this.previousUiCulture = Thread.CurrentThread.CurrentUICulture;

            if (culture != null) Thread.CurrentThread.CurrentCulture = culture;
            if (uiCulture != null) Thread.CurrentThread.CurrentUICulture = uiCulture;
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
        }

        public CultureInfo CurrentUICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
        }

        public virtual void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = this.previousCulture;
            Thread.CurrentThread.CurrentUICulture = this.previousUiCulture;
        }
    }
}
