using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using ViewRenderer.Abstractions;

namespace ViewRenderer.Core
{
    /// <summary>
    /// Represents a view renderer that can output a view in to a string.
    /// </summary>
    public class ViewToStringRenderer : IViewToStringRenderer
    {
        #region Members

        private readonly IHostingEnvironment m_hostingEnvironment;
        private readonly IRazorViewEngine m_razorViewEngine;
        private readonly ITempDataProvider m_tempDataProvider;
        private readonly IServiceProvider m_serviceProvider;
        private readonly ViewToStringRendererOptions m_options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ViewToStringRenderer"/> class.
        /// </summary>
        /// <param name="razorViewEngine">Razor view engine.</param>
        /// <param name="tempDataProvider">Temporary data provider.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public ViewToStringRenderer(IHostingEnvironment hostingEnvironment, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, IOptions<ViewToStringRendererOptions> options)
        {
            m_hostingEnvironment = hostingEnvironment;
            m_razorViewEngine = razorViewEngine;
            m_tempDataProvider = tempDataProvider;
            m_serviceProvider = serviceProvider;
            m_options = options.Value;
        }

        #endregion

        #region IViewToStringRenderer implementation

        /// <summary>
        /// Asynchronoulsy renders a view to string.
        /// </summary>
        /// <typeparam name="TModel">View model type.</typeparam>
        /// <param name="name">View name.</param>
        /// <param name="model">Model.</param>
        /// <returns>String representation of the view.</returns>
        public async Task<string> RenderViewToStringAsync<TModel>(string name, TModel model)
        {
            var actionContext = GetActionContext();

            var viewEngineResult = m_razorViewEngine.FindView(actionContext, name, false);

            if (!viewEngineResult.Success)
            {
                viewEngineResult = m_razorViewEngine.GetView(RelativeAssemblyDirectory(), $"Views/{m_options.EmailsFolder}/{name}.cshtml", false);
                if(!viewEngineResult.Success)
                    throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(actionContext.HttpContext, m_tempDataProvider),
                    output,
                    new HtmlHelperOptions()
                );

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns the action context.
        /// </summary>
        /// <returns><see cref="ActionContext"/> object.</returns>
        private ActionContext GetActionContext()
        {
            return new ActionContext(new DefaultHttpContext() { RequestServices = m_serviceProvider }, new RouteData(), new ActionDescriptor());
        }

        public string RelativeAssemblyDirectory()
        {
            var contentRootPath = m_hostingEnvironment.ContentRootPath;
            string executingAssemblyDirectoryAbsolutePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string executingAssemblyDirectoryRelativePath = System.IO.Path.GetRelativePath(contentRootPath, executingAssemblyDirectoryAbsolutePath);
            return executingAssemblyDirectoryRelativePath;
        }

        #endregion
    }

    /// <summary>
    /// Contains extension methods for <see cref="ViewToStringRenderer"/>.
    /// </summary>
    public static class ViewToStringRendererExtensions
    {
        /// <summary>
        /// Adds <see cref="IViewToStringRenderer"/> service to the service collection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="options">Options.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRazorViewToStringRenderer(this IServiceCollection services, Action<ViewToStringRendererOptions> options)
        {
            ViewToStringRendererOptions viewToStringrendererOptions = new ViewToStringRendererOptions();
            options(viewToStringrendererOptions);
            services.Configure<RazorViewEngineOptions>(o => o.ViewLocationExpanders.Add(new ViewLocationExpander(viewToStringrendererOptions)));
            services.Configure(options);
            services.AddTransient<IViewToStringRenderer, ViewToStringRenderer>();
            return services;
        }
    }
}