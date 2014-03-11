using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMF.Models
{
    public class UMFCategoryModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public string url { get; set; }
        public int number_of_discussion { get; set; }
        public int number_of_answers { get; set; }
        public bool hasAccess { get; set; }
        public UMFLatestDiscussionModel latest_discussion { get; set; }
    }

    public class UMFCategoryViewModel {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public List<UMFDiscussionOverviewModel> discussions { get; set; }
    }
}