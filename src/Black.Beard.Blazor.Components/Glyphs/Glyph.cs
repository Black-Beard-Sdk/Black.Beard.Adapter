
namespace Bb.UIComponents
{

    /// <summary>
    /// Represents a glyph
    /// </summary>
    public struct Glyph
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Glyph"/> class.
        /// </summary>
        /// <param name="value"></param>
        public Glyph(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Glyph value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Default empty glyph
        /// </summary>
        public static Glyph Empty { get; } = new Glyph(null);

        /// <summary>
        /// Indicates if the glyph is empty
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        /// <summary>
        /// implicit conversion from string to Glyph
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Glyph(string value)
        {
            return new Glyph(value);
        }

        /// <summary>
        /// implicit conversion from Glyph to string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator string(Glyph value)
        {
            return value.Value;
        }

    }


}


