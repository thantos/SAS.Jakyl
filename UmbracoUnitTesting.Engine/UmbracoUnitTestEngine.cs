using Moq;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;
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
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using Umbraco.Web.WebApi;
using UmbracoUnitTesting.BootManager;
using UmbracoUnitTesting.ViewEngine;

namespace UmbracoUnitTesting.Engine
{
    public class UmbracoUnitTestEngine : IDisposable
    {
        private Random _rand = new Random();
        private HashSet<int> contentIdCollection = new HashSet<int>();
        private HashSet<Action> ControllerActions = new HashSet<Action>();

        private readonly List<IPublishedContent> Content = new List<IPublishedContent>();
        private readonly List<IPublishedContent> Media = new List<IPublishedContent>();
        private readonly List<PublishedContentType> ContentTypes = new List<PublishedContentType>();
        private readonly List<PublishedContentType> MediaTypes = new List<PublishedContentType>();

        private readonly CustomBoot _boot;

        private readonly MockContainer _mocks;

        private readonly MockServiceContext mockServiceContext;

        private UmbracoHelper _umbHelper;
        private RouteData _routeData;
        private HttpRouteData _httpRouteData;
        private PublishedContentRequest _publishedContentRequest;

        private readonly bool EnforceUniqueContentIds;

        private bool _viewEngineCleared;

        public UmbracoUnitTestEngine(Fixture autoFixture = null, bool EnforceUniqueContentIds = true)
        {
            _Fixture = autoFixture ?? new Fixture();

            Content = new List<IPublishedContent>();
            Media = new List<IPublishedContent>();

            _mocks = new MockContainer();
            mockServiceContext = new MockServiceContext();

            this.ApplicationContext = UmbracoUnitTestHelper.GetApplicationContext(serviceContext: this.ServiceContext,
                logger: _mocks.ResolveObject<ILogger>());

            this.UmbracoContext = UmbracoUnitTestHelper.GetUmbracoContext(ApplicationContext, httpContext: _mocks.ResolveObject<HttpContextBase>()
                ,webRoutingSettings: _mocks.ResolveObject<IWebRoutingSection>());

            this.EnforceUniqueContentIds = EnforceUniqueContentIds;

            _boot = UmbracoUnitTestHelper.GetCustomBootManager(serviceContext: ServiceContext);
        }

        #region Properties
        public Fixture _Fixture { get; private set; }
        public ServiceContext ServiceContext { get { return this.mockServiceContext.ServiceContext; } }
        public ApplicationContext ApplicationContext { get; private set; }
        public UmbracoContext UmbracoContext { get; private set; }
        public Controller Controller { get; private set; }
        public ApiController ApiController { get; private set; }
        public bool HasAnyController { get { return HasMvcController || HasApiController; } }
        public bool HasMvcController { get { return Controller != null; } }
        public bool HasApiController { get { return ApiController != null; } }
        public UmbracoHelper UmbracoHelper { get { return NeedsUmbracoHelper(); } }
        public IPublishedContent CurrentPage { get { return _mocks.ResolveObject<IPublishedContent>("Current"); } }
        #endregion

        public UmbracoHelper WithUmbracoHelper()
        {
            return NeedsUmbracoHelper();
        }

        public void WithDictionaryValue(string key, string value)
        {
            NeedsUmbracoHelper();
            _mocks.Resolve<ICultureDictionary>().Setup(s => s[key]).Returns(value);
        }

        public IPublishedContent WithCurrentPage(string name = null, int? id = null, string path = null, string url = null, int? templateId = null, DateTime? updateDate = null, DateTime? createDate = null, PublishedContentType contentType = null, IPublishedContent parent = null, IEnumerable<IPublishedContent> Children = null, IEnumerable<IPublishedProperty> properties = null, int? index = null)
        {
            var content = WithPublishedContentPage(_mocks.Resolve<IPublishedContent>("Current"), name, id, path, url,
                templateId, updateDate, createDate
                , contentType, parent, Children, properties, index);

            NeedsPublishedContentRequest(); //Might want to removed, only needed with controller context, which calls this too... needs to have the published content request
            AffectsController(true, GiveControllerContext); //will make sure a controller has the controller context and current page

            return content;
        }

