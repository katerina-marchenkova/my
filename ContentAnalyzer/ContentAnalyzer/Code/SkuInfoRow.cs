using System.Data;

namespace ContentAnalyzer
{
	public struct SkuInfoRow
	{
		public string ManufacturerName;
		public string ParentTree;

		public SkuInfoRow(IDataRecord reader)
		{
			ManufacturerName = (string)reader["ManufacturerName"];
			ParentTree = (string)reader["ParentTree"];
			ParentTree = ParentTree.Replace("%%%", "/");
		}
	}
}
