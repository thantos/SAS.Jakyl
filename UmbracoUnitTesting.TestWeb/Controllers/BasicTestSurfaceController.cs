using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace UmbracoUnitTesting.TestWeb.Controllers
{
    public class BasicTestSurfaceController : SurfaceController
    {
        public BasicTestSurfaceController() { }

        public BasicTestSurfaceController(UmbracoContext context, UmbracoHelper helper) : base(context, helper) { }

        public PartialViewResult BasicTestAction()
        {
            return PartialView(new { });
        }

        public PartialViewResult BasicCurrentPageAction()
        {
            return PartialView(null, CurrentPage.Name);
        }

        public PartialViewResult BasicPublishedContentAction()
        {
            return PartialView(null, this.Umbraco.AssignedContentItem.Name);
        }

        public PartialViewResult BasicDictionaryAction()
        {
            return PartialView(null,this.Umbraco.GetDictionaryValue("Test Key"));
        }

        public PartialViewResult BasicTypedContentMediaAction(int contentId, int mediaId)
        {
            return PartialView(null, new Tuple<string, string>(this.Umbraco.TypedContent(contentId).Name, this.Umbraco.TypedMedia(mediaId).Name));
        }

        public PartialViewResult BasicDynamicContentMediaAction(int contentId, int mediaId)
        {
            return PartialView(null, new Tuple<string, string>(this.Umbraco.Content(contentId).Name, this.Umbraco.Media(mediaId).Name));
        }

        public PartialViewResult BasicTypedSearchAction(string search_term)
        {
            return PartialView(null, Umbraco.TypedSearch(search_term).Count());
        }

        public PartialViewResult BasicAction()
        {
            
        }
    }
}