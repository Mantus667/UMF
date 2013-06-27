using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.member;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UMF.Models
{
    public class UMFDiscussionOverviewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public int number_of_answers { get; set; }
        public string url { get; set; }
        public bool hasAnswer { get; set; }
        public int rating { get; set; }
        public Member writer { get; set; }
        public UMFAnswerSimpleModel latest_answer { get; set; }
    }

    public class UMFDiscussionDetailModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public string text { get; set; }
        public bool hasAnswer { get; set; }
        public int rating { get; set; }
        public Member writer { get; set; }
        public List<UMFAnswerModel> answers { get; set; }
    }

    public class UMFLatestDiscussionModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public string url { get; set; }
    }

    public class UMFCreateDiscussionModel
    {
        public int id { get; set; }
        [Required]
        public int parent { get; set; }

        [Required]
        public string name { get; set; }

        [AllowHtml]
        [Required]
        public string text { get; set; }
    }
}