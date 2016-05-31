using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Arebis.Types
{
    /// <summary>
    /// Represents a multilanguage indexable field type mapping to language specific properties.
    /// Supported languages is predefined to be Dutch, English, French and German.
    /// </summary>
    /// <typeparam name="T">Type of the owner of the field of this type.</typeparam>
    /// <example>
    /// <code>
    /// [Serializable, DataContract]
    /// public class SomeEntity : IDeserializationCallback
    /// {
    ///     private static MultiLangStringMap&lt;SomeEntity&gt; multiLangNameMap = new MultiLangStringMap&lt;SomeEntity&gt;(x => x.NameNl, x => x.NameFr, null, null); 
    /// 
    ///     [DataMember]
    ///     public string NameNl { get; set; }
    /// 
    ///     [DataMember]
    ///     public string NameFr { get; set; }
    /// 
    ///     public string Name
    ///     {
    ///         get { return multiLangNameMap.GetValue(this); }
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class MultiLangStringMap<T>
    {
        private Func<T, string> _compiledNl;
        private Func<T, string> _compiledFr;
        private Func<T, string> _compiledDe;
        private Func<T, string> _compiledEn;

        /// <summary>
        /// Constructs a new MultiLangStringMap.
        /// </summary>
        /// <param name="nl">Accessor espression to the Dutch language property.</param>
        /// <param name="fr">Accessor espression to the French language property.</param>
        /// <param name="de">Accessor espression to the German language property.</param>
        /// <param name="en">Accessor espression to the English language property.</param>
        public MultiLangStringMap(
            Expression<Func<T, String>> nl,
            Expression<Func<T, String>> fr,
            Expression<Func<T, String>> de,
            Expression<Func<T, String>> en)
        {
            _compiledNl = nl.Compile();
            _compiledFr = fr.Compile();
            _compiledDe = de.Compile();
            _compiledEn = en.Compile();
        }

        /// <summary>
        /// Returns the property value for the current threads closest matching language.
        /// </summary>
        public string GetValue(T instance)
        {
            return GetValue(instance, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
        }

        /// <summary>
        /// Returns the property value closest matching the given language name.
        /// </summary>
        public string GetValue(T instance, string twoLetterISOLanguageName)
        {
            switch (twoLetterISOLanguageName)
            {
                case "nl":
                    return _compiledNl(instance)
                        ?? _compiledFr(instance)
                        ?? _compiledDe(instance)
                        ?? _compiledEn(instance);
                case "fr":
                    return _compiledFr(instance)
                        ?? _compiledNl(instance)
                        ?? _compiledDe(instance)
                        ?? _compiledEn(instance);
                case "de":
                    return _compiledDe(instance)
                        ?? _compiledFr(instance)
                        ?? _compiledNl(instance)
                        ?? _compiledEn(instance);
                default:
                    return _compiledEn(instance)
                        ?? _compiledNl(instance)
                        ?? _compiledFr(instance)
                        ?? _compiledDe(instance);
            }
        }
    }
}
