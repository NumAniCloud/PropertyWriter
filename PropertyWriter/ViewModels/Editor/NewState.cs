﻿using System;
using System.Threading.Tasks;

namespace PropertyWriter.ViewModels.Editor
{
	internal class NewState : EditorState
	{
		public override string Title => " - 新規プロジェクト";
		public override bool CanSave => true;

		public NewState(MainViewModel owner, EditorLifecycleManager manager)
			: base(owner, manager)
		{
			CanClose.Value = false;
		}

		public override Task ModifyAsync()
		{
			return Task.CompletedTask;
		}

		public override async Task NewAsync()
		{
			switch (Manager.ConfirmClose())
			{
			case ClosingViewModel.Result.Anyway:
				break;

			case ClosingViewModel.Result.AfterSave:
				await Manager.SaveFileAsAsync();
				break;

			case ClosingViewModel.Result.Cancel:
			default:
				return;
			}

			var result = Manager.CreateNewProject();
			if (result)
			{
				Owner.State.Value = new NewState(Owner, Manager);
			}
		}

		public override async Task OpenAsync()
		{
			switch (Manager.ConfirmClose())
			{
			case ClosingViewModel.Result.Anyway:
				break;

			case ClosingViewModel.Result.AfterSave:
				await Manager.SaveFileAsAsync();
				break;

			case ClosingViewModel.Result.Cancel:
			default:
				return;
			}

			var path = await Manager.OpenProjectAsync();
			if (path != null)
			{
				Owner.State.Value = new CleanState(Owner, Manager, path);
			}
		}

		public override async Task SaveAsync()
		{
			var path = await Manager.SaveFileAsAsync();
			if (path != null)
			{
				Owner.State.Value = new CleanState(Owner, Manager, path);
			}
		}

		public override async Task SaveAsAsync()
		{
			var path = await Manager.SaveFileAsAsync();
			if (path != null)
			{
				Owner.State.Value = new CleanState(Owner, Manager, path);
			}
		}

		public override async Task CloseAsync()
		{
			switch (Manager.ConfirmClose())
			{
			case ClosingViewModel.Result.Anyway:
				break;

			case ClosingViewModel.Result.AfterSave:
				await Manager.SaveFileAsAsync();
				break;

			case ClosingViewModel.Result.Cancel:
			default:
				return;
			}

			CanClose.Value = true;
			await Manager.CloseAsync();
		}
	}
}