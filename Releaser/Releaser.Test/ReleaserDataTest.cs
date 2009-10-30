using System;
using System.Collections.Specialized;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shuruev.Releaser.Data;
using Shuruev.Releaser.Interfaces;

namespace Shuruev.Releaser.Test
{
	/// <summary>
	/// Testing data storages.
	/// </summary>
	[TestClass]
	public class ReleaserDataTest
	{
		private static readonly string databaseFile = @"D:\ReleaserData.dat";
		private static readonly ReleaserDataSQLite database = new ReleaserDataSQLite();

		#region Service methods

		/// <summary>
		/// Code that runs before running the first test in the class.
		/// </summary>
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			if (File.Exists(databaseFile))
			{
				File.Delete(databaseFile);
			}

			NameValueCollection config = new NameValueCollection();
			config["ReleaserData.DatabaseFile"] = databaseFile;
			config["ReleaserData.Timeout"] = "00:00:10";

			database.Initialize(config);

			Assert.IsTrue(File.Exists(databaseFile));
		}

		/// <summary>
		/// Code that runs after all tests in a class have run.
		/// </summary>
		[ClassCleanup]
		public static void MyClassCleanup()
		{
			if (File.Exists(databaseFile))
			{
				File.Delete(databaseFile);
			}
		}

		#endregion

		#region Project management

		/// <summary>
		/// Tests identity of project ID.
		/// </summary>
		[TestMethod]
		public void ProjectIdIdentity()
		{
			ProjectData data = new ProjectData(
				"New Project",
				"Default",
				"None",
				"None");

			int id = database.AddProject(data);
			Assert.AreEqual(1, id);

			id = database.AddProject(data);
			Assert.AreEqual(2, id);

			id = database.AddProject(data);
			Assert.AreEqual(3, id);

			database.RemoveProject(2);

			id = database.AddProject(data);
			Assert.AreEqual(4, id);

			database.RemoveProject(3);
			database.RemoveProject(4);

			id = database.AddProject(data);
			Assert.AreEqual(2, id);
		}

		/// <summary>
		/// Tests operations with projects.
		/// </summary>
		[TestMethod]
		public void ProjectOperations()
		{
			ProjectData data = new ProjectData(
				"New Project",
				"Default",
				"GIT",
				"None");
			int id = database.AddProject(data);

			ProjectRow project = database.GetProject(id);
			Assert.AreEqual(id, project.Id);
			Assert.AreEqual("New Project", project.Data.ProjectName);
			Assert.AreEqual("Default", project.Data.ImageCode);
			Assert.AreEqual("GIT", project.Data.StorageCode);
			Assert.AreEqual("None", project.Data.StoragePath);

			data = new ProjectData(
				"Updated Project",
				"Photo",
				"SourceSafe",
				"Path");
			database.UpdateProject(id, data);

			project = database.GetProject(id);
			Assert.AreEqual(id, project.Id);
			Assert.AreEqual("Updated Project", project.Data.ProjectName);
			Assert.AreEqual("Photo", project.Data.ImageCode);
			Assert.AreEqual("SourceSafe", project.Data.StorageCode);
			Assert.AreEqual("Path", project.Data.StoragePath);

			database.RemoveProject(id);

			project = database.GetProject(id);
			Assert.IsNull(project);

			database.RemoveProject(id);
			database.RemoveProject(Int32.MaxValue);
		}

		#endregion

		#region Property management

		/// <summary>
		/// Tests operations with properties.
		/// </summary>
		[TestMethod]
		public void PropertyOperations()
		{
			PropertyValues properties = new PropertyValues();
			properties.Add("Package", "zip");
			properties.Add("Keywords", "production");
			properties.Add("Keywords", "application");
			database.SetProperties(EntityType.Project, 10, properties);

			properties = database.GetProperties(EntityType.Project, 10);
			Assert.AreEqual(2, properties.GetCodes().Count);
			Assert.AreEqual(1, properties.GetValues("Package").Count);
			Assert.AreEqual("zip", properties.GetValues("Package")[0]);
			Assert.AreEqual(2, properties.GetValues("Keywords").Count);
			Assert.AreEqual("application", properties.GetValues("Keywords")[0]);
			Assert.AreEqual("production", properties.GetValues("Keywords")[1]);

			properties = new PropertyValues();
			properties.Add("Package", "zip");
			properties.Add("Package", "msi");
			properties.Add("Keywords", "service");
			properties.Add("Status", "beta");
			database.SetProperties(EntityType.Project, 10, properties);

			properties = database.GetProperties(EntityType.Project, 10);
			Assert.AreEqual(3, properties.GetCodes().Count);
			Assert.AreEqual(2, properties.GetValues("Package").Count);
			Assert.AreEqual("msi", properties.GetValues("Package")[0]);
			Assert.AreEqual("zip", properties.GetValues("Package")[1]);
			Assert.AreEqual(1, properties.GetValues("Keywords").Count);
			Assert.AreEqual("service", properties.GetValues("Keywords")[0]);
			Assert.AreEqual(1, properties.GetValues("Status").Count);
			Assert.AreEqual("beta", properties.GetValues("Status")[0]);

			properties = new PropertyValues();
			database.SetProperties(EntityType.Project, 10, properties);

			properties = database.GetProperties(EntityType.Project, 10);
			Assert.AreEqual(0, properties.GetCodes().Count);
		}

		#endregion
	}
}
