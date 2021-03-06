using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;

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

			Messages.AddInName = m_addInInstance.Name;

			m_adjustCommand = new AdjustCommand();

			try
			{
				m_adjustCommand.Initialize(m_applicationObject, m_addInInstance);
			}
			catch (Exception e)
			{
				Messages.ShowError(null, e.ToString());
			}
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
					using (RunForm dialog = new RunForm())
					{
						dialog.ApplicationObject = m_applicationObject;
						dialog.ShowDialog();

						handled = true;
						return;
					}
				}
			}
		}
	}
}