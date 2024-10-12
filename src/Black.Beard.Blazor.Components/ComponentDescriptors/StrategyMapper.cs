using Bb.ComponentModel.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bb.ComponentDescriptors
{

    /// <summary>
    /// Referential of strategies for resolve the editor of a property by type
    /// </summary>
    public class StrategyMapper
    {

        #region Ctors

        static StrategyMapper()
        {
            _mappers = new Dictionary<string, StrategyMapper>();
        }

        public StrategyMapper(string key)
        {
            this.Key = key;
            _strategySource = new Dictionary<Type, string>();
            _strategySourceCreators = new Dictionary<Type, Func<object>>();
            _strategyTargets = new Dictionary<string, (Type, Action<StrategyMapper, PropertyObjectDescriptor>)>();
            _strategies = new Dictionary<Type, StrategyEditor>();
            _initializerFromAttributes = new Dictionary<Type, Action<Attribute, StrategyMapper, PropertyObjectDescriptor>>();
            _initializerCustoms = new List<(Func<Type, bool>, Action<Type, StrategyMapper, PropertyObjectDescriptor>)>();
        }

        #endregion Ctors

        #region mappers

        #region sources

        /// <summary>
        /// Define a mapping between the source type and the target strategy to apply
        /// </summary>
        /// <typeparam name="T">source type</typeparam>
        /// <param name="targetStrategyName">Target strategy name to apply</param>
        /// <param name="creator">Delegate to execute. if the creator is null the mapping is removed</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper Source<T>(PropertyKindView targetStrategyName, Func<T>? creator = null)
        {
            return Source<T>(targetStrategyName.ToString(), creator);
        }

        /// <summary>
        /// Define a mapping between the source type and the target strategy to apply
        /// </summary>
        /// <typeparam name="T">type to match for apply the specified strategy</typeparam>
        /// <param name="targetStrategyName">Target strategy name to apply</param>
        /// <param name="creator">Delegate to execute. if the creator is null the mapping is removed</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper Source<T>(string targetStrategyName, Func<T>? creator = null)
        {
            return Source(targetStrategyName, typeof(T), () => creator());
        }

        /// <summary>
        /// Define a mapping between the source type and the target strategy to apply
        /// </summary>
        /// <param name="targetStrategy">Target strategy to apply</param>
        /// <param name="type">type to match for apply the specified strategy</param>
        /// <param name="creator">Delegate to execute. if the creator is null the mapping is removed</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper Source(PropertyKindView targetStrategy, Type type, Func<object>? creator = null)
        {
            return Source(targetStrategy.ToString(), type, creator);
        }

        /// <summary>
        /// Define a mapping between the source type and the target strategy to apply
        /// </summary>
        /// <param name="targetStrategyName">Target strategy name to apply</param>
        /// <param name="type">type to match for apply the specified strategy</param>
        /// <param name="creator">Delegate to execute. if the creator is null the mapping is removed</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper Source(string targetStrategyName, Type type, Func<object>? creator = null)
        {
            if (!_strategySource.ContainsKey(type))
                _strategySource.Add(type, targetStrategyName);
            else
                _strategySource[type] = targetStrategyName;

            if (creator != null)
            {
                if (!_strategySourceCreators.ContainsKey(type))
                    _strategySourceCreators.Add(type, creator);
                else
                    _strategySourceCreators[type] = creator;
            }
            else if (_strategySourceCreators.ContainsKey(type))
                _strategySourceCreators.Remove(type);

            return this;
        }

        #endregion sources

        #region Targets

        /// <summary>
        /// Define a Type do show the editor for the specified strategy
        /// </summary>
        /// <typeparam name="T">Type to use for the editor</typeparam>
        /// <param name="targetStrategyName">Target strategy name to match for select the specified editor type</param>
        /// <param name="initializer">Action to execute for configure the specified editor</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper ToTarget<T>(PropertyKindView targetStrategyName, Action<StrategyMapper, PropertyObjectDescriptor> initializer = null)
        {
            return ToTarget<T>(targetStrategyName.ToString(), initializer);
        }

        /// <summary>
        /// Target strategy name to resolve for select the specified editor type
        /// </summary>
        /// <typeparam name="T">Type to use for the editor</typeparam>
        /// <param name="targetStrategyName">Target strategy name to match for select the specified editor type</param>
        /// <param name="initializer">Action to execute for configure the specified editor</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper ToTarget<T>(string targetStrategyName, Action<StrategyMapper, PropertyObjectDescriptor> initializer = null)
        {
            return ToTarget(targetStrategyName.ToString(), typeof(T), initializer);
        }

        /// <summary>
        /// Define a Type do show the editor for the specified strategy
        /// </summary>
        /// <param name="targetStrategy">Target strategy to match for select the specified editor type</param>
        /// <param name="type">Type to use for the editor</param>
        /// <param name="initializer">Action to execute for configure the specified editor</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper ToTarget(PropertyKindView targetStrategy, Type type, Action<StrategyMapper, PropertyObjectDescriptor> initializer = null)
        {
            return ToTarget(targetStrategy.ToString(), type, initializer);
        }

        /// <summary>
        /// Define a Type do show the editor for the specified strategy
        /// </summary>
        /// <param name="targetStrategyName">Target strategy name to match for select the specified editor type</param>
        /// <param name="type">Type to use for the editor</param>
        /// <param name="initializer">Action to execute for configure the specified editor</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper ToTarget(string targetStrategyName, Type type, Action<StrategyMapper, PropertyObjectDescriptor> initializer = null)
        {
            if (!_strategyTargets.ContainsKey(targetStrategyName))
                _strategyTargets.Add(targetStrategyName, (type, initializer));
            else
                _strategyTargets[targetStrategyName] = (type, initializer);
            return this;
        }

        #endregion Targets

        /// <summary>
        /// Configure the strategy if the property contains the specified attribute
        /// </summary>
        /// <typeparam name="T">Specified attribute</typeparam>
        /// <param name="initializer">Delegate to execute to configure</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper ConfigureOnAttribute<T>(Action<T, StrategyMapper, PropertyObjectDescriptor> initializer)
            where T : Attribute
        {

            if (!_initializerFromAttributes.TryGetValue(typeof(T), out var list))
                _initializerFromAttributes.Add(typeof(T), (d, e, f) => initializer((T)d, e, f));
            else
                _initializerFromAttributes[typeof(T)] = (d, e, f) => initializer((T)d, e, f);
            return this;
        }

        /// <summary>
        /// Configure the strategy if the property contains the attribute
        /// </summary>
        /// <param name="attributeType">Specified attribute</param>
        /// <param name="initializer">Delegate to execute to configure</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper ConfigureOnAttribute(Type attributeType, Action<Attribute, StrategyMapper, PropertyObjectDescriptor> initializer)
        {

            if (!_initializerFromAttributes.TryGetValue(attributeType, out var list))
                _initializerFromAttributes.Add(attributeType, (d, e, f) => initializer(d, e, f));
            else
                _initializerFromAttributes[attributeType] = (d, e, f) => initializer(d, e, f);
            return this;
        }

        /// <summary>
        /// Evaluate the filter and configure the strategy if true
        /// </summary>
        /// <param name="filter">filter to evaluate</param>
        /// <param name="initializer">Delegate to execute to configure</param>
        /// <returns>see <see cref="StrategyMapper"/> for fluent syntax</returns>
        public StrategyMapper ConfigureWhere(Func<Type, bool> filter, Action<Type, StrategyMapper, PropertyObjectDescriptor> initializer)
        {
            _initializerCustoms.Add((filter, initializer));
            return this;
        }

        #endregion mappers

        #region Get

        /// <summary>
        /// Try to get the strategy by type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public bool TryGetValueByType(Type type, out StrategyEditor? strategy)
        {

            if (!_strategies.TryGetValue(type, out strategy))
                lock (_lock)
                    if (!_strategies.TryGetValue(type, out strategy))
                        strategy = Create(type);

            return strategy != null;

        }

        private StrategyEditor? Create(Type type)
        {

            // Try to resolve from registered mapping
            if (TryResolveStrategyName(type, out var strategyName))
                if (_strategyTargets.TryGetValue(strategyName, out var target))
                    if (_strategySourceCreators.TryGetValue(type, out var creator))
                        return new StrategyEditor(strategyName, type, target, creator)
                        {
                            Source = this.Key,
                            AttributeInitializers = _initializerFromAttributes
                        };

            
            // Create by default
            StrategyEditor? strategy = new (this.Key, type, (null, null), null)
            {
                Source = this.Key
            };


            // apply all initializers that match with filter
            foreach (var item in _initializerCustoms)
                if (item.Item1(type))
                    strategy.Initializers.Add(item.Item2);


            return strategy;

        }

        private bool TryResolveStrategyName(Type type, out string? strategyName)
        {

            if (type.IsEnum)
            {
                strategyName = PropertyKindView.Enumeration;
                return true;
            }

            if (_strategySource.TryGetValue(type, out strategyName))
                return true;

            return false;

        }

        #endregion Get

        /// <summary>
        /// Return the global strategy by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Referential of strategies for resolve the editor of a property by type</returns>
        public static StrategyMapper Get(string name)
        {

            if (!_mappers.TryGetValue(name, out var mapper))
                lock (_lock1)
                    if (!_mappers.TryGetValue(name, out mapper))
                        _mappers.Add(name, mapper = Default(name));

            return mapper;

        }

        /// <summary>
        /// Create a default strategy
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StrategyMapper Default(string name)
        {

            var strategy = new StrategyMapper(name)
                .Source(PropertyKindView.String, () => string.Empty)
                .Source<char>(PropertyKindView.Char)
                .Source(PropertyKindView.Bool, () => true)
                .Source<Guid>(PropertyKindView.Guid, () => Guid.Empty)
                //.Source<Guid>(PropertyKindView.String, () => Guid.Empty)
                .Source<Guid?>(PropertyKindView.Bool, () => Guid.Empty)
                .Source<bool?>(PropertyKindView.Bool, () => default)
                .Source<short>(PropertyKindView.Int16, () => 0)
                .Source<short?>(PropertyKindView.Int16, () => 0)
                .Source<int>(PropertyKindView.Int32, () => 0)
                .Source<int?>(PropertyKindView.Int32, () => 0)
                .Source<long>(PropertyKindView.Int64, () => 0)
                .Source<long?>(PropertyKindView.Int64, () => 0)
                .Source<ushort>(PropertyKindView.UInt16, () => 0)
                .Source<ushort?>(PropertyKindView.UInt16, () => 0)
                .Source<uint>(PropertyKindView.UInt32, () => 0)
                .Source<uint?>(PropertyKindView.UInt32, () => 0)
                .Source<ulong>(PropertyKindView.UInt64, () => 0)
                .Source<ulong?>(PropertyKindView.UInt64, () => 0)
                .Source(PropertyKindView.Date, () => DateTime.UtcNow)
                .Source<DateTime?>(PropertyKindView.Date, () => DateTime.UtcNow)
                .Source(PropertyKindView.DateOffset, () => DateTimeOffset.UtcNow)
                .Source<DateTimeOffset?>(PropertyKindView.DateOffset, () => DateTimeOffset.UtcNow)
                .Source(PropertyKindView.Time, () => TimeSpan.FromMinutes(0))
                .Source<TimeSpan?>(PropertyKindView.Time, () => TimeSpan.FromMinutes(0))
                .Source(PropertyKindView.Float, () => 0f)
                .Source<float?>(PropertyKindView.Float, () => 0f)
                .Source(PropertyKindView.Double, () => 0d)
                .Source<double?>(PropertyKindView.Double, () => 0d)
                .Source<decimal>(PropertyKindView.Decimal, () => 0)
                .Source<decimal?>(PropertyKindView.Decimal, () => 0);

            strategy
                .ConfigureOnAttribute<StringMaskAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Mask = attribute.Mask;
                })

                .ConfigureOnAttribute<EditorAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.EditorType = Type.GetType(attribute.EditorTypeName);
                })

                .ConfigureOnAttribute<StepNumericAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Step = attribute.Step;
                })

                .ConfigureOnAttribute<StringLengthAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Maximum = attribute.MaximumLength;
                    descriptor.Minimum = attribute.MinimumLength;
                })

                .ConfigureOnAttribute<MaxLengthAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Maximum = attribute.Length;
                })

                .ConfigureOnAttribute<MinLengthAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Maximum = attribute.Length;
                })

                .ConfigureOnAttribute<RangeAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Minimum = (int)attribute.Minimum;
                    descriptor.Maximum = (int)attribute.Maximum;
                })

                .ConfigureOnAttribute<DisplayFormatAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.FormatString = attribute.DataFormatString;
                    descriptor.HtmlEncode = attribute.HtmlEncode;
                })

                .ConfigureOnAttribute<EditableAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Browsable = attribute.AllowEdit;
                })

                .ConfigureOnAttribute<BrowsableAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Browsable = attribute.Browsable;
                })

                .ConfigureOnAttribute<ReadOnlyAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.ReadOnly = attribute.IsReadOnly;
                })

                .ConfigureOnAttribute<DefaultValueAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.DefaultValue = attribute.Value;
                })

                .ConfigureOnAttribute<RequiredAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.Required = true;
                })

                .ConfigureOnAttribute<DesignerAttribute>((attribute, mapper, descriptor) =>
                {
                    descriptor.EditorType = Type.GetType(attribute.DesignerTypeName);
                });

            if (DefaultInitializerExtension != null)
                DefaultInitializerExtension(strategy);

            return strategy;

        }

        /// <summary>
        /// Default initializer extension
        /// </summary>
        public static Action<StrategyMapper> DefaultInitializerExtension { get; set; }

        /// <summary>
        /// Name of the strategy
        /// </summary>
        public string Key { get; }

        private Dictionary<Type, string> _strategySource;
        private Dictionary<Type, Action<Attribute, StrategyMapper, PropertyObjectDescriptor>> _initializerFromAttributes;
        private List<(Func<Type, bool>, Action<Type, StrategyMapper, PropertyObjectDescriptor>)> _initializerCustoms;
        private Dictionary<Type, Func<object>> _strategySourceCreators;
        private Dictionary<Type, StrategyEditor> _strategies;
        private volatile object _lock = new object();
        private static object _lock1 = new object();
        private Dictionary<string, (Type, Action<StrategyMapper, PropertyObjectDescriptor>)> _strategyTargets;
        private static Dictionary<string, StrategyMapper> _mappers;

    }


}
