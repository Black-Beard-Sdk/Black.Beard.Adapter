using System.Diagnostics;

namespace Bb.Diagrams
{

    [DebuggerDisplay("S:{Source} - {Message}")]
    public class DiagramDiagnostic
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


}
