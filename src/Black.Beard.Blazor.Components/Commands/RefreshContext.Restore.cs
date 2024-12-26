using Bb.ComponentModel.Accessors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Channels;
using static MudBlazor.CategoryTypes;

namespace Bb.Commands
{


    /// <summary>
    /// Represents a context for refreshing objects.
    /// </summary>
    public partial class RefreshContext
    {

        public bool ApplyUpdate<T>(T left, T right)
        {

            bool changed = false;

            if (left == null)
                throw new NullReferenceException(nameof(left));

            if (right == null)
                throw new NullReferenceException(nameof(right));

            Type leftType = left.GetType();
            if (leftType != right.GetType())
                throw new InvalidOperationException("are not similar types.");

            if (left is IRestorable rleft)
            {
                if (rleft.Restore(right, this))
                    return true;
                return false;
            }

            if (TryRestoreWithAttribute(leftType, left, right))
            {
                Apply(RefreshStrategy.Updated, left, $"type {typeof(T)} has been changed.");
                return true;
            }

            changed = Restore(left, right);

            if (changed)
                Apply(RefreshStrategy.Updated, left, $"type {typeof(T)} has been changed.");

            return changed;

        }


        public bool Restore(object left, object right)
        {

            bool changed = false;
            var accessors = left.ResolvePropertiesToRestore();

            foreach (var source in accessors)
            {
                var oldValue = source.GetValue(left);
                var newValue = source.GetValue(right);
                if (TryToChange(left, source, oldValue, newValue))
                    changed = true;
            }

            return changed;

        }

        private bool TryToChange(object left, AccessorItem source, object oldValue, object? newValue)
        {

            bool changed = false;

            if (CanCopy(source.Type)) // Staple property
            {
                if (!Compare(oldValue, newValue))
                {
                    source.SetValue(left, newValue);
                    return true;
                }
                return false;
            }

            if (oldValue is IRestorable r)
                return r.Restore(newValue, this);

            else if (TryApplyRestoreUpdateOnMethod(source, newValue, oldValue))
                return true;

            else
            {

            }

            return changed;

        }

        protected virtual bool Compare(object oldValue, object newValue)
        {

            if (oldValue != null)
                return oldValue.Equals(newValue);

            return object.Equals(oldValue, newValue);

        }

        private bool TryApplyRestoreUpdateOnMethod(AccessorItem member, object left, object right)
        {

            var mapper = member.GetAttributes<RestoreAttribute>(true).FirstOrDefault();
            if (mapper != null)
            {

                if (!mapper.TypeCanRestore)
                    throw new InvalidOperationException($"restoring {member.DeclaringType}.{member.Name} failed. The restore {mapper.Type} can'be initialized or is not an implementation of {nameof(IRestorableMapper)}");
                IRestorableMapper restorer = null;
                try
                {
                    restorer = mapper.Get();
                }
                catch (Exception)
                {
                    throw new InvalidOperationException($"the restore type is can' be initialized or is not an implementation of {nameof(IRestorableMapper)}");
                }

                return restorer.Restore(left, right, this);

            }

            return false;
        }

        private bool TryRestoreWithAttribute(Type leftType, object left, object right)
        {

            if (TryGetMapper(leftType, out var mapperInstance))
            {
                mapperInstance.Restore(left, right, this);

                return true;
            }

            var mapper = TypeDescriptor.GetAttributes(leftType).OfType<RestoreAttribute>().FirstOrDefault();
            if (mapper != null)
            {

                if (!mapper.TypeCanRestore)
                    throw new InvalidOperationException($"restoring {leftType} failed. The restore {mapper.Type} can'be initialized or is not an implementation of {nameof(IRestorableMapper)}");
                IRestorableMapper restorer = null;
                try
                {
                    restorer = mapper.Get();
                }
                catch (Exception)
                {
                    throw new InvalidOperationException($"the restore type is can' be initialized or is not an implementation of {nameof(IRestorableMapper)}");
                }

                return restorer.Restore(left, right, this);

            }

            return false;

        }

        private bool TryGetMapper(Type type, out IRestorableMapper mapperInstance)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            mapperInstance = null;

            if (!this._dicMapper.TryGetValue(type, out mapperInstance))
            {

                var mapper = TypeDescriptor.GetAttributes(type).OfType<RestoreAttribute>().FirstOrDefault();
                if (mapper != null)
                {

                    if (!mapper.TypeCanRestore)
                        throw new InvalidOperationException($"restoring {type} failed. The restore {mapper.Type} can'be initialized or is not an implementation of {nameof(IRestorableMapper)}");

                    IRestorableMapper restorer = null;
                    try
                    {
                        restorer = mapper.Get();
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException($"the restore type is can' be initialized or is not an implementation of {nameof(IRestorableMapper)}");
                    }

                    this.Add(type, restorer);

                }
            }

            return mapperInstance != null;

        }

        public void AddTypeToCopy(Type type)
        {
            _types.Add(type);
        }

        private bool CanCopy(Type type)
        {

            if (_types.Contains(type))
                return true;

            if (type.IsEnum)
                return true;

            if (type.IsGenericType)
            {
                var t = type.GetGenericTypeDefinition();
                if (t == typeof(Nullable<>))
                    return CanCopy(type.GetGenericArguments()[0]);
            }



            return false;

        }



        protected void Add(Type key, IRestorableMapper mapper)
        {
            if (!this._dicMapper.TryGetValue(key, out var c))
                this._dicMapper.Add(key, mapper);
        }



        private readonly Dictionary<Type, IRestorableMapper> _dicMapper;
        private HashSet<Type> _types = new HashSet<Type>()
        {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(byte),
            typeof(decimal),
            typeof(float),
            typeof(double),
            typeof(bool),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(char),
        };


    }


}
