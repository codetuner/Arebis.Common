using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Arebis.Composition;

namespace Arebis.Web.Mvc
{
    public class ComposingControllerFactory : IControllerFactory
    {
        private IControllerFactory innerFactory;

        public ComposingControllerFactory(IControllerFactory innerFactory)
        {
            this.innerFactory = innerFactory;
        }

        #region IControllerFactory Members

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            // Create a controller wrapping an inner controller:
            return new ComposingController(this.innerFactory.CreateController(requestContext, controllerName));
        }

        public void ReleaseController(IController controller)
        {
            // Release the inner controller:
            this.innerFactory.ReleaseController(((ComposingController)controller).InnerController);
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return this.innerFactory.GetControllerSessionBehavior(requestContext, controllerName);
        }

        #endregion
    }

    internal class ComposingController : IController
    {
        private IController innerController;

        internal ComposingController(IController innerController)
        {
            this.innerController = innerController;
        }

        public IController InnerController
        {
            get { return this.innerController; }
        }

        public void Execute(System.Web.Routing.RequestContext requestContext)
        {
            using (var compositionContainer = new CompositionContainer(CompositionSettings.DefaultCatalog))
            {
                // Compose inner controller:
                compositionContainer.ComposeParts(this.innerController);

                // Delegate execution to inner controller:
                this.innerController.Execute(requestContext);
            }
        }
    }
}
