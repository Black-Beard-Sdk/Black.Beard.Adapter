using Bb.Expressions;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Bb.UserInterfaces
{

    public class ActionReference
    {

        public ActionReference()
        {

        }


        public ActionReference(string href)
        {
            _hRef = href;
        }


        public NavLinkMatch Match { get; set; }

        public virtual string? HRef
        {
            get
            {
                return _hRef;
            }
        }

        public static ActionReference Empty { get; } = new ActionReference(null) { Match = NavLinkMatch.All };

        private string? _hRef;

    }


    public class ActionReference<T> : ActionReference
        where T : ComponentBase
    {

        public ActionReference()
        {
            var attribute = typeof(T).GetCustomAttribute<RouteAttribute>(true);
            _template = attribute.Template;
        }

        public ActionReference<T> MapArgument<U>(Func<U> source, Expression<Func<T, U>> target)
        {
            Action2 action = new Action2<U>(source, target);
            _actions.Add(action);
            return this;

        }

        override public string? HRef
        {
            get
            {
                var t = _template;

                foreach (var item in _actions)
                {
                    t = item.Map(t);
                }

                return t;
            }
        }


        private List<Action2> _actions = new List<Action2>();
        private readonly string _template;


        private abstract class Action2
        {

            public Action2()
            {

            }

            internal abstract string Map(string t);

        }


        private class Action2<U> : Action2
        {

            public Action2(Func<U> source, Expression<Func<T, U>> target)
            {
                this._source = source;
                _name = ExpressionMemberVisitor.GetPropertyName(target);
                _pattern = @"{" + _name + @"(:\w+)?}";
            }

            internal override string Map(string t)
            {
                var u = _source()?.ToString() ?? string.Empty;
                t = Replace(t, u);
                return t;
            }

            private readonly Func<U> _source;
            private readonly string _name;
            private string _pattern = @"{\w+(:\w+)?}";
            private const RegexOptions options = RegexOptions.Multiline;

            private string Replace(string input, string substitution)
            {
                Regex regex = new Regex(_pattern, options);
                string result = regex.Replace(input, substitution);
                return result;
            }


        }

    }


}


