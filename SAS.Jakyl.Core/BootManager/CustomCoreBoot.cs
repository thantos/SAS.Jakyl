using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace SAS.Jakyl.Core.BootManager
{
    public class CustomBoot : CoreBootManager, IDisposable
    {
        private readonly ServiceContext _servContext;
        public bool Initialized { get; private set; }
        public bool Started { get; private  set; }
        public bool Completed { get; private set; }

        public CustomBoot(UmbracoApplication app, ServiceContext context) : base(app)
        {
            _servContext = context;
        }

        public override IBootManager Initialize()
        {
            this.Initialized = true;
            return base.Initialize();
        }

        protected override ServiceContext CreateServiceContext(DatabaseContext dbContext, IDatabaseFactory dbFactory)
        {
            return _servContext;
        }

        public override IBootManager Startup(Action<ApplicationContext> afterStartup)
        {
            this.Started = true;
            return base.Startup(afterStartup);
        }

        public override IBootManager Complete(Action<ApplicationContext> afterComplete)
        {
            FreezeResolution();
            this.Completed = true;
            return this;
        }

        public void Dispose()
        {
            this.Initialized = this.Started = this.Completed = false;//might need to expand on this..
        }
    }
}
