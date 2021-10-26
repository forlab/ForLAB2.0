using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForLabApi.Models
{
    public class Importdata
    {
        public int[] Testingareaids { get; set; }
        public int[] Producttypeids { get; set; }
        public int[] Programids { get; set; }
        public int userid { get; set; }
        public bool importtest { get; set; }
        public bool importproduct { get; set; }
        public bool importproductusage { get; set; }
        public bool importprogram { get; set; }
    }

    public class gettreeview
    {
        public string content { get; set; }

    }
    public class Matchrule
    {
        public string regSheet { get; set; }
        public string docSheet { get; set; }

        public List<Matchcols> columns { get; set; }
    }
    public class Matchcols
    {
        public string regCol { get; set; }
        public string docCol { get; set; }
    }
    public class listtree
    {
        public List<gettreeview> ilist { get; set; }
    }
}
