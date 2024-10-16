﻿//using System.Text.Json.Serialization;
//using System.Text.Json;
//using System.Reflection;
//using Bb.Diagrams;

//namespace Bb.TypeDescriptors
//{

//    public partial class InstanceJsonConverter : JsonConverterFactory
//    {

//        public override bool CanConvert(Type typeToConvert)
//        {
//            if (typeToConvert == typeof(Diagram))
//                return false;
//            return typeof(IDynamicDescriptorInstance).IsAssignableFrom(typeToConvert);
//        }

//        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
//        {

//            JsonConverter converter = null;

//            var ctor = type.GetConstructor(new Type[0]);
//            if (ctor != null)
//            {

//                converter = (JsonConverter)Activator.CreateInstance
//                (
//                    typeof(CustomSubPropertyJsonConverter<>).MakeGenericType(type),
//                    BindingFlags.Instance | BindingFlags.Public,
//                    binder: null,
//                    args: null,
//                    culture: null
//                )!;

//            }
//            else
//            {

//                converter = (JsonConverter)Activator.CreateInstance
//                (
//                    typeof(CustomSubPropertyJsonConverter<>).MakeGenericType(type),
//                    BindingFlags.Instance | BindingFlags.Public,
//                    binder: null,
//                    args: null,
//                    culture: null
//                )!;

//            }

//            return converter;
//        }

//    }

//}
