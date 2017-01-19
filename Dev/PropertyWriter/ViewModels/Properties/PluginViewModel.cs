﻿using PropertyWriter.ViewModels.Properties.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Reactive;
using PropertyWriter.Models.Properties.Interfaces;
using PropertyWriter.Models.Properties.Common;
using System.Windows.Controls;

namespace PropertyWriter.ViewModels.Properties
{
	public abstract class PluginViewModel : PropertyViewModel<IPropertyModel>
	{
		protected PropertyRouter Router { get; }
		public abstract UserControl UserControl { get; }

		public PluginViewModel(IPropertyModel model, ViewModelFactory factory) : base(model)
		{
			Router = new PropertyRouter(factory);
		}
	}
}
