namespace GF.UCenter.Web.Common
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Provide a class for MEF support MVC controller.
    /// </summary>
    [Export]
    public class MefControllerFactory : DefaultControllerFactory
    {
        private readonly ExportProvider exportProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefControllerFactory" /> class.
        /// </summary>
        /// <param name="exportProvider">Indicating the export provider.</param>
        [ImportingConstructor]
        public MefControllerFactory(ExportProvider exportProvider)
        {
            this.exportProvider = exportProvider;
        }

        /// <summary>
        /// Releases the specified controller.
        /// </summary>
        /// <param name="controller">Indicating the controller</param>
        public override void ReleaseController(IController controller)
        {
            ((IDisposable)controller).Dispose();
        }

        /// <summary>
        /// Get controller instance.
        /// </summary>
        /// <param name="requestContext">Indicating the request context.</param>
        /// <param name="controllerType">Indicating the controller type.</param>
        /// <returns>The controller instance.</returns>
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
    }
}