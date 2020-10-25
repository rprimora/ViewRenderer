using System.Threading.Tasks;

namespace ViewRenderer.Abstractions
{
    /// <summary>
    /// Describes an interface to view render service.
    /// </summary>
    public interface IViewToStringRenderer
    {
        /// <summary>
        /// When implemented asynchronously renders a given view to string.
        /// </summary>
        /// <typeparam name="TModel">View model type.</typeparam>
        /// <param name="name">View name.</param>
        /// <param name="model">Model.</param>
        /// <returns>String representation of the view.</returns>
        Task<string> RenderViewToStringAsync<TModel>(string name, TModel model);
    }
}