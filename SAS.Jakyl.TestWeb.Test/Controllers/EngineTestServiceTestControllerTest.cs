using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAS.Jakyl;
using SAS.Jakyl.TestWeb.Controllers;
using SAS.Jakyl.Core;
using Umbraco.Core.Models;
using Moq;

namespace SAS.Jakyl.TestWeb.Test.Controllers
{

    [TestClass]
    public class EngineTestServiceTestControllerTest
    {

        private UmbracoUnitTestEngine _unitTestEngine;

        [TestInitialize]
        public void start()
        {
            _unitTestEngine = new UmbracoUnitTestEngine();
        }

        [TestMethod]
        public void EngineContentGetByIdTest()
        {
            var id = 1;

            var content = new Mock<IContent>();

            _unitTestEngine.mockServiceContext.ContentService.Setup(s => s.GetById(id)).Returns(content.Object);

            var controller = new ServicesTestController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);

            var res = controller.ContentGetById(id);

            Assert.AreSame(content.Object, res);
        }

        [TestMethod]
        public void EngineContentSetValueTest()
        {
            var id = 1;
            var prop = "testProp";
            var value = "testVal";

            var content = new Mock<IContent>();

            _unitTestEngine.mockServiceContext.ContentService.Setup(s => s.GetById(id)).Returns(content.Object);
            content.Setup(s => s.SetValue(prop, value)).Verifiable("Must set a value");

            var controller = new ServicesTestController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);

            controller.ContentSetValue(id, prop, value);

            content.Verify();
        }

        [TestMethod]
        public void EngineContentCreateContentTest()
        {
            var name = "testName";
            var alias = "testAlias";

            var content = new Mock<IContent>();

            _unitTestEngine.mockServiceContext.ContentService.Setup(s => s.CreateContent(name, null, alias, 0)).Returns(content.Object).Verifiable("Must create content");

            var controller = new ServicesTestController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);

            var res = controller.ContentCreateContent(name,alias);

            Assert.AreSame(content.Object, res);

            _unitTestEngine.mockServiceContext.ContentService.Verify();
        }

        [TestMethod]
        public void EngineContentSaveAndPublishTest()
        {
            var id = 1;

            var content = new Mock<IContent>();

            _unitTestEngine.mockServiceContext.ContentService.Setup(s => s.GetById(id)).Returns(content.Object);
            _unitTestEngine.mockServiceContext.ContentService.Setup(s => s.SaveAndPublishWithStatus(content.Object, 0, true)).Verifiable("Must save");

            var controller = new ServicesTestController(_unitTestEngine.UmbracoContext, _unitTestEngine.UmbracoHelper);
            _unitTestEngine.RegisterController(controller);

            controller.ContentSaveAndPublishWithStatus(id);

            content.Verify();
        }


        [TestCleanup]
        public void clean()
        {
            if (_unitTestEngine != null)
                _unitTestEngine.Dispose();
        }

    }
}
