using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAS.Jakyl;
using SAS.Jakyl.TestWeb.Controllers;
using SAS.Jakyl.Core;

namespace SAS.Jakyl.TestWeb.Test.Controllers
{

    [TestClass]
    public class EngineTestSurfaceControllerTest
    {

        private UmbracoUnitTestEngine _unitTestEngine;

        [TestInitialize]
        public void start()
        {
            _unitTestEngine = new UmbracoUnitTestEngine();
        }

        [TestMethod]
        public void EngineActionTest()
        {
            //Now we can run a basic situation with no Umbraco boilerplate!
            var controller = new BasicTestSurfaceController();
            var res = controller.BasicTestAction();
            var model = res.Model as object;

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void EngineDictionaryTest()
        {
            var key = "Test Key";
            var value = "test";
            _unitTestEngine.WithDictionaryValue(key, value);

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);
            var res = controller.BasicDictionaryAction();
            var model = res.Model as string;

            Assert.AreEqual(value, model);
        }

        [TestMethod]
        public void EngineCurrentPageTest()
        {

            var content = _unitTestEngine.WithCurrentPage("Test");

            var controller = new BasicTestSurfaceController();
            _unitTestEngine.RegisterController(controller);
            var res = controller.BasicCurrentPageAction();
            var model = res.Model as string;

            Assert.AreEqual(content.Name, model);
        }

        [TestMethod]
        public void EnginePublishedContent1Test()
        {
            var content = _unitTestEngine.WithCurrentPage("Test");
            //Setup UmbracoContext with mocks. Sets UmbracoContext.Current
            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicPublishedContentAction();
            var model = res.Model as string;

            Assert.AreEqual(content.Name, model);
        }

        //content 2 and content 2 tests in basic and helper are preformed the same way

        [TestMethod]
        public void EngineTypedContentMediaTest()
        {
            //Auto fixture will take care of the name and unit test engine will assign an ID
            var content = _unitTestEngine.WithPublishedContentPage();
            var media = _unitTestEngine.WithPublishedMedia();

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicTypedContentMediaAction(content.Id, media.Id);
            var model = res.Model as Tuple<string, string>;

            Assert.AreEqual(content.Name, model.Item1);
            Assert.AreEqual(media.Name, model.Item2);
        }

        [TestMethod]
        public void EngineDynamicContentMediaTest()
        {
            //Auto fixture will take care of the name and unit test engine will assign an ID
            var content = _unitTestEngine.WithPublishedContentPage();
            var media = _unitTestEngine.WithPublishedMedia();

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicDynamicContentMediaAction(content.Id, media.Id);
            var model = res.Model as Tuple<string, string>;

            Assert.AreEqual(content.Name, model.Item1);
            Assert.AreEqual(media.Name, model.Item2);
        }

        [TestMethod]
        public void EngineContentTypeTest()
        {
            var content = _unitTestEngine.WithPublishedContentPage(contentType: _unitTestEngine.WithPublishedContentType());
                   
            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);
            var res = controller.BasicContentTypeAction(content.Id);
            var model = (string)res.Model;

            Assert.AreEqual(content.ContentType.Alias, model);
        }

        [TestMethod]
        public void EngineHasPropertyTest()
        {
            string propertyName = "testProp";
            var content = _unitTestEngine.WithPublishedContentPage(contentType: _unitTestEngine.WithPublishedContentType(propertyTypes: new[] { UmbracoUnitTestHelper.GetPropertyType(alias: propertyName) }));

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);
            var res = controller.BasicHasPropertyAction(content.Id, propertyName);
            var model = (bool)res.Model;

            Assert.IsTrue(model);
        }

        /// <summary>
        /// Pretty easy one actually, GetProperty is a method directly on the publishecontent interface
        /// </summary>
        [TestMethod]
        public void EngineGetPropertyTest()
        {
            var value = "testValue";
            var alias = "testAlias";
            var content = _unitTestEngine.WithPublishedContentPage(properties: new[] { UmbracoUnitTestHelper.GetPublishedProperty(value: value, alias: alias) });

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicGetPropertyAction(content.Id, content.Properties.First().PropertyTypeAlias);
            var model = (string)res.Model;

            Assert.AreEqual(content.Properties.First().Value, model);
        }


        [TestMethod]
        public void EnginePositionTest()
        {
            var content = _unitTestEngine.WithPublishedContentPage(index: 1);

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.BasicPositionAction(content.Id);
            var model = (bool)res.Model;

            Assert.IsFalse(model);
        }

        [TestMethod]
        public void EngineRelationChildTest()
        {
            var relation = _unitTestEngine.WithRelation();

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.RelationChildAction(relation.ChildId);
            var model = (int)res.Model;

            Assert.AreEqual(relation.Id, model);
        }

        [TestMethod]
        public void EngineRelationParentTest()
        {
            var relation = _unitTestEngine.WithRelation();

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.RelationParentAction(relation.ParentId);
            var model = (int)res.Model;

            Assert.AreEqual(relation.Id, model);
        }

        [TestMethod]
        public void EngineRelationTypeAliasTest()
        {
            var relation = _unitTestEngine.WithRelation();

            var controller = new BasicTestSurfaceController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            var res = controller.RelationAliasAction(relation.RelationType.Alias);
            var model = (int)res.Model;

            Assert.AreEqual(relation.Id, model);
        }

        [TestCleanup]
        public void clean()
        {
            if (_unitTestEngine != null)
                _unitTestEngine.Dispose();
        }

    }
}
