using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Profiling;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using UmbracoUnitTesting.BootManager;
using UmbracoUnitTesting.TestWeb.Controllers;

namespace UmbracoUnitTesting.TestWeb.Test.Controllers
{
    [TestClass]
    public class BasicTestUmbracoApiController
    {
        [TestMethod]
        public void BasicApiCallTest()
        {
            //Setup Application Contact with mocks. Sets ApplicaitonContext.Current
            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                new ServiceContext(),
                CacheHelper.CreateDisabledCacheHelper(),
                new ProfilingLogger(
                    Mock.Of<ILogger>(),
                    Mock.Of<IProfiler>()), true);

            //Setup UmbracoContext with mocks. Sets UmbracoContext.Current
            UmbracoContext.EnsureContext(
                Mock.Of<HttpContextBase>(),
                appCtx,
                new Mock<WebSecurity>(null, null).Object,
                Mock.Of<IUmbracoSettingsSection>(),
                Enumerable.Empty<IUrlProvider>(), true);

            var controller = new BasicUmbracoApiController();
            string res =  controller.BasicApiCall();

            Assert.AreEqual(res, string.Empty);
        }

        [TestMethod]
        public void BasicApiDictionaryTest()
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

            //Set ApplicationContext and UmbracoContext

            var test_value = "test";
            var test_key = "Test Key";

            var valueDict = new Dictionary<string, string>() { { test_key, test_value } };

            //we can create a mock of the culture dictionary interface and set any value we want
            var mockDict = new Mock<ICultureDictionary>();
            //setup our mock dictionary to passthrough to our local dictionary
            mockDict.Setup(s => s[It.IsAny<string>()]).Returns<string>(key => valueDict[key]);

