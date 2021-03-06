﻿using System;
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
		private string databaseFile;
		private string connectionString;

		private TimeSpan commandTimeout = TimeSpan.FromSeconds(30);
		private int commandTimeoutInSeconds = 30;

		#region Delegates

		/// <summary>
		/// Performs actions inside the reader.
		/// </summary>
		private delegate void DoWithReader(IDataRecord reader);

		/// <summary>
		/// Returns result from the reader.
		/// </summary>
		private delegate T ReturnFromReader<T>(IDataRecord reader);

		#endregion

		#region Initialization

		/// <summary>
		/// Initialize data engine.
		/// </summary>
		public void Initialize(NameValueCollection config)
		{
			this.databaseFile = ConfigurationHelper.ReadString(config, "ReleaserData.DatabaseFile");

			SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
			sb.DataSource = this.databaseFile;
			this.connectionString = sb.ConnectionString;

			if (ConfigurationHelper.HasValue(config, "ReleaserData.Timeout"))
			{
				this.commandTimeout = ConfigurationHelper.ReadTimeSpan(config, "ReleaserData.Timeout");
				this.commandTimeoutInSeconds = Convert.ToInt32(this.commandTimeout.TotalSeconds);
			}

			if (!File.Exists(this.databaseFile))
			{
				SQLiteConnection.CreateFile(this.databaseFile);
				this.CreateDatabase();
			}
		}

		#endregion

		#region Project management

		/// <summary>
		/// Adds project.
		/// </summary>
		public int AddProject(ProjectData data)
		{
			object id = this.ExecuteScalar(
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
			this.ExecuteNonQuery(
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
			this.ExecuteNonQuery(
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
			return this.ExecuteReader(
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
				delegate(IDataRecord reader)
				{
					int id = (int)Read.Int64(reader, "ProjectId");
					ProjectData data = new ProjectData(
						Read.String(reader, "ProjectName"),
						Read.String(reader, "ImageCode"),
						Read.String(reader, "StorageCode"),
						Read.String(reader, "StoragePath"));

					return new ProjectRow(id, data);
				},
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

			this.ExecuteNonQuery(
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

			this.ExecuteReader(
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

		#region Managing database schema

		/// <summary>
		/// Creates new database.
		/// </summary>
		private void CreateDatabase()
		{
			this.ExecuteNonQuery(
				@"
					CREATE TABLE [User] (
						[UserId] INTEGER PRIMARY KEY,
						[UserName] TEXT NOT NULL,
						[ImageCode] TEXT NOT NULL,
						[Login] TEXT NOT NULL);
				");

			this.ExecuteNonQuery(
				@"
					CREATE TABLE [Project] (
						[ProjectId] INTEGER PRIMARY KEY,
						[ProjectName] TEXT NOT NULL,
						[ImageCode] TEXT NOT NULL,
						[StorageCode] TEXT NOT NULL,
						[StoragePath] TEXT NOT NULL);
				");

			this.ExecuteNonQuery(
				@"
					CREATE TABLE [Property] (
						[PropertyCode] TEXT PRIMARY KEY,
						[PropertyName] TEXT NOT NULL);
				");

			this.ExecuteNonQuery(
				@"
					CREATE TABLE [PropertyUsage] (
						[PropertyCode] TEXT NOT NULL,
						[EntityType] TEXT NOT NULL,
						PRIMARY KEY (
								[PropertyCode],
								[EntityType]));
				");

			this.ExecuteNonQuery(
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

		#region Service methods

		/// <summary>
		/// Creates and opens database connection.
		/// </summary>
		private SQLiteConnection OpenConnection()
		{
			SQLiteConnection conn = new SQLiteConnection(this.connectionString);
			conn.Open();
			return conn;
		}

		/// <summary>
		/// Creates database command.
		/// </summary>
		private SQLiteCommand CreateCommand(SQLiteConnection conn)
		{
			SQLiteCommand cmd = conn.CreateCommand();
			cmd.CommandTimeout = this.commandTimeoutInSeconds;
			return cmd;
		}

		/// <summary>
		/// Executes non-query command.
		/// </summary>
		private void ExecuteNonQuery(
			string commandText,
			params ParamValue[] parameters)
		{
			using (SQLiteConnection conn = this.OpenConnection())
			{
				using (SQLiteCommand cmd = this.CreateCommand(conn))
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
			using (SQLiteConnection conn = this.OpenConnection())
			{
				using (SQLiteCommand cmd = this.CreateCommand(conn))
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
		/// Executes reader command.
		/// </summary>
		private void ExecuteReader(
			string commandText,
			DoWithReader action,
			params ParamValue[] parameters)
		{
			using (SQLiteConnection conn = this.OpenConnection())
			{
				using (SQLiteCommand cmd = this.CreateCommand(conn))
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
			using (SQLiteConnection conn = this.OpenConnection())
			{
				using (SQLiteCommand cmd = this.CreateCommand(conn))
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
						{
							return action(reader);
						}
					}
				}
			}

			return default(T);
		}

		#endregion
	}
}
