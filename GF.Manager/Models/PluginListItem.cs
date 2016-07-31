using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GF.Manager.Models
{
    public class PluginListItem : PluginItem
    {
        public string Keyword { get; set; }

        public string Orderby { get; set; }

        public int Page { get; set; }

        public int Count { get; set; }

        public string KeyName { get; set; }

        public IReadOnlyList<PluginListColumn> Columns { get; set; }
    }

    public class PluginListColumn
    {
        public string Name { get; set; }

        public string Title { get; set; }
    }
}