using System.Diagnostics;

namespace Bb.UIComponents
{

    [DebuggerDisplay("{Display}")]
    public class DynamicServerMenu : List<DynamicServerMenu>
    {

        public DynamicServerMenu()
        {

            this.Roles = new List<string>();
            this.Icon = string.Empty;
            this.Action = ActionReference.Default;
        }

        public DynamicServerMenu(int capacity)
            : base(capacity)
        {
            this.Roles = new List<string>();
            this.Icon = string.Empty;
            this.Action = ActionReference.Default;
        }

        public bool IsEmpty { get => this.Count == 0; }

        public bool ViewGuard { get; set; }

        public bool EnabledGuard { get; set; }


        public string? Display { get; set; }

        public Guid Uui { get; set; }

        public string? Type { get; set; }

        public List<string> Roles { get; set; }

        public bool HasImage { get => !string.IsNullOrEmpty(this.Icon); }

        public string Icon { get; set; }

        public string? KeyboardArrowDown { get; set; }

        public ActionReference Action { get; set; }

    }


}
