using System;
using System.Collections.Generic;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using VSLangProj;

namespace SolutionHelper
{
	/// <summary>
	/// The object for implementing an Add-in.
	/// </summary>
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		private DTE2 m_applicationObject;
		private AddIn m_addInInstance;

		private AdjustCommand m_adjustCommand;

		/// <summary>
		/// Implements the constructor for the Add-in object.
		/// Place your initialization code within this method.
		/// </summary>
		public Connect()
		{
		}

		/// <summary>
		/// Implements the OnConnection method of the IDTExtensibility2 interface.
		/// Receives notification that the Add-in is being loaded.
		/// </summary>
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			m_applicationObject = (DTE2)application;
			m_addInInstance = (AddIn)addInInst;

			m_adjustCommand = new AdjustCommand();

			try
			{
				m_adjustCommand.Initialize(m_applicationObject, m_addInInstance);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}

			/*if (connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object[] contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)m_applicationObject.Commands;
				string toolsMenuName = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:

				//Find the Tools command bar on the MenuBar command bar:
				//CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				//CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//Add a command to the Commands collection:
					Command command = commands.AddNamedCommand2(m_addInInstance, "ReferencePathHelper", "ReferencePathHelper", "Executes the command for ReferencePathHelper", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if ((command != null) && (toolsPopup != null))
					{
						command.AddControl(menuBarCommandBar, 1);
					}
				}
				catch (System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
					//  safely ignore the exception.
				}
			}*/
		}

		/// <summary>
		/// Implements the OnDisconnection method of the IDTExtensibility2 interface.
		/// Receives notification that the Add-in is being unloaded.
		/// </summary>
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>
		/// Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		/// Receives notification when the collection of Add-ins has changed.
		/// </summary>
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>
		/// Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		/// Receives notification that the host application has completed loading.
		/// </summary>
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>
		/// Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		/// Receives notification that the host application is being unloaded.
		/// </summary>
		public void OnBeginShutdown(ref Array custom)
		{
		}

		/// <summary>
		/// Implements the QueryStatus method of the IDTCommandTarget interface.
		/// This is called when the command's availability is updated.
		/// </summary>
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if (commandName == m_adjustCommand.FullCommandName)
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>
		/// Implements the Exec method of the IDTCommandTarget interface.
		/// This is called when the command is invoked.
		/// </summary>
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if (commandName == m_adjustCommand.FullCommandName)
				{
					try
					{
						ProjectAdapter adapter = new ProjectAdapter();
						adapter.Adjust(m_applicationObject);
					}
					catch (Exception e)
					{
						MessageBox.Show(
							e.Message,
							"Solution Helper",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						return;
					}

					MessageBox.Show(
						"Done.",
						"Solution Helper",
						MessageBoxButtons.OK,
						MessageBoxIcon.Information);

					handled = true;
					return;
				}
			}
		}
	}
}