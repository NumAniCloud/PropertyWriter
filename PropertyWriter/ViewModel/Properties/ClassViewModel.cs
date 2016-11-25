﻿using System;
using System.Linq;
using System.Reactive.Linq;
using Livet.Messaging;
using PropertyWriter.ViewModel;
using Reactive.Bindings;
using PropertyWriter.Model.Properties;

namespace PropertyWriter.Model.Instance
{
    class ClassViewModel : PropertyViewModel
	{
        private ClassProperty Property { get; }

        public IPropertyModel[] Members => Property.Members;
        public override ReactiveProperty<object> Value => Property.Value;
        public ReactiveCommand<PropertyViewModel> EditCommand { get; } = new ReactiveCommand<PropertyViewModel>();

        public override ReactiveProperty<string> FormatedString
        {
            get
            {
                var events = Property.Members
                    .Select(x => x.Value)
                    .Cast<IObservable<object>>()
                    .ToArray();
                return Observable.Merge(events)
                    .Select(x => Value.Value.ToString())
                    .ToReactiveProperty();
            }
        }

        public ClassViewModel(ClassProperty property)
        {
            Property = property;
            EditCommand.Subscribe(x => Messenger.Raise(
                new TransitionMessage(
                    new BlockViewModel(Property),
                    "BlockWindow")));
        }
	}
}
