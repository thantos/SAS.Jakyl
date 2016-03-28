using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UmbracoUnitTesting.TestWeb.Controllers
{
    public class BasicRenderMvcController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            return CurrentTemplate(model.Content.Name);
        }
    }
}