using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace GF.UCenter.Web
{
    public class MefControllerFactory : DefaultControllerFactory
    {
        private readonly ExportProvider exportProvider;

        public MefControllerFactory(ExportProvider exportProvider)
        {
            this.exportProvider = exportProvider;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller = null;
            try
            {
                controller = this.exportProvider.GetExportedValue<object>(controllerType.FullName) as IController;
                if (controller != null)
                {
                    return controller;
                }
            }
            catch (Exception)
            {
                // todo: log the error.
            }

            return base.GetControllerInstance(requestContext, controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            ((IDisposable)controller).Dispose();
        }
    }
}