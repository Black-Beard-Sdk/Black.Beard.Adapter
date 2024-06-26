﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
#if RAZOR4
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Language;
#else
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;
    using System.CodeDom.Compiler;
#endif

namespace RazorEngine.RazorTemplateEngineInternal
{
    /// <summary>
    /// Entry point to parse Razor files and generate code.
    /// </summary>
    public class RazorTemplateEngineInternal
    {
        private RazorTemplateEngineOptionsInternal _options;

        /// <summary>
        /// Initializes a new instance of <see cref="RazorTemplateEngine"/>.
        /// </summary>
        /// <param name="engine">The <see cref="RazorEngine"/>.</param>
        /// <param name="project">The <see cref="RazorProject"/>.</param>
        public RazorTemplateEngineInternal(Microsoft.AspNetCore.Razor.Language.RazorEngine engine, RazorProject project)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            Engine = engine;
            Project = project;
            _options = new RazorTemplateEngineOptionsInternal();
        }

        /// <summary>
        /// Gets the <see cref="RazorEngine"/>.
        /// </summary>
        public Microsoft.AspNetCore.Razor.Language.RazorEngine Engine { get; }

        /// <summary>
        /// Gets the <see cref="RazorProject"/>.
        /// </summary>
        public RazorProject Project { get; }

        /// <summary>
        /// Options to configure <see cref="RazorTemplateEngine"/>.
        /// </summary>
        public RazorTemplateEngineOptionsInternal Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Parses the template specified by the project item <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The template path.</param>
        /// <returns>The <see cref="RazorCSharpDocument"/>.</returns>
        [Obsolete]
        public RazorCSharpDocument GenerateCode(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Cannot be NULL or Empty", nameof(path));
            }

            var projectItem = Project.GetItem(path);
            return GenerateCode(projectItem);
        }

        /// <summary>
        /// Parses the template specified by <paramref name="projectItem"/>.
        /// </summary>
        /// <param name="projectItem">The <see cref="RazorProjectItem"/>.</param>
        /// <returns>The <see cref="RazorCSharpDocument"/>.</returns>
        public RazorCSharpDocument GenerateCode(RazorProjectItem projectItem)
        {
            if (projectItem == null)
            {
                throw new ArgumentNullException(nameof(projectItem));
            }

            if (!projectItem.Exists)
            {
                throw new InvalidOperationException("Item could not be found");
            }

            var codeDocument = CreateCodeDocument(projectItem);
            return GenerateCode(codeDocument);
        }

        /// <summary>
        /// Parses the template specified by <paramref name="codeDocument"/>.
        /// </summary>
        /// <param name="codeDocument">The <see cref="RazorProjectItem"/>.</param>
        /// <returns>The <see cref="RazorCSharpDocument"/>.</returns>
        public virtual RazorCSharpDocument GenerateCode(RazorCodeDocument codeDocument)
        {
            if (codeDocument == null)
            {
                throw new ArgumentNullException(nameof(codeDocument));
            }

            Engine.Process(codeDocument);
            return codeDocument.GetCSharpDocument();
        }

        /// <summary>
        /// Generates a <see cref="RazorCodeDocument"/> for the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The template path.</param>
        /// <returns>The created <see cref="RazorCodeDocument"/>.</returns>
        [Obsolete]
        public virtual RazorCodeDocument CreateCodeDocument(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Cannot be NULL or Empty", nameof(path));
            }

            var projectItem = Project.GetItem(path);
            return CreateCodeDocument(projectItem);
        }

        /// <summary>
        /// Generates a <see cref="RazorCodeDocument"/> for the specified <paramref name="projectItem"/>.
        /// </summary>
        /// <param name="projectItem">The <see cref="RazorProjectItem"/>.</param>
        /// <returns>The created <see cref="RazorCodeDocument"/>.</returns>
        public virtual RazorCodeDocument CreateCodeDocument(RazorProjectItem projectItem)
        {
            if (projectItem == null)
            {
                throw new ArgumentNullException(nameof(projectItem));
            }

            if (!projectItem.Exists)
            {
                throw new InvalidOperationException("Resources.FormatRazorTemplateEngine_ItemCouldNotBeFound(projectItem.FilePath)");
            }

            var source = RazorSourceDocument.ReadFrom(projectItem);
            var imports = GetImports(projectItem);

            return RazorCodeDocument.Create(source, imports);
        }

        /// <summary>
        /// Gets <see cref="RazorSourceDocument"/> that are applicable to the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The template path.</param>
        /// <returns>The sequence of applicable <see cref="RazorSourceDocument"/>.</returns>
        [Obsolete]
        public IEnumerable<RazorSourceDocument> GetImports(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Cannot be NULL or Empty", nameof(path));
            }

            var projectItem = Project.GetItem(path);
            return GetImports(projectItem);
        }

        /// <summary>
        /// Gets <see cref="RazorSourceDocument"/> that are applicable to the specified <paramref name="projectItem"/>.
        /// </summary>
        /// <param name="projectItem">The <see cref="RazorProjectItem"/>.</param>
        /// <returns>The sequence of applicable <see cref="RazorSourceDocument"/>.</returns>
        public virtual IEnumerable<RazorSourceDocument> GetImports(RazorProjectItem projectItem)
        {
            if (projectItem == null)
            {
                throw new ArgumentNullException(nameof(projectItem));
            }
            var result = new List<RazorSourceDocument>();

            var importProjectItems = GetImportItems(projectItem);
            foreach (var importItem in importProjectItems)
            {
                if (importItem.Exists)
                {
                    // We want items in descending order. FindHierarchicalItems returns items in ascending order.
                    result.Insert(0, RazorSourceDocument.ReadFrom(importItem));
                }
            }

            if (Options.DefaultImports != null)
            {
                result.Insert(0, Options.DefaultImports);
            }

            return result;
        }

        /// <summary>
        /// Gets the sequence of imports with the name specified by <see cref="RazorTemplateEngineOptions.ImportsFileName" />
        /// that apply to <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to look up import items for.</param>
        /// <returns>A sequence of <see cref="RazorProjectItem"/> instances that apply to the
        /// <paramref name="path"/>.</returns>
        [Obsolete]
        public IEnumerable<RazorProjectItem> GetImportItems(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Cannot be NULL or Empty", nameof(path));
            }

            var projectItem = Project.GetItem(path);
            return GetImportItems(projectItem);
        }

        /// <summary>
        /// Gets the sequence of imports with the name specified by <see cref="RazorTemplateEngineOptions.ImportsFileName" />
        /// that apply to <paramref name="projectItem"/>.
        /// </summary>
        /// <param name="projectItem">The <see cref="RazorProjectItem"/> to look up import items for.</param>
        /// <returns>A sequence of <see cref="RazorProjectItem"/> instances that apply to the
        /// <paramref name="projectItem"/>.</returns>
        public virtual IEnumerable<RazorProjectItem> GetImportItems(RazorProjectItem projectItem)
        {
            var importsFileName = Options.ImportsFileName;
            if (!string.IsNullOrEmpty(importsFileName))
            {
                return Project.FindHierarchicalItems(projectItem.FilePath, importsFileName);
            }

            return Enumerable.Empty<RazorProjectItem>();
        }
    }
}
