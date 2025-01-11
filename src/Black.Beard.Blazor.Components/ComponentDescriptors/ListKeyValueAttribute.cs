using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Bb.TypeDescriptors;
using System.Text;

namespace Bb.ComponentDescriptors
{

    public class ListKeyLabels
    {


        public ListKeyLabels()
        {
            this._dicKey = new Dictionary<Type, Func<object, object>>();
            this._dicLabel = new Dictionary<Type, Func<object, string>>();
        }


        public bool Add(Type sourceType)
        {

            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (_dicKey.ContainsKey(sourceType))
                return true;

            var typeDescriptors = TypeDescriptor
                .GetAttributes(sourceType)
                .OfType<ListKeyValueAttribute>()
                .ToList();

            if (typeDescriptors.Count > 0)
            {
                foreach (var typeDescriptor in typeDescriptors)
                {
                    _dicKey.Add(typeDescriptor.Type, typeDescriptor.GetKey);
                    _dicLabel.Add(typeDescriptor.Type, typeDescriptor.GetLabel);
                    return true;
                }
            }

            if (!_dicKey.ContainsKey(sourceType))
            {

                var props = TypeDescriptor.GetProperties(sourceType);
                var _propertyValueKeys = props.Where(c => c.ContainsAttribute<KeyAttribute>()).ToList();
                if (_propertyValueKeys.Count == 0)
                {
                    _propertyValueKeys = props.Where(c => c.Name == "Id").ToList();
                    if (_propertyValueKeys.Count == 0)
                        _propertyValueKeys = props.Where(c => c.Name == "Uuid").ToList();
                }

                if (_propertyValueKeys.Count > 1)
                {
                    Func<object, object> key = c => GetKey(_propertyValueKeys, c);
                    _dicKey.Add(sourceType, key);
                    var _propertyValueLabel = props.FirstOrDefault(c => c.ContainsAttribute<ValueLabelAttribute>());
                    _dicLabel.Add(sourceType, (c) => _propertyValueLabel == null ? key(c)?.ToString() : _propertyValueLabel.GetValue(c)?.ToString());
                    return true;
                }

                else if (_propertyValueKeys.Count == 1)
                {
                    var _propertyValueKey = _propertyValueKeys[0];
                    _dicKey.Add(sourceType, _propertyValueKey.GetValue);
                    var _propertyValueLabel = props.FirstOrDefault(c => c.ContainsAttribute<ValueLabelAttribute>()) ?? _propertyValueKey;
                    _dicLabel.Add(sourceType, (c) => _propertyValueLabel.GetValue(c)?.ToString());
                    return true;
                }

                return false;

            }

            return true;

        }

        private static string GetKey(IEnumerable<PropertyDescriptor> properties, object instance)
        {
            StringBuilder sb = new StringBuilder();

            bool t = false;
            foreach (var item in properties)
            {
                var p = item.GetValue(instance);
                if (p != null)
                {
                    if (t)
                        sb.Append(" ");
                    sb.Append(p.ToString());
                    t = true;
                }
            }

            return sb.ToString();

        }

        public bool Add(object instance)
        {

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();

            var typeDescriptors = TypeDescriptor.GetAttributes(instance).OfType<ListKeyValueAttribute>().ToList();

            foreach (var typeDescriptor in typeDescriptors)
            {
                _dicKey.Add(typeDescriptor.Type, typeDescriptor.GetKey);
                _dicLabel.Add(typeDescriptor.Type, typeDescriptor.GetLabel);
                return true;
            }

            return false;

        }

        public bool TryGetKey(object instance, out Func<object, object> result)
        {

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();

            if (_dicKey.TryGetValue(type, out result))
                return true;

            if (Add(type) && _dicKey.TryGetValue(type, out result))
                return true;

            return false;

        }

        public bool TryGetLabel(object instance, out Func<object, string> result)
        {

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();

            if (_dicLabel.TryGetValue(type, out result))
                return true;

            if (Add(type) && _dicLabel.TryGetValue(type, out result))
                return true;

            return false;

        }


        private readonly Dictionary<Type, Func<object, object>> _dicKey;
        private readonly Dictionary<Type, Func<object, string>> _dicLabel;

    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public sealed class ListKeyValueAttribute : Attribute
    {

        public ListKeyValueAttribute(Type type)
        {
            this.Type = type;
        }

        public ListKeyValueAttribute(Type type, string key, string label)
            : this(type)
        {
            this._key = key;
            this._label = key;
        }

        /// <summary>
        /// Return the key of the instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetKey(object instance)
        {

            var p = GetKeyProperty(instance);
            if (p != null)
                return p.GetValue(instance);

            return null;

        }

        /// <summary>
        /// Return the key property of the instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public PropertyDescriptor GetKeyProperty(object instance)
        {

            PropertyDescriptor? p = null;
            var props = TypeDescriptor.GetProperties(instance);

            if (!string.IsNullOrEmpty(_key))
                p = props.FirstOrDefault(c => c.Name == _key);
            else
                p = props.FirstOrDefault(c => c.ContainsAttribute<KeyAttribute>());

            return p;

        }

        /// <summary>
        /// Return the label of the instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public string GetLabel(object instance)
        {

            var p = GetLabelProperty(instance);

            if (p != null)
                return p.GetValue(instance)?.ToString();
            return null;

        }

        /// <summary>
        /// Return the label property of the instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public PropertyDescriptor GetLabelProperty(object instance)
        {

            PropertyDescriptor? p = null;
            var props = TypeDescriptor.GetProperties(instance);

            if (!string.IsNullOrEmpty(_label))
                p = props.FirstOrDefault(c => c.Name == _label);
            else
                p = props.FirstOrDefault(c => c.ContainsAttribute<ValueLabelAttribute>());

            return p;

        }

        private readonly string _key;
        private readonly string _label;

        public Type Type { get; }
    }


}
