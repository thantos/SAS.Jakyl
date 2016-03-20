using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace UmbracoUnitTesting.BootManager
{
    public class CustomBoot : CoreBootManager
    {
        private readonly ServiceContext _servContext;

        public CustomBoot(UmbracoApplication app, ServiceContext context) : base(app)
        {
            _servContext = context;
        }

        protected override ServiceContext CreateServiceContext(DatabaseContext dbContext, IDatabaseFactory dbFactory)
        {
            return _servContext;
        }

        public override IBootManager Complete(Action<ApplicationContext> afterComplete)
        {
            FreezeResolution();

            return this;
        }
    }
}
