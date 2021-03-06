﻿using System.Reflection;
using Reactive.Bindings;
using System;

namespace PropertyWriter.Models.Properties.Interfaces
{
    public interface IPropertyModel
    {
        ReactiveProperty<object> Value { get; }
		Type ValueType { get; }
        ReactiveProperty<string> Title { get; }
        PropertyInfo PropertyInfo { get; set; }
		IObservable<Exception> OnError { get; }
		void CopyFrom(IPropertyModel sourceProp);
	}
}
