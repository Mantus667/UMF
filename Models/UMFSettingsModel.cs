using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMF.Models
{
    public class UMFSettingsModel
    {
        public bool useKarma { get; set; }
        public int KarmaForDiscussion { get; set; }
        public int KarmaForAnswer { get; set; }
        public int KarmaForMarkedAnswer { get; set; }
    }
}