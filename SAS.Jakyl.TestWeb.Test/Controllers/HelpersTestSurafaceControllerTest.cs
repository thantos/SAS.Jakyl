using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using SAS.Jakyl.TestWeb.Controllers;
using SAS.Jakyl.Core;

namespace SAS.Jakyl.TestWeb.Test.Controllers
{
    [TestClass]
    public class HelpersTestSurafaceControllerTest
    {
        [TestMethod]
        public void HelperActionTest()
        {
            //Setup UmbracoContext with mocks. Sets UmbracoContext.Current
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var controller = new BasicTestSurfaceController();
            var res = controller.BasicTestAction();
            var model = res.Model as object;

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void HelperCurrentPageTest()
        {

            //Setup UmbracoContext with mocks. Sets UmbracoContext.Current
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var content = new TestPublishedContent() { Name = "test" };

            var controller = new BasicTestSurfaceController();
            //Setting the controller context will provide the route data, route def, publushed content request, and current page to the surface controller
            controller.ControllerContext = UmbracoUnitTestHelper.GetControllerContext(ctx, controller, UmbracoUnitTestHelper.GetPublishedContentRequest(ctx, currentContent: content));

            var res = controller.BasicCurrentPageAction();
            var model = res.Model as string;

            Assert.AreEqual(content.Name, model);
        }

        [TestMethod]
        public void HelperPublishedContent1Test()
        {
            //Setup UmbracoContext with mocks. Sets UmbracoContext.Current
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();
            //create an instance of our test implemention of IPublisheContent
            var content = new TestPublishedContent() { Name = "test" };
            //setup a helper object which will be given to the surface controller
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(context: ctx, content: content);
            //we use a surface controller that takes in th context and helper so that we can setup them for our needs
            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicPublishedContentAction();
            var model = res.Model as string;

            Assert.AreEqual(content.Name, model);

        }

        [TestMethod]
        public void HelperPublishedContent2Test()
        {
            //Setup UmbracoContext with mocks. Sets UmbracoContext.Current
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();
            //instead of using the implemenation of IPublishedContent, we mock IPublishedContent
            var mockContent = UmbracoUnitTestHelper.GetPublishedContentMock(name: "test").Object;
            //give our content to the umbraco helper which will be given to the controller
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, content: mockContent);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicPublishedContentAction();
            var model = res.Model as string;

            Assert.IsNotNull(mockContent.Name, model);
        }

        [TestMethod]
        public void HelperPublishedContent3Test()
        {
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var mockContent = UmbracoUnitTestHelper.GetPublishedContentMock(name: "test");

            UmbracoUnitTestHelper.SetPublishedContentRequest(ctx, UmbracoUnitTestHelper.GetPublishedContentRequest(ctx, currentContent: mockContent.Object));

            var res = new BasicTestSurfaceController().BasicPublishedContentAction();
            var model = res.Model as string;

            Assert.AreEqual(mockContent.Object.Name, model);
        }

        [TestMethod]
        public void HelperDictionaryTest()
        {
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var valueDict = new Dictionary<string, string>() { { "Test Key", "test" } };

            //we can create a mock of the culture dictionary interface and set any value we want
            var mockDict = new Mock<ICultureDictionary>();
            //setup our mock dictionary to passthrough to our local dictionary
            mockDict.Setup(s => s[It.IsAny<string>()]).Returns<string>(key => valueDict[key]);

            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, cultureDictionary: mockDict.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicDictionaryAction();
            var model = res.Model as string;

            Assert.AreEqual(valueDict["Test Key"], model);
        }

        [TestMethod]
        public void HelperTypedContentMediaTest()
        {
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var contentName = "contentName";
            var mediaName = "mediaName";
            var contentId = 20;
            var mediaId = 30;

            //create some IPublishedContent items
            var mediaItem = UmbracoUnitTestHelper.GetPublishedContentMock(name: mediaName, id: mediaId);
            var contentItem = UmbracoUnitTestHelper.GetPublishedContentMock(name: contentName, id: contentId);

            //we create a mock of the typed query, which is used internally by the Umbraco helper
            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentItem.Object);
            mockedTypedQuery.Setup(s => s.TypedMedia(mediaId)).Returns(mediaItem.Object);

            //give our typed query mock to the longer version of the UmbracoHelper constructor
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, typedQuery: mockedTypedQuery.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicTypedContentMediaAction(contentId, mediaId);
            var model = res.Model as Tuple<string, string>;

            Assert.AreEqual(contentItem.Object.Name, model.Item1);
            Assert.AreEqual(mediaItem.Object.Name, model.Item2);
        }

