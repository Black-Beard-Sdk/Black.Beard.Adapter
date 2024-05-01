
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

    }


}