        public IPublishedContent WithPublishedContentPage(Mock<IPublishedContent> mock = null, string name = null, int? id = null, string path = null, string url = null, int? templateId = null, DateTime? updateDate = null, DateTime? createDate = null, PublishedContentType contentType = null, IPublishedContent parent = null, IEnumerable<IPublishedContent> Children = null, IEnumerable<IPublishedProperty> properties = null, int? index = null)
        {

            var contentMock = UmbracoUnitTestHelper.SetPublishedContentMock(
                mock ?? new Mock<IPublishedContent>(),
                name ?? _Fixture.Create<string>(),
                ResolveUnqueContentId(id), path, url ?? _Fixture.Create<string>(),
                templateId, updateDate, createDate
                , contentType, parent, Children, properties, index);
            var content = contentMock.Object;

            Content.Add(content);

            return content;
        }

        public IPublishedContent WithPublishedMedia(Mock<IPublishedContent> mock = null, string name = null, int? id = null, string path = null, string url = null, int? templateId = null, DateTime? updateDate = null, DateTime? createDate = null, PublishedContentType contentType = null, IPublishedContent parent = null, IEnumerable<IPublishedContent> Children = null, IEnumerable<IPublishedProperty> properties = null, int? index = null)
        {

            var contentMock = UmbracoUnitTestHelper.SetPublishedContentMock(
                mock ?? new Mock<IPublishedContent>(),
                name ?? _Fixture.Create<string>(),
                ResolveUnqueContentId(id), path, url ?? _Fixture.Create<string>(),
                templateId, updateDate, createDate
                , contentType, parent, Children, properties, index, PublishedItemType.Media);
            var content = contentMock.Object;

            Media.Add(content);

            return content;
        }

        public void WithCurrentTemplate(string action = null, string controller = null)
        {
            var routeData = NeedsRouteData();
            routeData.Values.Add("action", action ?? _Fixture.Create<string>());
            routeData.Values.Add("controller", controller ?? _Fixture.Create<string>());
            _mocks.Resolve<HttpContextBase>().Setup(s => s.Items).Returns(new Dictionary<string, string>());
            NeedsPublishedContentRequest();
            NeedsCustomViewEngine(); //this also clears the standard view engines
            _mocks.Resolve<IWebRoutingSection>().Setup(s => s.UrlProviderMode).Returns(UrlProviderMode.AutoLegacy.ToString());
        }

        //TODO add MEMBER

        public PublishedContentType WithPublishedContentType(int? id = null, string name = null, string alias = null, IEnumerable<PropertyType> propertyTypes = null)
        {
            alias = alias ?? _Fixture.Create<string>();
            id = id.HasValue ? id : _Fixture.Create<int>(); //use unique ID system?
            name = name ?? _Fixture.Create<string>();
            PublishedContentType contentType = null;
            if ((contentType = this.ContentTypes.FirstOrDefault(c => c.Alias == alias)) != null) //already existings, return
            {
                return contentType;
            }
            if (propertyTypes != null && propertyTypes.Count() > 0) //need boot manager for property types...
                NeedsCoreBootManager();
            mockServiceContext.ContentTypeService.Setup(s => s.GetContentType(alias)).Returns(UmbracoUnitTestHelper.GetContentTypeComposition<IContentType>(alias: alias, name: name, id: id, propertyTypes: propertyTypes));
            contentType = UmbracoUnitTestHelper.GetPublishedContentType(PublishedItemType.Content, alias);
            return contentType;
        }

        public void WithCustomViewEngine(bool clearNonCustom = true, IViewEngine viewEngine = null)
        {
            if (clearNonCustom)
                NeedsClearedViewEngine();
            ViewEngines.Engines.Add(viewEngine);
        }

