using System;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Data associated with project.
	/// </summary>
	public class ProjectData
	{
		private string m_projectName;
		private string m_imageCode;
		private string m_storageCode;
		private string m_storagePath;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public ProjectData(
			string projectName,
			string imageCode,
			string storageCode,
			string storagePath)
		{
			ProjectName = projectName;
			ImageCode = imageCode;
			StorageCode = storageCode;
			StoragePath = storagePath;
		}

		#region Properties

		/// <summary>
		/// Gets or sets project name.
		/// </summary>
		public string ProjectName
		{
			get
			{
				return m_projectName;
			}
			set
			{
				if (StringHelper.IsEmpty(value))
					throw new ArgumentException("Property ProjectName should not be empty.");

				m_projectName = value;
			}
		}

		/// <summary>
		/// Gets or sets image code.
		/// </summary>
		public string ImageCode
		{
			get
			{
				return m_imageCode;
			}
			set
			{
				if (StringHelper.IsEmpty(value))
					throw new ArgumentException("Property ImageCode should not be empty.");

				m_imageCode = value;
			}
		}

		/// <summary>
		/// Gets or sets the code or storage provider.
		/// </summary>
		public string StorageCode
		{
			get
			{
				return m_storageCode;
			}
			set
			{
				if (StringHelper.IsEmpty(value))
					throw new ArgumentException("Property StorageCode should not be empty.");

				m_storageCode = value;
			}
		}

		/// <summary>
		/// Gets or sets the path inside storage that identifies current project.
		/// </summary>
		public string StoragePath
		{
			get
			{
				return m_storagePath;
			}
			set
			{
				if (StringHelper.IsEmpty(value))
					throw new ArgumentException("Property StoragePath should not be empty.");

				m_storagePath = value;
			}
		}

		#endregion

		#region Service methods

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return m_projectName;
		}

		#endregion
	}
}
