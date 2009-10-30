using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Shuruev.Releaser.Interfaces;
using ParamValue = System.Collections.Generic.KeyValuePair<string, object>;

namespace Shuruev.Releaser.Data
{
	/// <summary>
	/// Accessing Releaser data using SQLite engine.
	/// </summary>
	public class ReleaserDataSQLite : IReleaserData
	{
		private string m_databaseFile;
		private string m_connectionString;

		private TimeSpan m_commandTimeout = TimeSpan.FromSeconds(30);
		private int m_commandTimeoutInSeconds = 30;

		#region Initialization

		/// <summary>
		/// Initialize data engine.
		/// </summary>
		public void Initialize(NameValueCollection config)
		{
			m_databaseFile = ConfigurationHelper.ReadString(config, "ReleaserData.DatabaseFile");

			SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
			sb.DataSource = m_databaseFile;
			m_connectionString = sb.ConnectionString;

			if (ConfigurationHelper.HasValue(config, "ReleaserData.Timeout"))
			{
				m_commandTimeout = ConfigurationHelper.ReadTimeSpan(config, "ReleaserData.Timeout");
				m_commandTimeoutInSeconds = Convert.ToInt32(m_commandTimeout.TotalSeconds);
			}

			if (!File.Exists(m_databaseFile))
			{
				SQLiteConnection.CreateFile(m_databaseFile);
				CreateDatabase();
			}
		}

