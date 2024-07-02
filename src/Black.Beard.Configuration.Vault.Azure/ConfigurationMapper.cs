using Microsoft.Extensions.Configuration;

namespace Bb.Configuration.Vault.Azure
{
    internal static class ConfigurationMapper
    {

        static ConfigurationMapper()
        {

        }

        public static T MapFromConfiguration<T>(this T instance, IConfigurationRoot configuration)
        {

            var collection = instance.GetType().GetProperties();

            foreach (var item in collection)
                if (item.ResolveVariableName(out string variableName))
                    if (variableName.ResolveVariable(configuration, out string variableValue))

                    {
                        var value = Convert.ChangeType(variableValue, item.PropertyType);
                        item.SetValue(instance, value);
                    }

            return instance;
        }


        private static bool ResolveVariableName(this System.Reflection.PropertyInfo property, out string name)
        {

            name = property.Name;

            //var attribute = property.GetCustomAttribute<EnvironmentMapAttribute>();
            //if (attribute != null)
            //{

            //    if (!attribute.Map)
            //        return false;

            //    if (!string.IsNullOrEmpty(attribute.VariableName))
            //        name = attribute.VariableName;

            //}

            return true;
        }


        private static bool ResolveVariable(this string name, IConfigurationRoot configuration, out string result)
        {
            result = configuration[name];

            var r = !string.IsNullOrEmpty(result);
            var r2 = r ? "Successed" : "Failed";
            Console.WriteLine($"Resolving {name} from configuration : {r2}");
            return r;
        }


    }


}
