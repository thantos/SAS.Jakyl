using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Profiling;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Dictionary;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using UmbracoUnitTesting.TestWeb.Controllers;
using UmbracoUnitTesting.TestWeb.Test.TestClasses;

namespace UmbracoUnitTesting.TestWeb.Test.Controllers
{
    [TestClass]
    public class BasicTestSurfaceControllerTest
    {
        [TestMethod]
        public void BasicActionTest()
        {
            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            var controller = new BasicTestSurfaceController();
            var res = controller.BasicTestAction();
            var model = res.Model as object;

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void BasicCurrentPageTest()
        {
            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            var ctx = UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            string test_name = "test";

            var content = new TestPublishedContent() { Name = test_name };

            ctx.PublishedContentRequest = new PublishedContentRequest(new Uri("http://test.com"), ctx.RoutingContext,
                Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.AutoLegacy.ToString()),
                s => new string[] { })
            {
                PublishedContent = content
            };

            var routeDefinition = new RouteDefinition
            {
                PublishedContentRequest = ctx.PublishedContentRequest
            };

            var routeData = new RouteData();
            routeData.DataTokens.Add("umbraco-route-def", routeDefinition);

            var controller = new BasicTestSurfaceController();
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(ctx.HttpContext, routeData, controller);

            var res = controller.BasicCurrentPageAction();
            var model = res.Model as string;

            Assert.AreEqual(test_name, model);
        }

        [TestMethod]
        public void BasicPublishedContent1Test()
        {

            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            var ctx = UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            var test_name = "test";

            var content = new TestPublishedContent() { Name = test_name };
            var helper = new UmbracoHelper(ctx, content);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicPublishedContentAction();
            var model = res.Model as string;

            Assert.AreEqual(test_name, model);

        }

        [TestMethod]
        public void BasicPublishedContent2Test()
        {

            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            var ctx = UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            var test_name = "test";

            var mockContent = new Mock<IPublishedContent>();
            mockContent.Setup(s => s.Name).Returns(test_name);

            var helper = new UmbracoHelper(ctx, mockContent.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicPublishedContentAction();
            var model = res.Model as string;

            Assert.IsNotNull(test_name, model);
        }

        [TestMethod]
        public void BasicPublishedContent3Test()
        {

            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            var ctx = UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            var test_name = "test";

            var mockContent = BasicHelpers.GetPublishedContentMock(test_name);

            ctx.PublishedContentRequest = new PublishedContentRequest(new Uri("http://test.com"), ctx.RoutingContext,
                Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.AutoLegacy.ToString()),
                s => new string[] { })
            {
                PublishedContent = mockContent.Object
            };

            var res = new BasicTestSurfaceController().BasicPublishedContentAction();
            var model = res.Model as string;

            Assert.AreEqual(test_name, model);
        }

        [TestMethod]
        public void BasicDictionaryTest()
        {

            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            var ctx = UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            var test_name = "test";

            var valueDict = new Dictionary<string, string>() { { "Test Key", test_name } };

            var mockDict = new Mock<ICultureDictionary>();
            mockDict.Setup(s => s[It.IsAny<string>()]).Returns<string>(key => valueDict[key]);

            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                Mock.Of<ITypedPublishedContentQuery>(),
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString() ), new[] { Mock.Of<IUrlProvider>() }),
                mockDict.Object, //<--- set the dictionary
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicDictionaryAction();
            var model = res.Model as string;

            Assert.AreEqual(test_name, model);
        }
    }
}
