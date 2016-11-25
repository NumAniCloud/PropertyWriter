﻿using PropertyWriter.Model.Interfaces;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyWriter.Model.Properties
{
    class FloatProperty : PropertyModel
    {
        public ReactiveProperty<float> FloatValue { get; } = new ReactiveProperty<float>();
        public override ReactiveProperty<object> Value { get; }

        public FloatProperty()
        {
            Value = FloatValue.Select(x => (object)x).ToReactiveProperty();
        }
    }
}
