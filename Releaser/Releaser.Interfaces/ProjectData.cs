using System;

namespace Shuruev.Releaser.Interfaces
{
	/// <summary>
	/// Data associated with project.
	/// </summary>
	public class ProjectData
	{
		private string projectName;
		private string imageCode;
		private string storageCode;
		private string storagePath;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectData"/> class.
		/// </summary>
		public ProjectData(
			string projectName,
			string imageCode,
			string storageCode,
			string storagePath)
		{
			this.ProjectName = projectName;
			this.ImageCode = imageCode;
			this.StorageCode = storageCode;
			this.StoragePath = storagePath;
		}

		#region Properties

		/// <summary>
		/// Gets or sets project name.
		/// </summary>
		public string ProjectName
		{
			get
			{
				return this.projectName;
			}

			set
			{
				if (StringHelper.IsEmpty(value))
				{
					throw new ArgumentException("Property ProjectName should not be empty.");
				}

				this.projectName = value;
			}
		}

		/// <summary>
		/// Gets or sets image code.
		/// </summary>
		public string ImageCode
		{
			get
			{
				return this.imageCode;
			}

			set
			{
				if (StringHelper.IsEmpty(value))
				{
					throw new ArgumentException("Property ImageCode should not be empty.");
				}

				this.imageCode = value;
			}
		}

		/// <summary>
		/// Gets or sets the code of storage provider.
		/// </summary>
		public string StorageCode
		{
			get
			{
				return this.storageCode;
			}

			set
			{
				if (StringHelper.IsEmpty(value))
				{
					throw new ArgumentException("Property StorageCode should not be empty.");
				}

				this.storageCode = value;
			}
		}

		/// <summary>
		/// Gets or sets the path inside storage that identifies current project.
		/// </summary>
		public string StoragePath
		{
			get
			{
				return this.storagePath;
			}

			set
			{
				if (StringHelper.IsEmpty(value))
				{
					throw new ArgumentException("Property StoragePath should not be empty.");
				}

				this.storagePath = value;
			}
		}

		#endregion

		#region Service methods

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return this.projectName;
		}

		#endregion
	}
}