		/// <summary>
		/// Creates new database.
		/// </summary>
		private void CreateDatabase()
		{
			ExecuteNonQuery(
				@"
					CREATE TABLE [User] (
						[UserId] INTEGER PRIMARY KEY,
						[UserName] TEXT NOT NULL,
						[ImageCode] TEXT NOT NULL,
						[Login] TEXT NOT NULL);
				");

			ExecuteNonQuery(
				@"
					CREATE TABLE [Project] (
						[ProjectId] INTEGER PRIMARY KEY,
						[ProjectName] TEXT NOT NULL,
						[ImageCode] TEXT NOT NULL,
						[StorageCode] TEXT NOT NULL,
						[StoragePath] TEXT NOT NULL);
				");

			ExecuteNonQuery(
				@"
					CREATE TABLE [Property] (
						[PropertyCode] TEXT PRIMARY KEY,
						[PropertyName] TEXT NOT NULL);
				");

			ExecuteNonQuery(
				@"
					CREATE TABLE [PropertyUsage] (
						[PropertyCode] TEXT NOT NULL,
						[EntityType] TEXT NOT NULL,
						PRIMARY KEY (
								[PropertyCode],
								[EntityType]));
				");

			ExecuteNonQuery(
				@"
					CREATE TABLE [EntityProperty] (
						[EntityType] TEXT NOT NULL,
						[EntityId] INTEGER NOT NULL,
						[PropertyCode] TEXT NOT NULL,
						[PropertyValue] TEXT NOT NULL);

					CREATE INDEX [IX_EntityProperty_Entity] ON [EntityProperty] (
						[EntityType] ASC,
						[EntityId] ASC);
				");
		}

		#endregion

		#region Project management

		/// <summary>
		/// Adds project.
		/// </summary>
		public int AddProject(ProjectData data)
		{
			object id = ExecuteScalar(
				@"
					INSERT INTO [Project] (
						[ProjectId],
						[ProjectName],
						[ImageCode],
						[StorageCode],
						[StoragePath])
					VALUES (
						NULL,
						@projectName,
						@imageCode,
						@storageCode,
						@storagePath);

					SELECT last_insert_rowid() FROM [Project];
				",
				new ParamValue("@projectName", data.ProjectName),
				new ParamValue("@imageCode", data.ImageCode),
				new ParamValue("@storageCode", data.StorageCode),
				new ParamValue("@storagePath", data.StoragePath));

			return Convert.ToInt32(id);
		}

		/// <summary>
		/// Update project.
		/// </summary>
		public void UpdateProject(int projectId, ProjectData data)
		{
			ExecuteNonQuery(
				@"
					UPDATE [Project]
					SET
						[ProjectName] = @projectName,
						[ImageCode] = @imageCode,
						[StorageCode] = @storageCode,
						[StoragePath] = @storagePath
					WHERE [ProjectId] = @projectId;
				",
				new ParamValue("@projectId", projectId),
				new ParamValue("@projectName", data.ProjectName),
				new ParamValue("@imageCode", data.ImageCode),
				new ParamValue("@storageCode", data.StorageCode),
				new ParamValue("@storagePath", data.StoragePath));
		}

		/// <summary>
		/// Removes project.
		/// </summary>
		public void RemoveProject(int projectId)
		{
			ExecuteNonQuery(
				@"
					DELETE FROM [Project]
					WHERE [ProjectId] = @projectId;
				",
				new ParamValue("@projectId", projectId));
		}

		/// <summary>
		/// Gets project.
		/// </summary>
		public ProjectRow GetProject(int projectId)
		{
			return ExecuteReader(
				@"
					SELECT
						[ProjectId],
						[ProjectName],
						[ImageCode],
						[StorageCode],
						[StoragePath]
					FROM [Project]
					WHERE [ProjectId] = @projectId;
				",
				reader => new ProjectRowSQLite(reader),
				new ParamValue("@projectId", projectId));
		}

		#endregion

		#region Property management

		/// <summary>
		/// Sets all properties for specified entity.
		/// </summary>
		public void SetProperties(
			EntityType entityType,
			int entityId,
			PropertyValues properties)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("BEGIN TRANSACTION;");

			sb.Append(
				@"
					DELETE FROM [EntityProperty]
					WHERE
						[EntityType] = @entityType
						AND [EntityId] = @entityId;
				");

			List<ParamValue> parameters = new List<ParamValue>();
			parameters.Add(new ParamValue("@entityType", entityType));
			parameters.Add(new ParamValue("@entityId", entityId));

			int count = 0;
			foreach (string code in properties.GetCodes())
			{
				foreach (string value in properties.GetValues(code))
				{
					count++;
					string codeKey = String.Format("@code{0}", count);
					string valueKey = String.Format("@value{0}", count);

					sb.AppendFormat(
						@"
							INSERT INTO [EntityProperty] (
								[EntityType],
								[EntityId],
								[PropertyCode],
								[PropertyValue])
							VALUES (
								@entityType,
								@entityId,
								{0},
								{1});
						",
						codeKey,
						valueKey);

					parameters.Add(new ParamValue(codeKey, code));
					parameters.Add(new ParamValue(valueKey, value));
				}
			}

			sb.AppendLine();
			sb.AppendLine("COMMIT TRANSACTION;");

			ExecuteNonQuery(
				sb.ToString(),
				parameters.ToArray());
		}

		/// <summary>
		/// Gets all properties for specified entity.
		/// </summary>
		public PropertyValues GetProperties(
			EntityType entityType,
			int entityId)
		{
			PropertyValues properties = new PropertyValues();

			ExecuteReader(
				@"
					SELECT
						[PropertyCode],
						[PropertyValue]
					FROM [EntityProperty]
					WHERE
						[EntityType] = @entityType
						AND [EntityId] = @entityId
					ORDER BY
						[PropertyCode],
						[PropertyValue];
				",
				delegate(IDataRecord reader)
				{
					properties.Add(
						Read.String(reader, "PropertyCode"),
						Read.String(reader, "PropertyValue"));
				},
				new ParamValue("@entityType", entityType),
				new ParamValue("@entityId", entityId));

			return properties;
		}

		#endregion

		#region Service methods

		/// <summary>
		/// Creates and opens database connection.
		/// </summary>
		private SQLiteConnection OpenConnection()
		{
			SQLiteConnection conn = new SQLiteConnection(m_connectionString);
			conn.Open();
			return conn;
		}

		/// <summary>
		/// Creates database command.
		/// </summary>
		private SQLiteCommand CreateCommand(SQLiteConnection conn)
		{
			SQLiteCommand cmd = conn.CreateCommand();
			cmd.CommandTimeout = m_commandTimeoutInSeconds;
			return cmd;
		}

		/// <summary>
		/// Executes non-query command.
		/// </summary>
		private void ExecuteNonQuery(
			string commandText,
			params ParamValue[] parameters)
		{
			using (SQLiteConnection conn = OpenConnection())
			{
				using (SQLiteCommand cmd = CreateCommand(conn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = commandText;

					foreach (var parameter in parameters)
					{
						cmd.Parameters.AddWithValue(
							parameter.Key,
							parameter.Value);
					}

					cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Executes scalar command.
		/// </summary>
		private object ExecuteScalar(
			string commandText,
			params ParamValue[] parameters)
		{
			using (SQLiteConnection conn = OpenConnection())
			{
				using (SQLiteCommand cmd = CreateCommand(conn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = commandText;

					foreach (var parameter in parameters)
					{
						cmd.Parameters.AddWithValue(
							parameter.Key,
							parameter.Value);
					}

					return cmd.ExecuteScalar();
				}
			}
		}

		/// <summary>
		/// Performing actions inside the reader.
		/// </summary>
		private delegate void DoWithReader(IDataRecord reader);

		/// <summary>
		/// Return result from the reader.
		/// </summary>
		private delegate T ReturnFromReader<T>(IDataRecord reader);

		/// <summary>
		/// Executes reader command.
		/// </summary>
		private void ExecuteReader(
			string commandText,
			DoWithReader action,
			params ParamValue[] parameters)
		{
			using (SQLiteConnection conn = OpenConnection())
			{
				using (SQLiteCommand cmd = CreateCommand(conn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = commandText;

					foreach (var parameter in parameters)
					{
						cmd.Parameters.AddWithValue(
							parameter.Key,
							parameter.Value);
					}

					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							action(reader);
						}
					}
				}
			}
		}

		/// <summary>
		/// Executes reader command.
		/// </summary>
		private T ExecuteReader<T>(
			string commandText,
			ReturnFromReader<T> action,
			params ParamValue[] parameters)
		{
			using (SQLiteConnection conn = OpenConnection())
			{
				using (SQLiteCommand cmd = CreateCommand(conn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = commandText;

					foreach (var parameter in parameters)
					{
						cmd.Parameters.AddWithValue(
							parameter.Key,
							parameter.Value);
					}

					using (SQLiteDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
							return action(reader);
					}
				}
			}

			return default(T);
		}

		#endregion
	}
}
