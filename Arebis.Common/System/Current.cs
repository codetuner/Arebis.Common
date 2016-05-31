using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Factories.DateTime;
using System.Reflection;
using System.Factories.Localization;
using System.Factories.AppContext;
using System.Factories.ExceptionHandler;

namespace System
{
    /// <summary>
    /// Provides unit-testable system information.
    /// </summary>
    public static class Current
    {
        private static IAppContextFactory appContextFactory;
        private static IDateTimeFactory dateTimeFactory;
        private static IDateTimeOffsetFactory dateTimeOffsetFactory;
        private static IExceptionHandlerFactory exceptionHandlerFactory;
        private static ILocalizationFactory localizationFactory;

        /// <summary>
        /// The unit-testable AppContext factory.
        /// If set to null, reverts to the default AppContext factory.
        /// </summary>
        public static IAppContextFactory AppContext
        {
            get
            {
                if (appContextFactory == null)
                {
                    appContextFactory = new DefaultAppContextFactory();
                }
                return appContextFactory;
            }
            set
            {
                appContextFactory = value;
            }
        }

        /// <summary>
        /// The unit-testable DateTime factory.
        /// If set to null, reverts to the default DateTime factory.
        /// </summary>
        public static IDateTimeFactory DateTime
        {
            get
            {
                if (dateTimeFactory == null)
                {
                    dateTimeFactory = new DefaultDateTimeFactory();
                }
                return dateTimeFactory;
            }
            set
            {
                dateTimeFactory = value;
            }
        }

        /// <summary>
        /// The unit-testable DateTime factory.
        /// If set to null, reverts to the default DateTime factory.
        /// </summary>
        public static IDateTimeOffsetFactory DateTimeOffset
        {
            get
            {
                if (dateTimeOffsetFactory == null)
                {
                    dateTimeOffsetFactory = new DefaultDateTimeOffsetFactory();
                }
                return dateTimeOffsetFactory;
            }
            set
            {
                dateTimeOffsetFactory = value;
            }
        }

        /// <summary>
        /// The unit-testable ExceptionHandler factory.
        /// If set to null, reverts to the default ExceptionHandler factory.
        /// </summary>
        public static IExceptionHandlerFactory ExceptionHandler
        {
            get
            {
                if (exceptionHandlerFactory == null)
                {
                    exceptionHandlerFactory = new DefaultExceptionHandlerFactory();
                }
                return exceptionHandlerFactory;
            }
            set
            {
                exceptionHandlerFactory = value;
            }
        }

        /// <summary>
        /// The unit-testable Localization factory.
        /// If set to null, reverts to the default Localization factory.
        /// </summary>
        public static ILocalizationFactory Localization
        {
            get
            {
                if (localizationFactory == null)
                {
                    localizationFactory = new DefaultLocalizationFactory();
                }
                return localizationFactory;
            }
            set
            {
                localizationFactory = value;
            }
        }
        
        /// <summary>
        /// Name of this executable (entry assembly).
        /// </summary>
        public static AssemblyName ExecutableName
        {
            get { return Assembly.GetEntryAssembly().GetName(); }
        }

        /// <summary>
        /// Copyright of this executable (entry assembly).
        /// </summary>
        public static string ExecutableCopyright(Assembly assembly)
        {
            return ((AssemblyCopyrightAttribute)assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
        }
    }
}
