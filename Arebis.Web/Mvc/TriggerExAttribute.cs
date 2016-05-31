using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using System.Collections.Specialized;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// An Action selector attribute that selects the action by matching a
    /// regular expression against a 'trigger' name (i.e. name of a button)
    /// and adds named group values of the regular expression to the
    /// binding context.
    /// </summary>
    /// <example>
    /// [TriggerEx("triggerButton_(?'itemId'[0-9]+)")]
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TriggerExAttribute : ActionNameSelectorAttribute
    {
        private Regex expressionRegex;
        private string[] groupNames;

        /// <summary>
        /// Instantiates a new TriggerExAttribute instance.
        /// </summary>
        /// <param name="triggerMatchExpression">The regular expression to match the trigger name against.</param>
        public TriggerExAttribute(string triggerMatchExpression)
        {
            this.TriggerMatchExpression = triggerMatchExpression;
        }

        /// <summary>
        /// The regular expression to match the trigger name against.
        /// Named group values are added to the binding context.
        /// </summary>
        public string TriggerMatchExpression {
            get
            {
                return this.expressionRegex.ToString();
            }
            set
            {
                this.expressionRegex = new Regex(value);
                this.groupNames = this.expressionRegex.GetGroupNames();
            }
        }

        /// <summary>
        /// Whether the given action matches this selector.
        /// </summary>
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            
            // Search for a matching trigger:
            foreach (string key in controllerContext.HttpContext.Request.Form.AllKeys)
            {
                Match match = this.expressionRegex.Match(key);
                if (match.Success)
                {
                    // Build a SimpleValueProvider with the values of the named
                    // groups in the TriggerMatchExpression;
                    // Named group values are also added to the RouteData DataTokens:
                    NameValueCollection values = new NameValueCollection();
                    foreach (string groupName in this.groupNames)
                    {
                        if (groupName == "0") continue;
                        values[groupName.ToLower(currentCulture)] = match.Groups[groupName].Value;
                        controllerContext.RouteData.DataTokens[groupName] = match.Groups[groupName].Value;
                    }

                    // Install a new ValueProvider chaining both the new SimpleValueProvider
                    // and the original value provider:
                    List<IValueProvider> valueProviders = new List<IValueProvider>();
                    valueProviders.Add(new NameValueCollectionValueProvider(values, currentCulture));
                    valueProviders.Add(controllerContext.Controller.ValueProvider);
                    controllerContext.Controller.ValueProvider = new ValueProviderCollection(valueProviders);

                    // Indicate action selector is valid:
                    return true;
                }
            }

            // When no matching trigger is found, indicate selector is not valid:
            return false;
        }
    }
}
