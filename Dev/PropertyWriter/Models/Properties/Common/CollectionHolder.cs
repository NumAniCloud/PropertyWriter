﻿using System;
using System.Collections;
using System.Collections.ObjectModel;
using PropertyWriter.Models.Properties.Interfaces;
using Reactive.Bindings;
using System.Reactive.Subjects;
using System.Collections.Generic;

namespace PropertyWriter.Models.Properties.Common
{
	public class CollectionHolder
	{
		public static readonly string ElementTitle = "Element";

		private readonly PropertyFactory modelFactory_;

		private Subject<Exception> OnErrorSubject { get; } = new Subject<Exception>();

		public Type Type { get; }
		public Type ItemType { get; }
		public ObservableCollection<(IPropertyModel model, IDisposable error)> Collection { get; }
		public ReactiveProperty<IEnumerable> Value { get; }
		public IObservable<Exception> OnError => OnErrorSubject;
		public ReactiveProperty<int> Count { get; } = new ReactiveProperty<int>();

		public CollectionHolder(Type type, PropertyFactory modelFactory)
		{
			Type = type;
			modelFactory_ = modelFactory;

			if (type.Name == "IEnumerable`1")
			{
				ItemType = type.GenericTypeArguments[0];
			}
			else if(type.Name == "Dictionary`2")
			{
				ItemType = typeof(KeyValuePair<,>).MakeGenericType(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
			}
			else if (type.IsArray)
			{
				ItemType = type.GetElementType();
			}
			else
			{
				throw new ArgumentException("配列または IEnumerable<T> を指定する必要があります。", nameof(type));
			}

			Value = new ReactiveProperty<IEnumerable>(Array.CreateInstance(ItemType, 0));

			Collection = new ObservableCollection<(IPropertyModel model, IDisposable error)>();
			Collection.ToCollectionChanged()
				.Subscribe(x =>
				{
					Count.Value = Collection.Count;
					try
					{
						Value.Value = MakeValue(Collection);
					}
					catch (Exception e)
					{
						OnErrorSubject.OnNext(e);
						throw;
					}
				});
		}

		private IEnumerable MakeValue(ObservableCollection<(IPropertyModel model, IDisposable error)> collection)
		{
			var array = Array.CreateInstance(ItemType, collection.Count);
			for (var i = 0; i < collection.Count; i++)
			{
				array.SetValue(collection[i].model.Value.Value, i);
			}
			return array;
		}

		public IPropertyModel AddNewElement()
		{
			var instance = modelFactory_.Create(ItemType, ElementTitle);
			var errorDisposable = instance.OnError.Subscribe(x => OnErrorSubject.OnNext(x));
			Collection.Add((instance, errorDisposable));
			instance.Value.Subscribe(x => Value.Value = MakeValue(Collection));
			return instance;
		}

		public void AddElement(IPropertyModel model)
		{
			var errorDisposable = model.OnError.Subscribe(x => OnErrorSubject.OnNext(x));
			Collection.Add((model, errorDisposable));
			model.Value.Subscribe(x => Value.Value = MakeValue(Collection));
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Collection.Count)
			{
				return;
			}
			Collection[index].error.Dispose();
			Collection.RemoveAt(index);
		}
		
		public void Move(int oldIndex, int newIndex)
		{
			if (oldIndex < 0 || oldIndex >= Collection.Count)
			{
				return;
			}
			if (newIndex < 0 || newIndex >= Collection.Count)
			{
				return;
			}
			Collection.Move(oldIndex, newIndex);
		}

		public void Duplicate(int source, int destination)
		{
			var sourceProp = Collection[source].model;
			var sourceType = sourceProp.ValueType;

			var clone = modelFactory_.Create(sourceType, sourceProp.Title.Value);
			clone.CopyFrom(sourceProp);
			var disposable = clone.OnError.Subscribe(x => OnErrorSubject.OnNext(x));

			Collection.Insert(destination, (clone, disposable));
		}
	}
}