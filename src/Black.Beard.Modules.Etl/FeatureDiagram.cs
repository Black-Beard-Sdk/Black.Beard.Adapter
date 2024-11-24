using Bb.Addons;

namespace Bb.Modules
{

    /// <summary>
    /// Generic feature for future diagram
    /// </summary>
    public class FeatureDiagram : Diagrams.Diagram, IFeatureInitializer
    {

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dynamicToolBox"></param>
        public FeatureDiagram(Guid key, bool dynamicToolBox)
            : base(key, dynamicToolBox)
        {

        }

        /// <summary>
        /// Initialize feature diagram
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="document"></param>
        public virtual void Initialize(Feature feature, Document document)
        {

            _feature = feature;
            SetSave(model =>
            {
                _feature.Save(document, this);
                document.Parent.Save(document);
            });

            SetMemorize(_feature.Memorize, _feature.Restore);

        }

        private Feature _feature;

    }



}
