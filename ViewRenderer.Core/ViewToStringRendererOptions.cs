namespace ViewRenderer.Core
{
    /// <summary>
    /// Options used to instantiate <see cref="ViewLocationExpander"/>.
    /// </summary>
    public class ViewToStringRendererOptions
    {
        /// <summary>
        /// Gets or sets the content root path.
        /// </summary>
        public string ContentRoot { get; set; }

        /// <summary>
        /// Gets or sets the name of the folder that contains email views. Default is 'Emails'.
        /// </summary>
        public string EmailsFolder { get; set; } = "Emails";
    }
}