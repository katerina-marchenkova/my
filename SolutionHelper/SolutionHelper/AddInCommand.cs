using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace SolutionHelper
{
	/// <summary>
	/// Add-in command.
	/// </summary>
	public abstract class AddInCommand
	{
		/// <summary>
		/// Gets add-in name.
		/// </summary>
		public string AddInName { get; protected set; }

		/// <summary>
		/// Gets class name.
		/// </summary>
		public string ClassName { get; protected set; }

		/// <summary>
		/// Gets command name.
		/// </summary>
		public string CommandName { get; protected set; }

		/// <summary>
		/// Gets command text.
		/// </summary>
		public string CommandText { get; protected set; }

		/// <summary>
		/// Gets command tooltip.
		/// </summary>
		public string CommandTooltip { get; protected set; }

		/// <summary>
		/// Gets command bar code.
		/// </summary>
		public string CommandBarCode { get; protected set; }

		#region Properties

		/// <summary>
		/// Gets full command name.
		/// </summary>
		public string FullCommandName
		{
			get
			{
				return String.Format("{0}.{1}.{2}", AddInName, ClassName, CommandName);
			}
		}

		#endregion

		#region Command initialization

		/// <summary>
		/// Tries to find current command in specified command collection.
		/// </summary>
		private Command FindCommand(Commands2 commands)
		{
			for (int i = commands.Count; i >= 1; i--)
			{
				Command command = commands.Item(i);
				if (command.Name == FullCommandName)
				{
					return command;
				}
			}

			return null;
		}

		/// <summary>
		/// Adds new command to the specified command collection.
		/// </summary>
		private Command CreateCommand(AddIn addInInstance, Commands2 commands)
		{
			return commands.AddNamedCommand2(
				AddInInstance: addInInstance,
				Name: CommandName,
				ButtonText: CommandText,
				Tooltip: CommandTooltip,
				MSOButton: true,
				Bitmap: 62,
				vsCommandStatusValue: (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled,
				CommandStyleFlags: (int)vsCommandStyle.vsCommandStylePictAndText,
				ControlType: vsCommandControlType.vsCommandControlTypeButton);
		}

		/// <summary>
		/// Tries to find control for current command in specified command bar.
		/// </summary>
		private CommandBarControl FindControl(CommandBar commandBar)
		{
			foreach (CommandBarControl control in commandBar.Controls)
			{
				if (control.accName == CommandText)
					return control;
			}

			return null;
		}

		/// <summary>
		/// Initializes current command in environment.
		/// </summary>
		public void Initialize(DTE2 applicationObject, AddIn addInInstance)
		{
			Commands2 commands = (Commands2)applicationObject.Commands;
			Command command = FindCommand(commands);
			if (command == null)
				command = CreateCommand(addInInstance, commands);

			CommandBars commandBars = (CommandBars)applicationObject.CommandBars;
			CommandBar commandBar = commandBars[CommandBarCode];
			CommandBarControl control = FindControl(commandBar);
			if (control == null)
				command.AddControl(commandBar, 1);
		}

		#endregion
	}
}
