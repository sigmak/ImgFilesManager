using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgFilesManager
{
    public class Raven_XML_DB
    {
        public pNoItem[] pNoItems;


        //public override string ToString()
        //{
        //    return "empCode: " + empCode + " empName: " + empName;
        //}
    }
    public class pNoItem
    {
        public string? pNo { get; set; }
        public string? Desc { get; set; }
        public string? img { get; set; }
        public string? img_id { get; set; }
    }
}
