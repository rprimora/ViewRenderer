using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ViewRenderer.Core
{
    /// <summary>
    /// Represents a view location expander that the razor engine uses to determine the location of the view.
    /// </summary>
    public class ViewLocationExpander : IViewLocationExpander
    {
        #region Members

        private readonly IEnumerable<string> m_directoryLocations;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ViewLocationExpander"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        public ViewLocationExpander(ViewToStringRendererOptions options)
        {
            var root = Directory.GetCurrentDirectory();
            // Find all 'Emails' folders in the directory
            m_directoryLocations = Directory.GetDirectories(root, options.EmailsFolder, SearchOption.AllDirectories);
            // Only include the ones in the running directory, remove the root path (the view engine uses relative paths)
            // and append the file name
            m_directoryLocations = m_directoryLocations.Where(s => s.Contains(options.ContentRoot))
                                                    .Select(s => s.Replace(root, ""))
                                                    .Select(s => s.Insert(s.Length, "\\{0}.cshtml"));

        }

        #endregion

        #region IViewLocationExpander implementation

        /// <summary>
        /// Invoked by a <see cref="Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine"/> to determine potential
        /// locations for a view.
        /// </summary>
        /// <param name="context">The <see cref="Microsoft.AspNetCore.Mvc.Razor.ViewLocationExpanderContext"/> for the current view location expansion operation.</param>
        /// <param name="viewLocations">The sequence of view locations to expand.</param>
        /// <returns>A list of expanded view locations.</returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return m_directoryLocations.Union(viewLocations);
        }

        /// <summary>
        /// Invoked by a <see cref="Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine"/> to determine the
        /// values that would be consumed by this instance of <see cref="Microsoft.AspNetCore.Mvc.Razor.IViewLocationExpander"/>.
        /// The calculated values are used to determine if the view location has changed
        /// since the last time it was located.
        /// </summary>
        /// <param name="context">The <see cref="Microsoft.AspNetCore.Mvc.Razor.ViewLocationExpanderContext"/> for the current view location expansion operation.</param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(ViewLocationExpander);
        }

        #endregion
    }
}
