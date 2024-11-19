using Bb.Addons;

namespace Bb.Modules
{

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

        public virtual void Initialize(Feature feature, Document document)
        {

            _feature = feature;
            SetSave(model =>
            {
                _feature.Save(document, this);
                document.Parent.Save(document);
            });

            SetMemorize(_feature.Memorize);

        }

        private Feature _feature;

    }



}
