using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using MefContrib.Integration.Exporters;
using MefContrib.Properties;

namespace MefContrib.Integration
{
    /// <summary>
    /// Provides common services.
    /// </summary>
    public static class ContainerServices
    {
        /// <summary>
        /// Resolves an object wrapped inside <see cref="Lazy{T}"/> from an <see cref="ExportProvider"/>.
        /// </summary>
        /// <param name="exportProvider">Export provider.</param>
        /// <param name="type">Type to be resolved.</param>
        /// <param name="name">Optional name.</param>
        /// <returns>Resolved instance or null, if no instance has been found.</returns>
        /// <remarks>
        /// Does not resolve instances which are provided by means of
        /// <see cref="RegistrationBasedFactoryExportProvider"/>.
        /// </remarks>
        public static Lazy<object> Resolve(ExportProvider exportProvider, Type type, string name)
        {
            var exports = exportProvider.GetExports(type, null, name);

            if (exports.Count() == 0)
                return null;

            if (exports.Count() > 1)
                throw new ImportCardinalityMismatchException(Resources.TooManyInstances);

            var lazyExport = exports.First();
            var lazyExportMetadata = lazyExport.Metadata as IDictionary<string, object>;
            if (lazyExportMetadata != null &&
                lazyExportMetadata.ContainsKey(ExporterConstants.IsContractBasedExportMetadataName) &&
                true.Equals(lazyExportMetadata[ExporterConstants.IsContractBasedExportMetadataName]))
            {
                return null;
            }

            return lazyExport;
        }

        /// <summary>
        /// Resolves an collection of objects wrapped inside <see cref="Lazy{T}"/>
        /// from an <see cref="ExportProvider"/>.
        /// </summary>
        /// <param name="exportProvider">Export provider.</param>
        /// <param name="type">Type to be resolved.</param>
        /// <param name="name">Optional name.</param>
        /// <returns>Resolved collection of lazy instances or null, if no instance has been found.</returns>
        /// <remarks>
        /// Does not resolve instances which are provided by means of
        /// <see cref="RegistrationBasedFactoryExportProvider"/>.
        /// </remarks>
        public static IEnumerable<Lazy<object>> ResolveAll(ExportProvider exportProvider, Type type, string name)
        {
            var exports = exportProvider.GetExports(type, null, name);

            if (exports.Count() == 0)
                return Enumerable.Empty<Lazy<object>>();
            
            var list = new List<Lazy<object>>();

            foreach (var export in exports)
            {
                var lazyExportMetadata = export.Metadata as IDictionary<string, object>;
                if (lazyExportMetadata != null &&
                    lazyExportMetadata.ContainsKey(ExporterConstants.IsContractBasedExportMetadataName) &&
                    true.Equals(lazyExportMetadata[ExporterConstants.IsContractBasedExportMetadataName]))
                {
                    continue;
                }

                list.Add(export);
            }

            return list;
        }
    }
}