using Bb.Diagrams;
using Bb.Modules.Sgbd.Models;
using Bb.TypeDescriptors;
using Blazor.Diagrams;
using Blazor.Diagrams.Core;
using System.Text.Json.Serialization;

namespace Bb.Modules.Bpms.Models
{

    public class BpmsDiagram : Diagrams.Diagram
    {

        public static Guid Key = new Guid("0E61164D-92C8-4A3E-8BD8-68EF1EAAB2BA");

        public BpmsDiagram()
            : base(Key, false)
        {

        }

        public override void InitializeToolbox(DiagramToolbox toolbox)
        {

            toolbox
                  .Add(new SwimLaneTool())

                  ;

        }

        static BpmsDiagram()
        {

            //DynamicTypeDescriptionProvider.Configure<Table>(c =>
            //{

            //    c.RemoveProperties("ControlledSize", "Title", "Selected", "Id", "Locked", "Visible");

            //    c.Property(c => c.Name, i =>
            //    {
            //        i.PropertyOrder(1)
            //        ;
            //    });

            //    c.Property(u => u.Group, i =>
            //    {
            //        i.DisableValidation();
            //    });

            //    c.Property(u => u.HasPrimaryColumn, i =>
            //    {
            //        i.DisableValidation();
            //    });

            //    c.Property(u => u.Links, i =>
            //    {
            //        i.DisableValidation();
            //    });

            //    c.Property(u => u.PortLinks, i =>
            //    {
            //        i.DisableValidation();
            //    });

            //    c.Property(u => u.Ports, i =>
            //    {
            //        i.DisableValidation();
            //    });

            //    c.Property(u => u.Position, i =>
            //    {
            //        i.DisableValidation();
            //    });

            //});

            //DynamicTypeDescriptionProvider.Configure<Column>(c =>
            //{

            //    c.Property(c => c.Name, i =>
            //    {
            //        i.PropertyOrder(1)
            //        ;
            //    })
            //    .Property(c => c.Type, i =>
            //    {
            //        i.PropertyOrder(2)
            //        ;
            //    });
            //});

            DynamicTypeDescriptionProvider.Configure<BpmsDiagram>(c =>
            {
                c.Property(u => u.Models, i =>
                {
                    i.DisableBrowsable();
                });

                c.Property(u => u.Relationships, i =>
                {
                    i.DisableBrowsable();
                });

                c.Property(u => u.TypeModelId, i =>
                {
                    i.DisableBrowsable();
                });

            });

        }


    }



}
