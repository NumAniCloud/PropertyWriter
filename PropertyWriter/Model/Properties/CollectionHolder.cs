﻿using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace PropertyWriter.Model.Properties
{
    internal class CollectionHolder
    {
        private readonly ModelFactory modelFactory_;
        public Type Type { get; }
        public Type ItemType { get; }
        public ObservableCollection<Instance.IPropertyViewModel> Collection { get; }
        public ReactiveProperty<IEnumerable> Value { get; }

        public CollectionHolder(Type type, ModelFactory modelFactory)
        {
            Type = type;
            modelFactory_ = modelFactory;

            if (type.Name == "IEnumerable`1")
            {
                ItemType = type.GenericTypeArguments[0];
            }
            else if (type.IsArray)
            {
                ItemType = type.GetElementType();
            }
            else
            {
                throw new ArgumentException("配列または IEnumerable<T> を指定する必要があります。", nameof(type));
            }

            Value = new ReactiveProperty<IEnumerable>();
            Value.Value = Array.CreateInstance(ItemType, 0);

            Collection = new ObservableCollection<Instance.IPropertyViewModel>();
            Collection.ToCollectionChanged()
                .Subscribe(x => Value.Value = MakeValue(Collection));
        }

        private IEnumerable MakeValue(ObservableCollection<Instance.IPropertyViewModel> collection)
        {
            var array = Array.CreateInstance(ItemType, collection.Count);
            for (var i = 0; i < collection.Count; i++)
            {
                array.SetValue(collection[i].Value.Value, i);
            }
            return array;
        }

        public Instance.IPropertyViewModel AddNewElement()
        {
            var instance = modelFactory_.Create(ItemType, "Element");
            Collection.Add(instance);
            instance.Value.Subscribe(x => Value.Value = MakeValue(Collection));
            return instance;
        }

        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
        }
    }
}