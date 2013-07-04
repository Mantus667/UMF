using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UMF.Models;
using Umbraco.Web.Routing;
using Umbraco.Web;
using umbraco.cms.businesslogic.member;
using System.Web.Security;
using Umbraco.Core.Models;
using Examine;
using System.Diagnostics;
using Examine.SearchCriteria;

namespace UMF.Controllers
{
    [PluginController("UMF")]
    public class UMFSurfaceController : SurfaceController
    {
        /// <summary>
        /// Gets the content to display on the overview page of the forum
        /// </summary>
        /// <param name="id">The id of the forum to target</param>
        [ChildActionOnly]
        public ActionResult GetOverviewData(int id)
        {
            var cs = Services.ContentService;
            var helper = new UmbracoHelper(UmbracoContext);
            var root = cs.GetById(id);
            List<UMFGroupModel> groups = new List<UMFGroupModel>();
            foreach (var group in cs.GetChildren(id).Where(x => x.Published && x.ContentType.Alias == "UMF_Group")) //&& Convert.ToBoolean(x.GetValue("umbracoNaviHide").ToString()) == false))
            {
                UMFGroupModel tmp = new UMFGroupModel()
                {
                    id = group.Id,
                    name = group.Name,
                    created = group.CreateDate,
                    Categorys = new List<UMFCategoryModel>()
                };
                foreach (var category in cs.GetChildren(group.Id).Where(x => x.Published))
                {
                    var tmp2 = new UMFCategoryModel()
                    {
                        id = category.Id,
                        name = category.Name,
                        created = category.CreateDate
                    };

                    //Does the user have access
                    try
                    {
                        bool onlySpecialGroup = false, inGroup = false;
                        onlySpecialGroup = Convert.ToBoolean(int.Parse(category.GetValue("allowOnlySpecialGroups").ToString()));
                        if (onlySpecialGroup)
                        {
                            tmp2.hasAccess = false;
                            foreach (string val in category.GetValue("allowedGroups").ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var member = Membership.GetUser();
                                if (member != null)
                                {
                                    var mem_group = MemberGroup.GetByName(val);
                                    inGroup = mem_group.HasMember((int)member.ProviderUserKey);
                                    if (inGroup)
                                    {
                                        tmp2.hasAccess = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    tmp2.hasAccess = false;
                                }
                            }
                        }
                        else
                        {
                            tmp2.hasAccess = true;
                        }
                    }
                    catch { tmp2.hasAccess = true; }

                    //Check number of Answers and number of discussions
                    try
                    {
                        tmp2.number_of_answers = cs.GetDescendants(category).Where(x => x.ContentType.Alias == "UMF_Answer").Count();
                        tmp2.number_of_discussion = cs.GetDescendants(category).Where(x => x.ContentType.Alias == "UMF_Discussion").Count();
                        tmp2.url = helper.TypedContent(category.Id).Url;
                    }
                    catch { }

                    //Try to get the latest discussion
                    try
                    {
                        var latest_discussion = cs.GetChildren(category.Id).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                        tmp2.latest_discussion = new UMFLatestDiscussionModel()
                        {
                            id = latest_discussion.Id,
                            name = latest_discussion.Name,
                            created = latest_discussion.CreateDate,
                            url = helper.TypedContent(latest_discussion.Id).Url
                        };
                    }
                    catch { }

                    tmp.Categorys.Add(tmp2);
                }
                groups.Add(tmp);
            }

            return PartialView("UMFOverview",groups);
        }

        /// <summary>
        /// Gets the content for a single category
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <returns>A category details models with all discussions in it</returns>
        [ChildActionOnly]
        public ActionResult GetCategory(int id)
        {
            UMFCategoryViewModel model = new UMFCategoryViewModel();
            var cs = Services.ContentService;
            var category = cs.GetById(id);
            var helper = new UmbracoHelper(UmbracoContext);

            model.id = id;
            model.created = category.CreateDate;
            model.name = category.Name;
            model.discussions = new List<UMFDiscussionOverviewModel>();

            foreach (var disc in cs.GetChildren(id))
            {
                UMFDiscussionOverviewModel tmp = new UMFDiscussionOverviewModel();
                tmp.id = disc.Id;
                tmp.name = disc.Name;
                tmp.created = disc.CreateDate;
                tmp.number_of_answers = cs.GetChildren(disc.Id).Count();
                tmp.url = helper.TypedContent(disc.Id).Url;
                tmp.hasAnswer = (null != cs.GetChildren(disc.Id).FirstOrDefault(x => null != x.GetValue("isAnswer") && x.GetValue("isAnswer").ToString() == "1")) ? true : false;
                int score = 0, votes = 0, rating  = 0;
                try {
                    int.TryParse(disc.GetValue("timesRated").ToString(), out votes);
                    int.TryParse(disc.GetValue("voteScore").ToString(), out score);
                    rating = (int)Math.Round((decimal)(score / votes), 0);
                }
                catch { }
                tmp.rating = rating;
                int userId = int.Parse(disc.GetValue("userId").ToString());
                if (userId != 0)
                {
                    try
                    {
                        tmp.writer = Member.GetAllAsList().SingleOrDefault(x => x.Id == userId);
                    }
                    catch { tmp.writer = null; }
                }
                else
                {
                    tmp.writer = null;
                }

                model.discussions.Add(tmp);
            }

            return PartialView("UMFCategory", model);
        }

        /// <summary>
        /// Gets a single discussion with all it's answers
        /// </summary>
        /// <param name="id">The id of the discussion</param>
        /// <returns>Discussion details model with answers</returns>
        [ChildActionOnly]
        public ActionResult GetDiscussion(int id)
        {
            UMFDiscussionDetailModel model = new UMFDiscussionDetailModel();
            var cs = Services.ContentService;
            var discussion = cs.GetById(id);
            var helper = new UmbracoHelper(UmbracoContext);

            model.id = id;
            model.name = discussion.Name;
            model.text = discussion.GetValue("bodyText").ToString();
            model.created = discussion.CreateDate;
            model.hasAnswer = (null != cs.GetChildren(id).FirstOrDefault(x => null != x.GetValue("isAnswer") && x.GetValue("isAnswer").ToString() == "1")) ? true : false;
            int score = 0, votes = 0, rating = 0;
            try
            {
                int.TryParse(discussion.GetValue("timesRated").ToString(), out votes);
                int.TryParse(discussion.GetValue("voteScore").ToString(), out score);
                rating = (int)Math.Round((decimal)(score / votes), 0);
            }
            catch { }
            model.rating = rating;
            int userId = 0;
            try
            {
                userId = Convert.ToInt32(discussion.GetValue("userId").ToString().Trim());
                if (userId != 0)
                {
                    try
                    {
                        model.writer = Member.GetAllAsList().SingleOrDefault(x => x.Id == userId);
                    }
                    catch { model.writer = null; }
                }
                else
                {
                    model.writer = null;
                }
            }
            catch { model.writer = null; }
            
            model.answers = new List<UMFAnswerModel>();

            foreach (var answer in cs.GetChildren(id).OrderBy(x => x.CreateDate))
            {
                score = 0; votes = 0; rating = 0;
                try
                {
                    int.TryParse(answer.GetValue("timesRated").ToString(), out votes);
                    int.TryParse(answer.GetValue("voteScore").ToString(), out score);
                    rating = (int)Math.Round((decimal)(score / votes), 0);
                }
                catch { }
                var answerm = new UMFAnswerModel(){
                    id = answer.Id,
                    name = answer.Name,
                    created = answer.CreateDate,
                    text = answer.GetValue("bodyText").ToString(),
                    isAnswer = (null != answer.GetValue("isAnswer") && answer.GetValue("isAnswer").ToString() == "1" ) ? true : false,
                    rating = rating
                };
                int auserId = 0;
                int.TryParse(answer.GetValue("userId").ToString().Trim(), out auserId);
                if (auserId != 0)
                {
                    try
                    {
                        answerm.writer = Member.GetAllAsList().SingleOrDefault(x => x.Id == userId);
                    }
                    catch { answerm.writer = null; }
                }
                else
                {
                    answerm.writer = null;
                }

                model.answers.Add(answerm);
            }

            return PartialView("UMFDiscussion", model);
        }

        [ChildActionOnly]
        public ActionResult RenderCreateDiscussion(int parent, int discussion = 0)
        {
            if (discussion == 0)
            {
                return PartialView("UMFCreateDiscussion", new UMFCreateDiscussionModel() { parent = parent });
            }
            else
            {
                var cs = Services.ContentService;
                var current = cs.GetById(discussion);
                return PartialView("UMFCreateDiscussion",
                    new UMFCreateDiscussionModel()
                    {
                        id = discussion,
                        parent = parent,
                        name = current.Name,
                        text = current.GetValue("bodyText").ToString()
                    });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleCreateDiscussion(UMFCreateDiscussionModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var cs = Services.ContentService;
            var helper = new UmbracoHelper(UmbracoContext);
            int userId = 0;
            try
            {
                int.TryParse(Membership.GetUser().ProviderUserKey.ToString(), out userId);
            }
            catch { }

            //Check if its a new discussion or one gets updated
            if (model.id == 0)
            {
                //Create a new Discussion
                var newDiscusssion = cs.CreateContent(model.name, cs.GetById(model.parent), "UMF_Discussion");
                newDiscusssion.SetValue("bodyText", model.text);
                newDiscusssion.SetValue("userId", userId);
                cs.SaveAndPublish(newDiscusssion);

                //Insert new count of posts to the member and update karma
                if (0 != userId)
                {
                    var settings = this.GetSettings(newDiscusssion.Id);
                    var member = new Member(userId);
                    int noPosts = 0, karma = 0;
                    try
                    {
                        if (int.TryParse(member.getProperty("numberOfPosts").Value.ToString(), out noPosts))
                        {
                            //Managed to parse it to a number
                            //Don't need to do anything as we have default value of 0
                        }
                        member.getProperty("numberOfPosts").Value = noPosts + 1;
                    }
                    catch { }
                    try
                    {
                        if (settings.useKarma)
                        {
                            int.TryParse(member.getProperty("karma").Value.ToString(), out karma);
                            member.getProperty("karma").Value = karma + settings.KarmaForAnswer;
                        }
                    }
                    catch { }
                    member.Save();
                }
            }
            else
            {
                //Update discussion
                var discussion = cs.GetById(model.id);
                discussion.Name = model.name;
                discussion.SetValue("bodyText", model.text);
                cs.SaveAndPublish(discussion);
            }

            return Redirect(helper.TypedContent(model.parent).Url);
        }

        [ChildActionOnly]
        public ActionResult RenderCreateAnswer(int parent, int answer = 0)
        {
            if (answer == 0)
            {
                return PartialView("UMFCreateAnswer", new UMFCreateAnswerModel() { parent = parent });
            }
            else
            {
                var cs = Services.ContentService;
                var current = cs.GetById(answer);
                return PartialView("UMFCreateAnswer",
                    new UMFCreateAnswerModel()
                    {
                        id = answer,
                        parent = parent,
                        text = current.GetValue("bodyText").ToString()
                    });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleCreateAnswer(UMFCreateAnswerModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var cs = Services.ContentService;
            var helper = new UmbracoHelper(UmbracoContext);
            int userId = 0;
            try
            {
                int.TryParse(Membership.GetUser().ProviderUserKey.ToString().Trim(), out userId);
            }
            catch { }

            //Check if its a new one or it gets updated
            if (model.id == 0)
            {
                //Create new answer                
                var newDiscusssion = cs.CreateContent("Comment-" + helper.TypedContent(model.parent).Children.Count().ToString(), cs.GetById(model.parent), "UMF_Answer");
                newDiscusssion.SetValue("bodyText", model.text);
                newDiscusssion.SetValue("userId", userId);
                cs.SaveAndPublish(newDiscusssion);

                if (userId != 0)
                {
                    //Insert new count of posts to the member and update karma
                    var settings = this.GetSettings(newDiscusssion.Id);
                    var member = new Member(userId);
                    int noPosts = 0, karma = 0;
                    if (int.TryParse(member.getProperty("numberOfPosts").Value.ToString(), out noPosts))
                    {
                        //Managed to parse it to a number
                        //Don't need to do anything as we have default value of 0
                    }
                    member.getProperty("numberOfPosts").Value = noPosts + 1;
                    if (settings.useKarma)
                    {
                        int.TryParse(member.getProperty("karma").Value.ToString(), out karma);
                        member.getProperty("karma").Value = karma + settings.KarmaForAnswer;
                    }
                    member.Save();
                }
            }
            else
            {
                //Update answer
                var discussion = cs.GetById(model.id);
                discussion.SetValue("bodyText", model.text);
                cs.SaveAndPublish(discussion);
            }

            return Redirect(helper.TypedContent(model.parent).Url);
        }

        [HttpGet]
        public ActionResult MarkAsAnswer(int answer, int discussion)
        {
            var cs = Services.ContentService;
            var helper = new UmbracoHelper(UmbracoContext);

            //Update Answer
            var current = cs.GetById(answer);
            current.SetValue("isAnswer", true);
            cs.SaveAndPublish(current);

            //Uopdate Karma
            var settings = this.GetSettings(answer);            
            if (settings.useKarma)
            {
                int userId = Convert.ToInt32(current.GetValue("userId").ToString());
                var member = new Member(userId);
                int karma = 0;
                int.TryParse(member.getProperty("karma").Value.ToString(),out karma);
                member.getProperty("karma").Value = karma + settings.KarmaForMarkedAnswer;
                member.Save();
            }

            return Redirect(helper.TypedContent(discussion).Url);
        }

        [ChildActionOnly]
        public ActionResult Search(string searchTerm)
        {
            List<UMFSearchResultModel> results = new List<UMFSearchResultModel>();
            var Searcher = ExamineManager.Instance.SearchProviderCollection["UMFSearcher"];
            var searchCriteria = Searcher.CreateSearchCriteria(BooleanOperation.Or);
            var query = searchCriteria.Field("nodeName", searchTerm).Or().Field("bodyText", searchTerm).Compile();
            var searchResults = Searcher.Search(query);

            foreach (var result in searchResults)
            {
                var node = Umbraco.TypedContent(result.Fields["id"]);
                string url = (node.DocumentTypeAlias == "UMF_Answer") ? node.Parent.UrlWithDomain().ToString() : node.UrlWithDomain().ToString();
                UMFSearchResultModel model = new UMFSearchResultModel() {
                    name = result.Fields["nodeName"],
                    url = url
                };
                results.Add(model);
            }
            return PartialView("UMFSearch", results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RateDiscussion(int score, int discussion)
        {
            var cs = Services.ContentService;
            var current = cs.GetById(discussion);
            int oldScore = 0, oldNo = 0;
            try
            {
                int.TryParse(current.GetValue("voteScore").ToString(), out oldScore);
                int.TryParse(current.GetValue("timesRated").ToString(), out oldNo);

                current.SetValue("voteScore", oldScore + score);
                current.SetValue("timesRated", oldNo + 1);                

                //Set cookie so he can't rate more than 1 time
                HttpCookie myCookie = HttpContext.Request.Cookies["RatingDiscussion-" + discussion.ToString()] ?? new HttpCookie("RatingDiscussion-" + discussion.ToString());
                myCookie.Expires = DateTime.Now.AddDays(365);
                HttpContext.Response.SetCookie(myCookie);

                //Save the changes
                cs.SaveAndPublish(current);

                return PartialView("UMFRatingSuccess");
            }
            catch {
                return PartialView("UMFRatingError");
            }
        }

        private UMFSettingsModel GetSettings(int currentNode)
        {
            var cs = Services.ContentService;
            var root = cs.GetAncestors(cs.GetById(currentNode)).FirstOrDefault(x => x.ContentType.Alias == "UMF_Root");
            return new UMFSettingsModel()
            {
                useKarma = (!String.IsNullOrEmpty(root.GetValue("useKarma").ToString()) && root.GetValue("useKarma").ToString() != "0") ? true : false,
                KarmaForDiscussion = (!String.IsNullOrEmpty(root.GetValue("karmaPointsForDiscussion").ToString())) ? Convert.ToInt32(root.GetValue("karmaPointsForDiscussion").ToString()) : 0,
                KarmaForAnswer = (!String.IsNullOrEmpty(root.GetValue("karmaPointsForAnswer").ToString())) ? Convert.ToInt32(root.GetValue("karmaPointsForAnswer").ToString()) : 0,
                KarmaForMarkedAnswer = (!String.IsNullOrEmpty(root.GetValue("karmaPointsForMarkedAnswer").ToString())) ? Convert.ToInt32(root.GetValue("karmaPointsForMarkedAnswer").ToString()) : 0
            };
        }
    }
}