﻿using System.ComponentModel;

namespace Bb.CustomComponents
{
    public class DiagnosticValidatorItem
    {

        public DiagnosticValidatorItem(PropertyDescriptor descriptor)
        {
            this.Descriptor = descriptor;
            this._diagnostics = new List<string>();
        }

        public void Add(string message)
        {
            _diagnostics.Add(message);
        }


        public string Message => String.Concat(_diagnostics.Select(c => ", " + c)).Trim(',', ' ');

        public bool IsValid => _diagnostics.Count == 0;

        public List<string> MessageService => _diagnostics;

        public PropertyDescriptor Descriptor { get; }

        private readonly List<string> _diagnostics;

    }


}
