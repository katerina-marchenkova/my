using System;
using System.Collections.Generic;
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
		public static string DatabaseFile = @"D:\ReleaserData.dat";
		public static ReleaserDataSQLite Data = new ReleaserDataSQLite();

		public List<string> EmptyWords = new List<string>();

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ReleaserDataTest()
		{
			EmptyWords.Add("   ");
			EmptyWords.Add("\r\n\r\n");
			EmptyWords.Add("\t\t\v\v");
			EmptyWords.Add("   \t\t   \r\n   \v\v   ");
			EmptyWords.Add(null);
		}

		#region Service methods

		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			if (File.Exists(DatabaseFile))
				File.Delete(DatabaseFile);

			NameValueCollection config = new NameValueCollection();
			config["ReleaserData.DatabaseFile"] = DatabaseFile;
			config["ReleaserData.Timeout"] = "00:00:10";

			Data = new ReleaserDataSQLite();
			Data.Initialize(config);

			Assert.IsTrue(File.Exists(DatabaseFile));
		}

		[ClassCleanup]
		public static void MyClassCleanup()
		{
			if (File.Exists(DatabaseFile))
				File.Delete(DatabaseFile);
		}

		#endregion

		#region Project management

		[TestMethod]
		public void ProjectIdIdentity()
		{
			ProjectData data = new ProjectData(
				"New Project",
				"Default",
				"None",
				"None");

			int id = Data.AddProject(data);
			Assert.AreEqual(1, id);

			id = Data.AddProject(data);
			Assert.AreEqual(2, id);

			id = Data.AddProject(data);
			Assert.AreEqual(3, id);

			Data.RemoveProject(2);

			id = Data.AddProject(data);
			Assert.AreEqual(4, id);

			Data.RemoveProject(3);
			Data.RemoveProject(4);

			id = Data.AddProject(data);
			Assert.AreEqual(2, id);
		}

		[TestMethod]
		public void ProjectOperations()
		{
			ProjectData data = new ProjectData(
				"New Project",
				"Default",
				"GIT",
				"None");
			int id = Data.AddProject(data);

			ProjectRow project = Data.GetProject(id);
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
			Data.UpdateProject(id, data);

			project = Data.GetProject(id);
			Assert.AreEqual(id, project.Id);
			Assert.AreEqual("Updated Project", project.Data.ProjectName);
			Assert.AreEqual("Photo", project.Data.ImageCode);
			Assert.AreEqual("SourceSafe", project.Data.StorageCode);
			Assert.AreEqual("Path", project.Data.StoragePath);

			Data.RemoveProject(id);

			project = Data.GetProject(id);
			Assert.IsNull(project);

			Data.RemoveProject(id);
			Data.RemoveProject(Int32.MaxValue);
		}

		#endregion

		#region Property management

		[TestMethod]
		public void PropertyOperations()
		{
			PropertyValues properties = new PropertyValues();
			properties.Add("Package", "zip");
			properties.Add("Keywords", "production");
			properties.Add("Keywords", "application");
			Data.SetProperties(EntityType.Project, 10, properties);

			properties = Data.GetProperties(EntityType.Project, 10);
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
			Data.SetProperties(EntityType.Project, 10, properties);

			properties = Data.GetProperties(EntityType.Project, 10);
			Assert.AreEqual(3, properties.GetCodes().Count);
			Assert.AreEqual(2, properties.GetValues("Package").Count);
			Assert.AreEqual("msi", properties.GetValues("Package")[0]);
			Assert.AreEqual("zip", properties.GetValues("Package")[1]);
			Assert.AreEqual(1, properties.GetValues("Keywords").Count);
			Assert.AreEqual("service", properties.GetValues("Keywords")[0]);
			Assert.AreEqual(1, properties.GetValues("Status").Count);
			Assert.AreEqual("beta", properties.GetValues("Status")[0]);

			properties = new PropertyValues();
			Data.SetProperties(EntityType.Project, 10, properties);

			properties = Data.GetProperties(EntityType.Project, 10);
			Assert.AreEqual(0, properties.GetCodes().Count);
		}

		#endregion
	}
}
