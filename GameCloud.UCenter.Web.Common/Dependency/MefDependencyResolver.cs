using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web.Http.Dependencies;

namespace GameCloud.UCenter
{
    /// <summary>
    /// The MEF dependency resolver
    /// </summary>
    public class MefDependencyResolver : IDependencyResolver
    {
        private readonly ExportProvider exportProvider;
        private readonly IDependencyResolver innerResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefDependencyResolver" /> class.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="innerResolver">Indicating the inner resolver.</param>
        public MefDependencyResolver(ExportProvider exportProvider, IDependencyResolver innerResolver)
        {
            this.exportProvider = exportProvider;
            this.innerResolver = innerResolver;
        }

        /// <summary>
        /// Begin scope
        /// </summary>
        /// <returns>scope instance</returns>
        public IDependencyScope BeginScope()
        {
            return this;
        }

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Get service by service type
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service instance.</returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var contractName = AttributedModelServices.GetContractName(serviceType);
            var export = this.exportProvider.GetExportedValueOrDefault<object>(contractName);

            if (export == null && this.innerResolver != null)
            {
                return this.innerResolver.GetService(serviceType);
            }
            else
            {
                return export;
            }
        }

        /// <summary>
        /// Get services by service type
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The service list</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var exports = this.exportProvider.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));

            if (exports == null && this.innerResolver != null)
            {
                return this.innerResolver.GetServices(serviceType);
            }
            else
            {
                return exports;
            }
        }
    }
}