using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMF.Models
{
    public class UMFGroupModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public List<UMFCategoryModel> Categorys { get; set; }
    }
}