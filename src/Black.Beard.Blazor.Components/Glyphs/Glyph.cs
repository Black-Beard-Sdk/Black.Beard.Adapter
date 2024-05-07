
namespace Bb.UIComponents
{

    public struct Glyph
    {

        public Glyph(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }

        public static Glyph Empty { get; } = new Glyph(null);


        public static implicit operator Glyph(string value)
        {
            return new Glyph(value);
        }

        public static implicit operator string(Glyph value)
        {
            return value.Value;
        }

    }


}


