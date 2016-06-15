using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAS.Jakyl;
using SAS.Jakyl.TestWeb.Controllers;
using SAS.Jakyl.Core;

namespace SAS.Jakyl.TestWeb.NuGetTest
{

    [TestClass]
    public class NuGetEngineTestSurfaceControllerTest
    {

        private UmbracoUnitTestEngine _unitTestEngine;

        [TestInitialize]
        public void start()
        {
            _unitTestEngine = new UmbracoUnitTestEngine();
        }

        [TestMethod]
        public void NuGetEngineActionTest()
        {
            //Now we can run a basic situation with no Umbraco boilerplate!
            var controller = new BasicTestSurfaceController();
            var res = controller.BasicTestAction();
            var model = res.Model as object;

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void NuGetEngineDictionaryTest()
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

        [TestCleanup]
        public void clean()
        {
            if (_unitTestEngine != null)
                _unitTestEngine.Dispose();
        }

    }
}
