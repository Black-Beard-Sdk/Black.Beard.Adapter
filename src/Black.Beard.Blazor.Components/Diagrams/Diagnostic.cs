using Microsoft.Fast.Components.FluentUI;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Bb.Diagrams
{

    [DebuggerDisplay("S:{Source} - {Message}")]
    public class Diagnostic
    {

        public DiagnosticLevel Level { get; set; }

        public string? Message { get; set; }

        public string? Source { get; set; }

        public TargetSource? Target { get; set; }

    }

    public enum DiagnosticLevel
    {
        Info,
        Warning,
        Error
    }

    public class TargetSource
    {
        private object model;

        public TargetSource(object model)
        {
            this.model = model;
        }

        public Guid Feature { get; set; }

        public List<Guid>? Target { get; set; }

        public object Model => model;

    }


}
