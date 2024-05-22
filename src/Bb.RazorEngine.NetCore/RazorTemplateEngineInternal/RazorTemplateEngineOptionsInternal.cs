using Microsoft.AspNetCore.Razor.Language;

namespace RazorEngine.RazorTemplateEngineInternal
{
    /// <summary>
    /// Options for code generation in the <see cref="RazorTemplateEngine"/>.
    /// </summary>
    public sealed class RazorTemplateEngineOptionsInternal
    {
        /// <summary>
        /// Gets or sets the file name of the imports file (e.g. _ViewImports.cshtml).
        /// </summary>
        public string ImportsFileName { get; set; }

        /// <summary>
        /// Gets or sets the default set of imports.
        /// </summary>
        public RazorSourceDocument DefaultImports { get; set; }
    }
}
