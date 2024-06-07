using RazorEngine.Templating;

namespace Bb.Generators
{
    public class RazorTemplateModule<T> : TemplateBase<T>
    {

        public string Write(params string[] items)
        {

            foreach (var item in items.Where(items => !string.IsNullOrEmpty(items)))
            {
                base.Write(item);
            }

            return string.Empty;
        }

        public string WriteLine(params string[] items)
        {

            foreach (var item in items.Where(items => !string.IsNullOrEmpty(items)))
            {
                base.Write(item);
            }

            base.Write(Environment.NewLine);

            return string.Empty;
        }

        public static string Tab = ('\t').ToString();

    }


}
