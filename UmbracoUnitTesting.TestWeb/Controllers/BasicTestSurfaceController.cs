using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace UmbracoUnitTesting.TestWeb.Controllers
{
    public class BasicTestSurfaceController : SurfaceController
    {
        public PartialViewResult BasicTestAction()
        {
            return PartialView(new { });
        }
    }
}