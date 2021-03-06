﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PropertyWriter
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		public static readonly string PluginDirectory = "../plugins";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			Livet.DispatcherHelper.UIDispatcher = this.Dispatcher;
		}
	}
}
