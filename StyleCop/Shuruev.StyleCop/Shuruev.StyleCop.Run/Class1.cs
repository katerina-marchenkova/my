using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shuruev.StyleCop.Test
{
	public class Class1
	{
		private class Entity
		{
			int Id;
			string Name;
		}

		private IQueryable<Entity> sectors;

		[HttpPost]
        public ActionResult Search(string searchCriteria, IEnumerable<int?> selected, int? page = null)
        {
            if (page.HasValue)
            {
                const int PageSize = 6;
                IEnumerable src =
                    this.sectors.Where(o => (selected == null || !selected.Contains(o.Id)) && o.Name.Contains(searchCriteria));
                string rows = this.RenderView(
                    @"Awesome\LookupList", src.Skip((page.Value - 1) * PageSize).Take(PageSize));
                return this.Json(new { rows, more = src.Count() > page * PageSize });
            }

        return this.View(
            @"Awesome\LookupList",
            this.sectors.Where(o => (selected == null || !selected.Contains(o.Id)) && o.Name.Contains(searchCriteria)));
		}
	}
}
