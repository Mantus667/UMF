using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.member;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UMF.Models
{
    public class UMFAnswerModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public string text { get; set; }
        public bool isAnswer { get; set; }
        public int rating { get; set; }
        public Member writer { get; set; }
    }

    public class UMFAnswerSimpleModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class UMFCreateAnswerModel
    {
        public int id { get; set; }

        [Required]
        [AllowHtml]
        public string text { get; set; }

        [Required]
        public int parent { get; set; }
    }
}