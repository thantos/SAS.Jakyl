using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Models;
using UmbracoUnitTesting.Engine;
using UmbracoUnitTesting.TestWeb.Controllers;

namespace UmbracoUnitTesting.TestWeb.Test.Controllers
{

    [TestClass]
    public class EngineTestRenderMVCControllerTest
    {

        private UmbracoUnitTestEngine _unitTestEngine;

        [TestInitialize]
        public void start()
        {
            _unitTestEngine = new UmbracoUnitTestEngine();
        }

        [TestMethod]
        public void EngineCurrentTemplateTest()
        {
            var content = _unitTestEngine.WithCurrentPage();
            _unitTestEngine.WithCurrentTemplate();

            var renderModel = new RenderModel(content);

            var controller = new BasicRenderMvcController();
            _unitTestEngine.RegisterController(controller);

            var res = controller.Index(renderModel) as ViewResult;
            var model = res.Model as string;

            Assert.AreEqual(model, content.Name);
        }

        [TestMethod]
        public void EngineGetSecurityTest()
        {
            _unitTestEngine.WithCurrentUser();

            var controller = new BasicRenderMvcController(); //don't really care about the helper here

            _unitTestEngine.RegisterController(controller);

            var model = controller.BasicGetSecurityAction();

            Assert.IsNotNull(model);
        }

        [TestCleanup]
        public void clean()
        {
            if (_unitTestEngine != null)
                _unitTestEngine.Dispose();
        }

    }
}
