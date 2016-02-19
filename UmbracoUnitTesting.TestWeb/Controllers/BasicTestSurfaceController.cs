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
    }
}