﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace SAS.Jakyl.TestWeb.Controllers
{
    public class BasicRenderMvcController : RenderMvcController
    {
        public BasicRenderMvcController() { }
        public BasicRenderMvcController(UmbracoContext context, UmbracoHelper helper) : base(context, helper) { }

        public override ActionResult Index(RenderModel model)
        {
            return CurrentTemplate(model.Content.Name);
        }

        public PartialViewResult BasicGetSecurityAction()
        {
            return PartialView(Security.CurrentUser);
        }

        public PartialViewResult BasicIsAuthenticatedAction()
        {
            return PartialView(Security.IsAuthenticated());
        }

        public PartialViewResult BasicGetUserIdAction()
        {
            return PartialView(Security.GetUserId());
        }

    }
}