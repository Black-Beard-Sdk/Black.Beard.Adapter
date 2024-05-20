using Bb.CustomComponents;
using System;
using System.ComponentModel;

namespace Bb.TypeDescriptors
{


    class DynamicPropertyDescriptor : PropertyDescriptor
    {

        internal DynamicPropertyDescriptor(ConfigurationPropertyDescriptor configuration)
            : base(configuration.Name, configuration.Attributes)
        {
            this._configuration = configuration;
        }

        public override string Category => _configuration.Category ?? base.Category;

        public override string Description => _configuration.Description ?? base.Description;

        public override string DisplayName => _configuration.DisplayName ?? base.DisplayName;

        public override AttributeCollection Attributes => base.Attributes;

        public override TypeConverter Converter => base.Converter;


        public override bool IsBrowsable => base.IsBrowsable;


        public override bool IsLocalizable => base.IsLocalizable;


        public override bool SupportsChangeEvents => base.SupportsChangeEvents;




        public override bool CanResetValue(object component) => this._configuration.CanResetValue;

        public override Type ComponentType => _configuration.ComponentType;

        public override bool IsReadOnly => _configuration.IsReadOnly;

        public override Type PropertyType => _configuration.Type;

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value) => _configuration.SetValue(component, value);

        public override object GetValue(object component) => _configuration.GetValue(component);

        public override bool ShouldSerializeValue(object component) => _configuration.ShouldSerializeValue;


        private readonly ConfigurationPropertyDescriptor _configuration;

    }


}
