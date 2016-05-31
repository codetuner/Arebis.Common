using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
 
namespace Arebis.Web.Mvc
{
    /// <summary>
    /// An MVC ActionName Selector for actions handling form buttons.
    /// </summary>
    public class ButtonHandlerAttribute : ActionNameSelectorAttribute
    {
        private readonly Regex ButtonNameParser = new Regex("^(?<name>.*?)(\\[(?<arg>.+?)\\])*$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
 
        private string argumentNames;
        private string[] arguments;
 
        /// <summary>
        /// Indicates this action handles actions for a button with the name of the action method.
        /// </summary>
        public ButtonHandlerAttribute()
        {
            this.ValueArgumentName = "value";
        }
 
        /// <summary>
        /// Whether GET-requests are allowed (by default not allowed).
        /// </summary>
        public bool AllowGetRequests { get; set; }
 
        /// <summary>
        /// Name of the MVC action.
        /// </summary>
        public string ActionName { get; set; }
 
        /// <summary>
        /// Name of the button (without arguments).
        /// </summary>
        public string ButtonName { get; set; }
 
        /// <summary>
        /// Comma-separated list of argument names to bind to the button arguments.
        /// </summary>
        public string ArgumentNames
        {
            get
            {
                return this.argumentNames;
            }
            set
            {
                this.argumentNames = value;
                if (String.IsNullOrWhiteSpace(value))
                    this.arguments = null;
                else
                    this.arguments = value.Split(',').Select(s => s.Trim()).ToArray();
            }
        }
 
        /// <summary>
        /// Name of the method argument to bind to the button value.
        /// </summary>
        public string ValueArgumentName { get; set; }
 
        /// <summary>
        /// Determines whether the action name is valid in the specified controller context.
        /// </summary>
        public override bool IsValidName(ControllerContext controllerContext, string actionName, System.Reflection.MethodInfo methodInfo)
        {
            // Reject GET requests if not allowed:
            if (!AllowGetRequests)
                if (controllerContext.HttpContext.Request.GetHttpMethodOverride().Equals("GET", StringComparison.OrdinalIgnoreCase))
                    return false;
 
            // Check ActionName if given:
            if (this.ActionName != null)
                if (!this.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase))
                    return false;
 
            // Check button name:
            var values = new NameValueCollection();
            if ((this.arguments == null) || (this.arguments.Length == 0))
            {
                // Buttonname has no args, perform an exact match:
                var buttonName = this.ButtonName ?? methodInfo.Name;
 
                // Return false if button not found:
                if (controllerContext.HttpContext.Request[buttonName] == null)
                    return false;
 
                // Button is found, add button value:
                if (this.ValueArgumentName != null)
                    values.Add(this.ValueArgumentName, controllerContext.HttpContext.Request[buttonName]);
            }
            else
            { 
                // Buttonnname has arguments, perform a match up to the first argument:
                var buttonName = this.ButtonName ?? methodInfo.Name;
                var buttonNamePrefix = buttonName + "[";

                string buttonFieldname = null;
                string[] args = null;
                foreach (var fieldname in controllerContext.HttpContext.Request.Form.AllKeys
                    .Union(controllerContext.HttpContext.Request.QueryString.AllKeys))
                {
                    if (fieldname.StartsWith(buttonNamePrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var match = ButtonNameParser.Match(fieldname);
                        if (match == null) continue;
                        args = match.Groups["arg"].Captures.OfType<Capture>().Select(c => c.Value).ToArray();
                        if (args.Length != this.arguments.Length) continue;
                        buttonFieldname = fieldname;
                        break;
                    }
                }
 
                // Return false if button not found:
                if (buttonFieldname == null)
                    return false;
 
                // Button is found, add button value:
                if (this.ValueArgumentName != null)
                    values.Add(this.ValueArgumentName, controllerContext.HttpContext.Request[buttonFieldname]);
 
                // Also add arguments:
                for(int i=0; i<this.arguments.Length; i++)
                {
                    values.Add(this.arguments[i], args[i]);
                }
            }
 
            // Install a new ValueProvider for the found values:
            var valueProviders = new List<IValueProvider>();
            valueProviders.Add(new NameValueCollectionValueProvider(values, Thread.CurrentThread.CurrentCulture));
            valueProviders.Add(controllerContext.Controller.ValueProvider);
            controllerContext.Controller.ValueProvider = new ValueProviderCollection(valueProviders);
 
            // Return success:
            return true;
        }
    }
}
