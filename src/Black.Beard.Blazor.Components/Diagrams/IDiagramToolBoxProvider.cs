namespace Bb.Diagrams
{

    public interface IDiagramToolBoxProvider
    {
        
        /// <summary>
        /// Initialize the toolbox
        /// </summary>
        /// <param name="toolbox"></param>
        void InitializeToolbox(DiagramToolbox toolbox);

        /// <summary>
        /// Return true if the provider is dynamic
        /// </summary>
        bool DynamicToolbox { get; }

    }

}