        [TestMethod]
        public void HelperDynamicContentMediaTest()
        {
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

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
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, dynamicQuery: mockedDynamicQuery.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicDynamicContentMediaAction(contentId, mediaId);
            var model = res.Model as Tuple<string, string>;

            Assert.AreEqual(contentItem.Object.Name, model.Item1);
            Assert.AreEqual(mediaItem.Object.Name, model.Item2);
        }

        [TestMethod]
        public void HelperTypedSearchActionTest()
        {
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var searchTerm = "test";

            //Create some content to be returned by search
            var searchItems = new IPublishedContent[] { BasicHelpers.GetPublishedContent(), BasicHelpers.GetPublishedContent() };

            //mock the search call of the typed query
            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedSearch(searchTerm, true, null)).Returns(searchItems);

            //give our typed query mock to the longer version of the UmbracoHelper constructor
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, typedQuery: mockedTypedQuery.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicTypedSearchAction(searchTerm);
            var model = (int)res.Model;

            Assert.AreEqual(searchItems.Count(), model);
        }

        [TestMethod]
        public void HelperContentTypeTest()
        {
            var mocks = new MockServiceContext();

            var appCtx = UmbracoUnitTestHelper.GetApplicationContext(serviceContext: mocks.ServiceContext);

            var ctx = UmbracoUnitTestHelper.GetUmbracoContext(appCtx);

            //pass in emtpy proprty types to avoid uninitialized resolver issue. To bypass, must use CoreBootManager
            UmbracoUnitTestHelper.SetupServicesForPublishedContentTypeResolution(mocks, new PropertyType[] { });

            var alias = "test_alias";
            var contentType = UmbracoUnitTestHelper.GetPublishedContentType(alias: alias);

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentMock = UmbracoUnitTestHelper.GetPublishedContentMock(contentType: contentType);

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(It.IsAny<int>())).Returns(contentMock.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, typedQuery: mockedTypedQuery.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicContentTypeAction(contentId);
            var model = (string)res.Model;

            Assert.AreEqual(alias, model);
        }

        [TestMethod]
        public void HelperHasPropertyTest()
        {
            //Uses our special service context object (it mocks all services!!)
            var mockServiceContext = new MockServiceContext();

            var appCtx = UmbracoUnitTestHelper.GetApplicationContext(serviceContext: mockServiceContext.ServiceContext);
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext(appCtx);

            UmbracoUnitTestHelper.StartCoreBootManager( UmbracoUnitTestHelper.GetCustomBootManager(serviceContext: mockServiceContext.ServiceContext));

            string propertyName = "testProp";

            //THIS TIME we do need a property type defined.... this is more complicated...
            UmbracoUnitTestHelper.SetupServicesForPublishedContentTypeResolution(mockServiceContext, new[] { UmbracoUnitTestHelper.GetPropertyType(alias: propertyName) });

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentType = UmbracoUnitTestHelper.GetPublishedContentType();
            var contentMock = UmbracoUnitTestHelper.GetPublishedContentMock(contentType:contentType);

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentMock.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, typedQuery: mockedTypedQuery.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicHasPropertyAction(contentId, propertyName);
            var model = (bool)res.Model;

            Assert.IsTrue(model);

            //clean up resolved so we can use this again...
            UmbracoUnitTestHelper.CleanupCoreBootManager(appCtx);
        }

        /// <summary>
        /// Pretty easy one actually, GetProperty is a method directly on the publishecontent interface
        /// </summary>
        [TestMethod]
        public void HelperGetPropertyTest()
        {
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentMock = UmbracoUnitTestHelper.GetPublishedContentMock( properties: new[] { UmbracoUnitTestHelper.GetPublishedProperty(value: "testValue", alias: "testProp") } );

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentMock.Object);
                
            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, typedQuery: mockedTypedQuery.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicGetPropertyAction(contentId, contentMock.Object.Properties.First().PropertyTypeAlias);
            var model = (string)res.Model;

            Assert.AreEqual(contentMock.Object.Properties.First().Value, model);
        }

        [TestMethod]
        public void HelperPositionTest()
        {
            var ctx = UmbracoUnitTestHelper.GetUmbracoContext();

            var contentId = 2;
            //get a mocked IPublishedContent
            var contentMock = UmbracoUnitTestHelper.GetPublishedContentMock(index: 1);

            var mockedTypedQuery = new Mock<ITypedPublishedContentQuery>();
            mockedTypedQuery.Setup(s => s.TypedContent(contentId)).Returns(contentMock.Object);

            //give our dynamic query mock to the longer version of the UmbracoHelper constructor
            var helper = UmbracoUnitTestHelper.GetUmbracoHelper(ctx, typedQuery: mockedTypedQuery.Object);

            var controller = new BasicTestSurfaceController(ctx, helper);
            var res = controller.BasicPositionAction(contentId);
            var model = (bool)res.Model;

            Assert.IsFalse(model);
        }


    }
}
