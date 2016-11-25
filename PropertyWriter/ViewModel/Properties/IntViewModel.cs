﻿using System.Reactive.Linq;
using Reactive.Bindings;
using PropertyWriter.Model.Properties;

namespace PropertyWriter.Model.Instance
{
	class IntViewModel : PropertyViewModel
	{
        private IntProperty Property { get; }

		public ReactiveProperty<int> IntValue => Property.IntValue;
		public override ReactiveProperty<object> Value => Property.Value;

        public IntViewModel(IntProperty property)
        {
            Property = property;
        }
	}
}
