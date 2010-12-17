namespace SomeNamespace
{
	public class SP2001
	{
		public bool SomeCondition()
		{
			return true;
		}

		public void SomeMethod()
		{
			int a = 10;
		    int b = 20;

			if (SomeCondition()
			    && a < b)
			{
			}
		}
	}
}
