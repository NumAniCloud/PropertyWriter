﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PropertyWriter.Models.Exceptions;
using PropertyWriter.Models.Properties;
using PropertyWriter.Models.Properties.Interfaces;

namespace PropertyWriter.Models.Serialize
{
	class ErrorHandlerOnDispose : IDisposable
	{
		private Exception error;
		private IDisposable errorHandlerDisposable;

		public ErrorHandlerOnDispose(IObservable<Exception> errorSequence)
		{
			error = null;
			errorHandlerDisposable = errorSequence.Subscribe(x => error = x);
		}

		public void Dispose()
		{
			errorHandlerDisposable.Dispose();
			if (error != null)
			{
				throw error;
			}
		}
	}

    class ModelConverter
    {
        public async Task LoadValueToRootAsync(PropertyRoot root, object obj)
        {
            var references = new List<(ReferenceByIntProperty model, int id)>();
            foreach (var p in root.Structure.Properties)
            {
                var value = p.PropertyInfo.GetValue(obj);
                LoadValueToModel(p, value, references);
            }
            await Task.Delay(100);
            foreach (var reference in references)
            {
                reference.model.SetItemById(reference.id);
            }
        }

        private void LoadValueToModel(
            IPropertyModel model,
            object value,
            List<(ReferenceByIntProperty reference, int id)> references)
        {
            switch (model)
            {
            case IntProperty m:
                m.IntValue.Value = (int)value;
                break;
            case StringProperty m:
                m.StringValue.Value = (string)value;
                break;
            case BoolProperty m:
                m.BoolValue.Value = (bool)value;
                break;
            case FloatProperty m:
                m.FloatValue.Value = (float)value;
                break;
            case EnumProperty m:
                ConvertEnum(m, value);
                break;
            case ClassProperty m:
                LoadObjectToModel(m, value, references);
                break;
            case StructProperty m:
                LoadObjectToModel(m, value, references);
                break;
            case ReferenceByIntProperty m:
                references.Add((m, (int)value));
                break;
            case SubtypingProperty m:
                LoadSubtypeToModel(m, value, references);
                break;
            case ComplicateCollectionProperty m:
                LoadCollectionToModel(m, value, references);
                break;
            case BasicCollectionProperty m:
                LoadCollectionToModel(m, value, references);
                break;
			case ReferenceByIntCollectionProperty m:
				LoadCollectionToModel(m, value, references);
				break;
            default:
                throw new Exception("開発者向け：不正なModelです。");
            }
        }

        private void LoadSubtypeToModel(
			SubtypingProperty m,
			object value,
			List<(ReferenceByIntProperty reference, int id)> references)
		{
            foreach (var type in m.AvailableTypes)
            {
                if (type.Type == value?.GetType())
				{
					using (var error = new ErrorHandlerOnDispose(m.OnError))
					{
						m.SelectedType.Value = type;
					}
                    LoadObjectToModel((IStructureProperty)m.Model.Value, value, references);
                }
            }
        }

        private void LoadCollectionToModel(
            ICollectionProperty model,
            object value,
            List<(ReferenceByIntProperty reference, int id)> references)
        {
            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
					IPropertyModel element;
					using (var error = new ErrorHandlerOnDispose(model.OnError))
					{
						element = model.AddNewElement();
					}
                    LoadValueToModel(element, item, references);
                }
            }
			else if(value == null)
			{
			}
            else
            {
                throw new ArgumentException("開発者向け：コレクションかどうかの判定が間違っています。", nameof(value));
            }
        }

        private void LoadObjectToModel(
            IStructureProperty model,
            object structureValue,
            List<(ReferenceByIntProperty reference, int id)> references)
        {
			if (structureValue == null)
			{
				return;
			}

            var members = model.Members;
            foreach (var property in members)
            {
                object value = null;

                try
                {
                    value = property.PropertyInfo.GetValue(structureValue);
                }
                catch (ArgumentException)
                {
                    throw new PwObjectMissmatchException(model.ValueType.Name, property.Title.Value);
                }

                LoadValueToModel(property, value, references);
            }
        }

        private void ConvertEnum(EnumProperty model, object obj)
        {
            var val = model.EnumValues.FirstOrDefault(x => x.ToString() == model.ValueType.GetEnumName(obj));
            model.EnumValue.Value = val;
        }
    }
}
