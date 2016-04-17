using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmbracoUnitTesting.Engine;
using UmbracoUnitTesting.TestWeb.Controllers;

namespace UmbracoUnitTesting.TestWeb.Test.Controllers
{

    [TestClass]
    public class EngineTestUmbracoApiControllerTest
    {

        private UmbracoUnitTestEngine _unitTestEngine;

        [TestInitialize]
        public void start()
        {
            _unitTestEngine = new UmbracoUnitTestEngine();
        }

        [TestMethod]
        public void EngineApiActionTest()
        {
            //Now we can run a basic situation with no Umbraco boilerplate!
            var controller = new BasicUmbracoApiController();
            var res = controller.BasicApiCall();

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void EngineApiDictionaryTest()
        {
            var key = "Test Key";
            var value = "test";
            _unitTestEngine.WithDictionaryValue(key, value);

            var controller = new BasicUmbracoApiController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);
            var res = controller.BasicDictionaryAction(key);

            Assert.AreEqual(value, res);
        }

        [TestMethod]
        public void EngineApiTypedContentMediaTest()
        {
            //Auto fixture will take care of the name and unit test EngineApi will assign an ID
            var content = _unitTestEngine.WithPublishedContentPage();
            var media = _unitTestEngine.WithPublishedMedia();

            var controller = new BasicUmbracoApiController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicTypedContentMediaAction(content.Id, media.Id);

            Assert.AreEqual(content.Name, res.Item1);
            Assert.AreEqual(media.Name, res.Item2);
        }

        [TestMethod]
        public void EngineApiDynamicContentMediaTest()
        {
            //Auto fixture will take care of the name and unit test EngineApi will assign an ID
            var content = _unitTestEngine.WithPublishedContentPage();
            var media = _unitTestEngine.WithPublishedMedia();

            var controller = new BasicUmbracoApiController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicDynamicContentMediaAction(content.Id, media.Id);

            Assert.AreEqual(content.Name, res.Item1);
            Assert.AreEqual(media.Name, res.Item2);
        }

        [TestMethod]
        public void EngineApiContentTypeTest()
        {
            var content = _unitTestEngine.WithPublishedContentPage(contentType: _unitTestEngine.WithPublishedContentType());
                   
            var controller = new BasicUmbracoApiController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);
            var res = controller.BasicContentTypeAction(content.Id);

            Assert.AreEqual(content.ContentType.Alias, res);
        }

        [TestMethod]
        public void EngineApiHasPropertyTest()
        {
            string propertyName = "testProp";
            var content = _unitTestEngine.WithPublishedContentPage(contentType: _unitTestEngine.WithPublishedContentType(propertyTypes: new[] { UmbracoUnitTestHelper.GetPropertyType(alias: propertyName) }));

            var controller = new BasicUmbracoApiController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);
            var res = controller.BasicHasPropertyAction(content.Id, propertyName);

            Assert.IsTrue(res);
        }

        /// <summary>
        /// Pretty easy one actually, GetProperty is a method directly on the publishecontent interface
        /// </summary>
        [TestMethod]
        public void EngineApiGetPropertyTest()
        {
            var value = "testValue";
            var alias = "testAlias";
            var content = _unitTestEngine.WithPublishedContentPage(properties: new[] { UmbracoUnitTestHelper.GetPublishedProperty(value: value, alias: alias) });

            var controller = new BasicUmbracoApiController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicGetPropertyAction(content.Id, content.Properties.First().PropertyTypeAlias);

            Assert.AreEqual(content.Properties.First().Value, res);
        }


        [TestMethod]
        public void EngineApiPositionTest()
        {
            var content = _unitTestEngine.WithPublishedContentPage(index: 1);

            var controller = new BasicUmbracoApiController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicPositionAction(content.Id);

            Assert.IsFalse(res);
        }

        [TestCleanup]
        public void clean()
        {
            if (_unitTestEngine != null)
                _unitTestEngine.Dispose();
        }

    }
}