        #region Controller Methods

        public void RegisterController(Controller controller)
        {
            Controller = controller;
            AffectsController(false, ControllerActions.ToArray());
        }

        public void RegisterController(ApiController controller)
        {
            ApiController = controller;
            AffectsController(false, ControllerActions.ToArray());
        }

        #endregion

        #region Needs

        private UmbracoHelper NeedsUmbracoHelper()
        {
            if (_umbHelper == null)
            {
                _umbHelper = UmbracoUnitTestHelper.GetUmbracoHelper(this.UmbracoContext,
                    cultureDictionary: _mocks.ResolveObject<ICultureDictionary>(),
                    content: _mocks.ResolveObject<IPublishedContent>("Current"),
                    typedQuery: _mocks.ResolveObject<ITypedPublishedContentQuery>(),
                    dynamicQuery: _mocks.ResolveObject<IDynamicPublishedContentQuery>());
                NeedsTypedQuery();
                NeedsDynamicQuery();
                AffectsController(true, GiveControllerContext, EnsureControllerHasHelper);
            }
            return _umbHelper;
        }

        private RouteData NeedsRouteData()
        {
            if (_routeData == null)
                _routeData = new RouteData();
            return _routeData;
        }

        private HttpRouteData NeedsHttpRouteData()
        {
            if (_httpRouteData == null)
            {
                _httpRouteData = new HttpRouteData(_mocks.ResolveObject<IHttpRoute>(), new HttpRouteValueDictionary());
            }
            return _httpRouteData;
        }

        private PublishedContentRequest NeedsPublishedContentRequest()
        {
            if (_publishedContentRequest == null)
            {
                _publishedContentRequest = UmbracoUnitTestHelper.GetPublishedContentRequest(UmbracoContext, currentContent: _mocks.ResolveObject<IPublishedContent>("Current"));
                UmbracoContext.PublishedContentRequest = _publishedContentRequest;
            }
            return _publishedContentRequest;
        }

        private bool typedQuerySetup = false;
        private void NeedsTypedQuery()
        {
            if (!typedQuerySetup)
            {
                var mock =
                    _mocks.Resolve<ITypedPublishedContentQuery>();

                mock.Setup(s => s.TypedContent(It.IsAny<int>())).Returns<int>(id =>
                      this.Content.FirstOrDefault(c => c.Id == id)
                    );

                mock.Setup(s => s.TypedContent(It.IsAny<IEnumerable<int>>())).Returns<IEnumerable<int>>(ids =>
                      this.Content.Where(c => ids.Contains(c.Id))
                  );

                mock.Setup(s => s.TypedContentAtRoot()).Returns(() => this.Content.Where(s => s.Level == 1));

                mock.Setup(s => s.TypedMedia(It.IsAny<int>())).Returns<int>(id =>
                    this.Media.FirstOrDefault(c => c.Id == id)
                );

                mock.Setup(s => s.TypedMedia(It.IsAny<IEnumerable<int>>())).Returns<IEnumerable<int>>(ids =>
                    this.Media.Where(c => ids.Contains(c.Id))
                );

                mock.Setup(s => s.TypedMediaAtRoot()).Returns(() => this.Media.Where(s => s.Level == 1));

                //TODO typed search

                typedQuerySetup = true;
            }
        }

        private bool dynamicQuerySetup = false;
        private void NeedsDynamicQuery()
        {
            if (!dynamicQuerySetup)
            {
                var mock =
                    _mocks.Resolve<IDynamicPublishedContentQuery>();

                mock.Setup(s => s.Content(It.IsAny<int>())).Returns<int>(id =>
                      this.Content.FirstOrDefault(c => c.Id == id)
                    );

                mock.Setup(s => s.Content(It.IsAny<IEnumerable<int>>())).Returns<IEnumerable<int>>(ids =>
                      this.Content.Where(c => ids.Contains(c.Id))
                  );

                mock.Setup(s => s.ContentAtRoot()).Returns(() => this.Content.Where(s => s.Level == 1));

                mock.Setup(s => s.Media(It.IsAny<int>())).Returns<int>(id =>
                    this.Media.FirstOrDefault(c => c.Id == id)
                );

                mock.Setup(s => s.Media(It.IsAny<IEnumerable<int>>())).Returns<IEnumerable<int>>(ids =>
                    this.Media.Where(c => ids.Contains(c.Id))
                );

                mock.Setup(s => s.MediaAtRoot()).Returns(() => this.Media.Where(s => s.Level == 1));

                //TODO typed search

                dynamicQuerySetup = true;
            }
        }