            //This time we use the larger constructor for the Helper. In ther we will set the dictionary that we mocked
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(), Mock.Of<ITypedPublishedContentQuery>(),
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section =>
                    section.UrlProviderMode == UrlProviderMode.Auto.ToString()),
                    new[] { Mock.Of<IUrlProvider>() }),
                mockDict.Object, //<--- set the dictionary
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicDictionaryAction(test_key);

            Assert.AreEqual(test_value, res);
        }

        [TestMethod]
        public void BasicApiTypedContentMediaTest()
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

            var contentName = "contentName";
            var mediaName = "mediaName";
            var contentId = 20;
            var mediaId = 30;

            //create some IPublishedContent items
            var mediaItem = BasicHelpers.GetPublishedContentMock(name: mediaName, id: mediaId);
            var contentItem = BasicHelpers.GetPublishedContentMock(name: contentName, id: contentId);

            //we create a mock of the typed query, which is used internally by the Umbraco helper
            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentItem.Object);
            mockedTypedQuery.Setup(s => s.TypedMedia(mediaId)).Returns(mediaItem.Object);

            //give our typed query mock to the longer version of the UmbracoHelper constructor
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                mockedTypedQuery.Object,
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString()), new[] { Mock.Of<IUrlProvider>() }),
                Mock.Of<ICultureDictionary>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicTypedContentMediaAction(contentId, mediaId);

            Assert.AreEqual(contentItem.Object.Name, res.Item1);
            Assert.AreEqual(mediaItem.Object.Name, res.Item2);
        }

        [TestMethod]
        public void BasicApiDynamicContentMediaTest()
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

            var contentName = "contentName";
            var mediaName = "mediaName";
            var contentId = 20;
            var mediaId = 30;

            //create content items to be returned
            var mediaItem = BasicHelpers.GetPublishedContentMock(name: mediaName, id: mediaId);
            var contentItem = BasicHelpers.GetPublishedContentMock(name: contentName, id: contentId);

            //we create a mock of the dynamic query, which is used internally by the Umbraco helper
            var mockedDynamicQuery = new Mock<IDynamicPublishedContentQuery>();
            mockedDynamicQuery.Setup(s => s.Content(contentId)).Returns(contentItem.Object);
            mockedDynamicQuery.Setup(s => s.Media(mediaId)).Returns(mediaItem.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                Mock.Of<ITypedPublishedContentQuery>(),
                mockedDynamicQuery.Object,
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString()), new[] { Mock.Of<IUrlProvider>() }),
                Mock.Of<ICultureDictionary>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicDynamicContentMediaAction(contentId, mediaId);

            Assert.AreEqual(contentItem.Object.Name, res.Item1);
            Assert.AreEqual(mediaItem.Object.Name, res.Item2);
        }

        [TestMethod]
        public void BasicApiTypedSearchActionTest()
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

            var searchTerm = "test";

            //Create some content to be returned by search
            var searchItems = new IPublishedContent[] { BasicHelpers.GetPublishedContent(), BasicHelpers.GetPublishedContent() };

            //mock the search call of the typed query
            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedSearch(searchTerm, true, null)).Returns(searchItems);

            //give our typed query mock to the longer version of the UmbracoHelper constructor
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                mockedTypedQuery.Object,
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString()), new[] { Mock.Of<IUrlProvider>() }),
                Mock.Of<ICultureDictionary>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicTypedSearchAction(searchTerm);

            Assert.AreEqual(searchItems.Count(), res);
        }

        [TestMethod]
        public void BasicApiContentTypeTest()
        {
            //create a mock of the content type service
            var mockContentService = new Mock<IContentTypeService>();
            //this time we will make our own service context, which can take in all of the umbraco services
            //Pass the context the mocked content service object
            var serviceContext = new ServiceContext(contentTypeService: mockContentService.Object);

            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                serviceContext,
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

            var alias = "test_alias";

            //we need to mock a content type composition with our alias
            var mockContentType = new Mock<IContentType>();
            mockContentType.Setup(s => s.Alias).Returns(alias);
            //we are not going to give any property types because this will cause an error down the line
            mockContentType.Setup(s => s.CompositionPropertyTypes).Returns(new PropertyType[] { });

            //PublishedContentType.Get will eventually request a content type from one of the data services (content, media, member)
            //In our case content
            //We tell it which content type object to return
            mockContentService.Setup(s => s.GetContentType(alias)).Returns(mockContentType.Object);

            var ContentType = PublishedContentType.Get(PublishedItemType.Content, alias);

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentMock = BasicHelpers.GetPublishedContentMock();
            contentMock.Setup(s => s.ContentType).Returns(ContentType);

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentMock.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                mockedTypedQuery.Object,
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString()), new[] { Mock.Of<IUrlProvider>() }),
                Mock.Of<ICultureDictionary>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));


            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicContentTypeAction(contentId);

            Assert.AreEqual(alias, res);
        }

        [TestMethod]
        public void BasicApiHasPropertyTest()
        {
            //create a mock of the content type service
            var mockContentService = new Mock<IContentTypeService>();
            //this time we will make our own service context, which can take in all of the umbraco services
            //Pass the context the mocked content service object
            //core boot manager requires Services.TextService to not be null (pass in mock of ILocalizedTextService)
            var serviceContext = new ServiceContext(contentTypeService: mockContentService.Object, localizedTextService: Mock.Of<ILocalizedTextService>());

            var appCtx = ApplicationContext.EnsureContext(
                new DatabaseContext(Mock.Of<IDatabaseFactory>(), Mock.Of<ILogger>(), new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() })),
                serviceContext,
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

            //Have to use an inherited instance of boot manager to remove methods we can't use
            var bm = new CustomBoot(new UmbracoApplication(), serviceContext);
            bm.Initialize().Startup(null).Complete(null);

            string ctAlias = "testAlias";
            string propertyName = "testProp";

            //THIS TIME we do need a property type defined.... this is more complicated...
            var mockContentType = new Mock<IContentType>();
            mockContentType.Setup(s => s.Alias).Returns(ctAlias);
            mockContentType.Setup(s => s.CompositionPropertyTypes).Returns(new PropertyType[] { new PropertyType(propertyName, DataTypeDatabaseType.Nvarchar, propertyName) });

            mockContentService.Setup(s => s.GetContentType(ctAlias)).Returns(mockContentType.Object);

            var ContentType = PublishedContentType.Get(PublishedItemType.Content, ctAlias);

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentMock = new Mock<IPublishedContent>();
            contentMock.Setup(s => s.ContentType).Returns(ContentType);

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentMock.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                mockedTypedQuery.Object,
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString()), new[] { Mock.Of<IUrlProvider>() }),
                Mock.Of<ICultureDictionary>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicHasPropertyAction(contentId, propertyName);

            Assert.IsTrue(res);

            //clean up resolved so we can use this again...
            appCtx.DisposeIfDisposable();
        }

        /// <summary>
        /// Pretty easy one actually, GetProperty is a method directly on the publishecontent interface
        /// </summary>
        [TestMethod]
        public void BasicApiGetPropertyTest()
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

            string propertyName = "testProp";
            string propValue = "testValue";

            var propertyMock = new Mock<IPublishedProperty>();
            propertyMock.Setup(s => s.Value).Returns(propValue);

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentMock = BasicHelpers.GetPublishedContentMock();
            contentMock.Setup(s => s.GetProperty(propertyName)).Returns(propertyMock.Object);

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentMock.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                mockedTypedQuery.Object,
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString()), new[] { Mock.Of<IUrlProvider>() }),
                Mock.Of<ICultureDictionary>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicGetPropertyAction(contentId, propertyName);

            Assert.AreEqual(propValue, res);
        }

        [TestMethod]
        public void BasicApiPositionTest()
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

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentMock = BasicHelpers.GetPublishedContentMock();
            contentMock.Setup(s => s.GetIndex()).Returns(1);

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentMock.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = new UmbracoHelper(ctx,
                Mock.Of<IPublishedContent>(),
                mockedTypedQuery.Object,
                Mock.Of<IDynamicPublishedContentQuery>(),
                Mock.Of<ITagQuery>(),
                Mock.Of<IDataTypeService>(),
                new UrlProvider(ctx, Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == UrlProviderMode.Auto.ToString()), new[] { Mock.Of<IUrlProvider>() }),
                Mock.Of<ICultureDictionary>(),
                Mock.Of<IUmbracoComponentRenderer>(),
                new MembershipHelper(ctx, Mock.Of<MembershipProvider>(), Mock.Of<RoleProvider>()));

            var controller = new BasicUmbracoApiController(ctx, helper);
            var res = controller.BasicPositionAction(contentId);

            //not first
            Assert.IsFalse(res);
        }
    }
}
