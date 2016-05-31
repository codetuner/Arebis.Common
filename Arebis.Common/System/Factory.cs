using System;
using System.Collections.Generic;
using System.Text;
using System.Factories.DateTime;

namespace System
{
    [Obsolete("Replaced by System.Current")]
    public static class Factory
    {
        [Obsolete("Replaced by System.Current.DateTime")]
        public static IDateTimeFactory DateTime
        {
            get 
            {
                return Current.DateTime;
            }
            set
            {
                Current.DateTime = value;
            }
        }
    }
}
