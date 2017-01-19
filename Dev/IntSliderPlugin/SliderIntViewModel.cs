﻿using PropertyWriter.ViewModels.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyWriter.Models.Properties.Interfaces;
using System.Windows.Controls;
using System.Reactive.Linq;
using System.Reactive;
using Reactive.Bindings;
using System.ComponentModel.Composition;
using PropertyWriter.ViewModels.Properties.Common;
using PropertyWriter.ViewModels.Properties.Extensibility;
using PropertyWriter.ViewModels;
using PropertyWriter.Models.Properties;

namespace IntSliderPlugin
{
	//[Export(typeof(IPluginViewModelFactory))]
	public class SliderIntPlugin : IPluginViewModelFactory
	{
		public Type EntityType => typeof(int);
		public PluginViewModel CreateViewModel(IPropertyModel model, ViewModelFactory factory) =>
			new SliderIntViewModel(model, factory);
	}

	public class SliderIntViewModel : PluginViewModel
	{
		public override UserControl UserControl => new IntSlider(this);
		public ReactiveProperty<int> IntValue { get; set; }

		public SliderIntViewModel(IPropertyModel model, ViewModelFactory factory)
			: base(model, factory)
		{
			IntValue = Compounder.CreateIntViewModel("").IntValue;
		}
	}
}