        private void NeedsCoreBootManager()
        {
            if (!_boot.Initialized)
                _boot.Initialize();
            if (!_boot.Started)
                _boot.Startup(null);
            if (!_boot.Completed)
                _boot.Complete(null);
        }

        private void NeedsClearedViewEngine()
        {
            if (!_viewEngineCleared)
            {
                _viewEngineCleared = true;
                ViewEngines.Engines.RemoveAll((s) => true);
            }
        }

        private void NeedsCustomViewEngine()
        {
            if (!ViewEngines.Engines.Any(c => c.GetType() == typeof(CustomViewEngine)))
                WithCustomViewEngine(true, new CustomViewEngine());
        }

        #endregion

        #region Affects
        private void AffectsController(bool store, params Action[] actions)
        {
            foreach (var action in actions)
            {
                if (!store || ControllerActions.Add(action))//add the action
                    if (HasAnyController) //only preform the action if it is new and the controller exists
                        action();
            }
        }
        #endregion

        #region Give
        private void GiveControllerContext()
        {
            if (HasAnyController)
            {
                if (HasApiController)
                {
                    if (ApiController.ControllerContext == null)
                        ApiController.ControllerContext = UmbracoUnitTestHelper.GetApiControllerContext(NeedsHttpRouteData());
                }
                else if (HasMvcController)
                {
                    if (Controller.ControllerContext == null)
                        Controller.ControllerContext = UmbracoUnitTestHelper.GetControllerContext(UmbracoContext, Controller, NeedsPublishedContentRequest(), NeedsRouteData());
                }
            }
        }
        #endregion

        #region Ensure
        private void EnsureControllerHasHelper()
        {
            if (HasAnyController)
            {
                UmbracoHelper _assignedHelper = null;
                if (HasApiController)
                {
                    if (ApiController is UmbracoApiController)
                        _assignedHelper = ((UmbracoApiController)ApiController).Umbraco;
                }
                else if (HasMvcController)
                {
                    if (Controller is RenderMvcController)
                        _assignedHelper = ((RenderMvcController)Controller).Umbraco;
                    else if (Controller is SurfaceController)
                        _assignedHelper = ((SurfaceController)Controller).Umbraco;
                }
                if (_assignedHelper != UmbracoHelper) // should be our object!!
                {
                    throw new Exception(string.Format("{0} must implement and use base constructor which takes in the Umrabco Helper. This allows the Umbraco Helper with mocked data to be passed in.", Controller.GetType().Name));//make a better excpetion class?
                }
            }
        }

        #endregion

        private int ResolveUnqueContentId(int? id = null, bool errorOnDuplicateProvided = true)
        {
            if (id.HasValue)
                if (contentIdCollection.Add(id.Value) || !errorOnDuplicateProvided || !EnforceUniqueContentIds)
                {
                    return id.Value;
                }
                else
                {
                    throw new Exception("Duplicate ID provided. Enforcement of Unique Ids can be disabled using the EnforceUniqueContentIds paramater. If unique ID enforcement is prefered, generally providing a null ID will assign a random ID");
                }
            do
            {
                id = _rand.Next();
            } while (!contentIdCollection.Add(id.Value));
            return id.Value;
        }

        public void Dispose()
        {
            UmbracoUnitTestHelper.CleanupCoreBootManager(this.ApplicationContext);
        }



    }
}
