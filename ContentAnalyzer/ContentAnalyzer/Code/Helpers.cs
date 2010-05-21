using System;

namespace ContentAnalyzer
{
	public static class Helpers
	{
		/// <summary>
		/// Gets an URL for digital content file.
		/// </summary>
		public static string GetDigitalContentUrl(Guid contentUid)
		{
			string uid = contentUid.ToString().ToLowerInvariant();
			return String.Format(
				"https://cdn.cnetcontent.com/{0}/{1}/{2}.jpg",
				uid.Substring(0, 2),
				uid.Substring(2, 2),
				uid);
		}
	}
}
