using System;
using System.Collections.Generic;
using System.Reflection;

namespace Black.Beard.Configuration.Git
{


    public static class EnvironmentMapper
    {

        static EnvironmentMapper()
        {

            _args = Parse(Environment.GetCommandLineArgs());

        }


        public static T MapFromEnvironment<T>(this T instance)
        {

            var collection = instance.GetType().GetProperties();

            foreach (var item in collection)
                if (item.ResolveVariableName(out string variableName))
                    if (variableName.ResolveVariable(out string variableValue))
                    {
                        var value = Convert.ChangeType(variableValue, item.PropertyType);
                        item.SetValue(instance, value);
                    }

            return instance;

        }


        private static bool ResolveVariableName(this System.Reflection.PropertyInfo property, out string name)
        {

            name = property.Name;

            var attribute = property.GetCustomAttribute<EnvironmentMapAttribute>();
            if (attribute != null)
            {

                if (!attribute.Map)
                    return false;

                if (!string.IsNullOrEmpty(attribute.VariableName))
                    name = attribute.VariableName;

            }

            return true;
        }

        private static bool ResolveVariable(this string name, out string result)
        {

            if (_args.TryGetValue(name, out result))
                return true;
            
            result = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            var r = !string.IsNullOrEmpty(result);
            var r2 = r ? "Successed" : "Failed";
            Console.WriteLine($"Resolving {name} from configuration : {r2}");
            return r;
        }

        private static Dictionary<string, string> Parse(params string[] parts)
        {
            var variables = new Dictionary<string, string>();
            for (int i = 0; i < parts.Length; i++)
            {
                string key = null;
                string value = null;

                // Format --key=value
                if (parts[i].StartsWith("--"))
                {
                    var keyValue = parts[i].Substring(2).Split(new[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        key = keyValue[0];
                        value = keyValue[1];
                    }
                }
                // Format -key value
                else if (parts[i].StartsWith("-") && i + 1 < parts.Length && !parts[i + 1].StartsWith("-"))
                {
                    key = parts[i].Substring(1);
                    value = parts[++i]; // Incrémente i pour sauter la valeur déjà lue
                }

                if (key != null && value != null && !variables.ContainsKey(key))
                    variables.Add(key, value);

            }

            return variables;
        }

        private static readonly Dictionary<string, string> _args;

    }

}
