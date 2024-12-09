using Bb.ComponentModel.Translations;
using Bb.UIComponents;

namespace Bb.Diagrams
{

    


    public class DiagramToolBase
    {

        public DiagramToolBase
        (
            Guid? uuid,
            TranslatedKeyLabel category,
            TranslatedKeyLabel name,
            TranslatedKeyLabel description,
            string icon)
        {

            if (uuid == null)
                throw new NullReferenceException($"{nameof(uuid)} is required");
            this.Uuid = uuid.Value;

            if (name == null)
                throw new NullReferenceException($"{nameof(name)} is required");
            this.Name = name;

            if (category == null)
                throw new NullReferenceException($"{nameof(category)} is required");
            Category = category;

            if (description == null)
                throw new NullReferenceException($"{nameof(description)} is required");
            this.ToolTip = description;

            _defaultName = name.DefaultDisplay.Replace(" ", "_");

            Icon = icon;

        }

        /// <summary>
        /// Hide in the toolbox
        /// </summary>
        public bool Hidden { get; set; }


        public virtual string GetDefaultName()
        {
            return _defaultName;
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Uuid { get; }

        public virtual void WithModel<T>(Action<Initializer<T>> initializer = null)
            where T : UIModel
        {            
            TypeModel = typeof(T);

            var i = new Initializer<T>()
            {

            };

            if (initializer != null)
                initializer(i);

            this._Initializer = i;

        }

        protected void Initialize(UIModel model)
        {
            if (_Initializer != null)
                _Initializer.ExecuteInitialize(model);
        }
    
        protected void InitializeAfterAdded(UIModel model)
        {
            if (_Initializer != null)
                _Initializer.ExecuteInitializeAfterAdded(model);
        }

        private Initializer _Initializer;

        public Type TypeModel { get; private set; } = typeof(UIModel);

        internal protected virtual void SetTypeUI<T>()
            where T : Microsoft.AspNetCore.Components.IComponent
        {
            TypeUI = typeof(T);
        }

        public static Glyph Evaluate(Glyph left, Glyph right)
        {
            if (left.IsEmpty)
                return right;
            return left;
        }

        public Type TypeUI { get; private set; }

        public TranslatedKeyLabel Category { get; }

        /// <summary>
        /// Name of the entity tool
        /// </summary>
        public TranslatedKeyLabel Name { get; }

        /// <summary>
        /// Description of the entity tool
        /// </summary>
        public TranslatedKeyLabel ToolTip { get; }

        private readonly string _defaultName;

        /// <summary>
        /// Icon of the entity tool in the toolbox
        /// </summary>
        public string Icon { get; }

        public ToolKind Kind { get; protected set; }

    }

    public enum ToolKind
    {
        Node,
        Link,
        Group,

    }

}
