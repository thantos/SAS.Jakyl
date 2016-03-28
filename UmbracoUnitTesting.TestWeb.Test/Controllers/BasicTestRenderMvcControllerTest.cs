﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Profiling;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using UmbracoUnitTesting.TestWeb.Controllers;
using UmbracoUnitTesting.TestWeb.Test.TestClasses;

namespace UmbracoUnitTesting.TestWeb.Test.Controllers
{
    [TestClass]
    public class BasicTestRenderMvcControllerTest
    {
        [TestMethod]
        public void BasicCurrentTemplateTest()
        {
            var routeData = new RouteData();

            routeData.Values.Add("action", "test");
            routeData.Values.Add("controller", "test");

            var httpContext = new Mock<HttpContextBase>();

            httpContext.Setup(s => s.Items).Returns(new Dictionary<string, string>());

            ViewEngines.Engines.RemoveAll((s) => true);
            ViewEngines.Engines.Add(new TestViewEngine());

            //Setup Application Contact with mocks. Sets ApplicaitonContext.Current
            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            //Setup UmbracoContext with mocks. Sets UmbracoContext.Current
            var ctx = UmbracoContext.EnsureContext(
                httpContext.Object,
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            var content = new Mock<IPublishedContent>();
            content.Setup(s => s.Name).Returns("test");

            //setup published content request. This sets the current content on the Umbraco Context and will be used later
            ctx.PublishedContentRequest = new PublishedContentRequest(new Uri("http://test.com"), ctx.RoutingContext,
                Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.AutoLegacy.ToString()),
                s => new string[] { })
            {
                PublishedContent = content.Object
            };

            var renderModel = new RenderModel(content.Object);

            //The reoute definition will contain the current page request object and be passed into the route data
            var routeDefinition = new RouteDefinition
            {
                PublishedContentRequest = ctx.PublishedContentRequest
            };

            //We create a route data object to be given to the Controller context
            routeData.DataTokens.Add("umbraco-route-def", routeDefinition);

            var controller = new BasicRenderMvcController();

            //Setting the controller context will provide the route data, route def, publushed content request, and current page to the surface controller
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(ctx.HttpContext, routeData, controller);

            var res = controller.Index(renderModel) as ViewResult;
            var model = res.Model as string;

            Assert.AreEqual(model, content.Object.Name);
        }
    }
}
