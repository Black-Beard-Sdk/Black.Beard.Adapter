using System.Runtime.InteropServices;

namespace Bb.ComponentDescriptors
{

    /// <summary>
    /// Name of the view strategy to apply for a property
    /// </summary>
    public class PropertyKindView
    {

        /// <summary>
        /// Initializes a new instance of the PropertyKindView class.
        /// </summary>
        /// <param name="type"></param>
        public PropertyKindView(Type type) 
            : this(type.Name)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the PropertyKindView class.
        /// </summary>
        /// <param name="name"></param>
        public PropertyKindView(string name)
        {
            this.Name = name;
        }

        public static readonly PropertyKindView String = new PropertyKindView(typeof(String));
        public static readonly PropertyKindView Char = new PropertyKindView(typeof(Char));
        public static readonly PropertyKindView Bool = new PropertyKindView(typeof(Boolean));
        public static readonly PropertyKindView Int16 = new PropertyKindView(typeof(Int16));
        public static readonly PropertyKindView Int32 = new PropertyKindView(typeof(Int32));
        public static readonly PropertyKindView Int64 = new PropertyKindView(typeof(Int64));
        public static readonly PropertyKindView UInt16 = new PropertyKindView(typeof(UInt16));
        public static readonly PropertyKindView UInt32 = new PropertyKindView(typeof(UInt32));
        public static readonly PropertyKindView UInt64 = new PropertyKindView(typeof(UInt64));
        public static readonly PropertyKindView Date = new PropertyKindView("Date");
        public static readonly PropertyKindView DateOffset = new PropertyKindView(typeof(DateTimeOffset));
        public static readonly PropertyKindView Time = new PropertyKindView(typeof(TimeSpan));
        public static readonly PropertyKindView Float = new PropertyKindView(typeof(Single));
        public static readonly PropertyKindView Double = new PropertyKindView(typeof(Double));
        public static readonly PropertyKindView Decimal = new PropertyKindView(typeof(Decimal));
        public static readonly PropertyKindView Guid = new PropertyKindView(typeof(Guid));
        public static readonly PropertyKindView Object = new PropertyKindView(typeof(Object));
        public static readonly PropertyKindView List = new PropertyKindView("List");
        public static readonly PropertyKindView Enumeration = new PropertyKindView("Enumeration");
        public static readonly PropertyKindView Password = new PropertyKindView("Password");

        /// <summary>
        /// Returns the name of the PropertyKindView
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// ToString is intended to provide a textual representation of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name.ToString();
        }

        /// <summary>
        /// GetHashCode is intended to serve as a hash function for this object.
        /// Based on the contents of the object, the hash function will return a suitable
        /// value with a relatively random distribution over the various inputs.
        /// The default implementation returns the sync block index for this instance.
        /// Calling it on the same object multiple times will return the same value, so
        /// it will technically meet the needs of a hash function, but it's less than ideal.
        /// Objects (& especially value classes) should override this method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// Determines whether two strings match
        /// </summary>
        /// <param name="obj">instance to evaluate</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is PropertyKindView)
                return Name.Equals(((PropertyKindView)obj).Name);
        
            return false;

        }

        /// <summary>
        /// Evaluates whether two PropertyKindView instances are equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(PropertyKindView left, PropertyKindView right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Evaluates whether two PropertyKindView instances are not equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(PropertyKindView left, PropertyKindView right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// return the name of the property kind view
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator string(PropertyKindView value)
        {
            return value.Name;
        }

    }

}
