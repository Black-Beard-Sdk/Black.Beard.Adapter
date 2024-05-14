using Bb.ComponentModel.Translations;
using Bb.Modules.Sgbd.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Bb.Modules.Sgbd.Components
{

    public partial class TableNode : ComponentBase, ITranslateHost
    {

        [Parameter]
        public Table Node { get; set; }

        [Inject]
        public ITranslateService TranslationService { get; set; }

        public string TableName
        {
            get => Node.Title;
            set => Node.Title = value;
        }
              
        public void AutoAddColumn(MouseEventArgs eventArgs)
        {
          AddColumn();
        }

        public Column AddColumn()
        {

            string title = TranslationService.Translate(DatasComponentConstants.Column) + " ";
            int count = 1;
            while (Node.Columns.Any(c => c.Name == (title + count.ToString())))
                count++;

            var column = new Column()
            {
                Name = title + count.ToString(),
                Primary = false,
                Id = Guid.NewGuid()
            };

            var diagram = this.Node.Source.Diagram as SgbdDiagram;
            var t = diagram.GetTechnology();
            if (t != null)
                column.Type = t.DefaultColumnType.Label;

            Node.AddColumn(column);

            return column;

        }

        public void Refresh()
        {
            StateHasChanged();
        }

        public IEnumerable<Column> Columns => Node.Columns;

    }

}
