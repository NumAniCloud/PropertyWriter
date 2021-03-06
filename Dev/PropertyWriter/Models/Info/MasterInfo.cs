﻿using System.Reflection;
using PropertyWriter.Models.Properties.Interfaces;

namespace PropertyWriter.Models.Info
{
    public class MasterInfo
	{
		public string Key { get; }
		public PropertyInfo Property { get; }
		public IPropertyModel Master { get; }

		public MasterInfo(string key, PropertyInfo property, IPropertyModel master)
		{
			Property = property;
			Master = master;
			Key = key ?? Property.Name;
		}
	}
}
