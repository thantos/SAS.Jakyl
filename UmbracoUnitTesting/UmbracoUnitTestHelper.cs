using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Cache;
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
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;


namespace UmbracoUnitTesting
{
    public static class UmbracoUnitTestHelper
    {
        public static ApplicationContext GetApplicationContext(ServiceContext serviceContext = null, DatabaseContext databaseContext = null, CacheHelper cacheHelper = null, ILogger logger = null, IProfiler profiler = null, IWebRoutingSection webRoutingSection = null)
        {

            return ApplicationContext.EnsureContext(
                databaseContext ?? GetDatabaseContext(logger: logger),
                serviceContext ?? GetServiceContext(),
                cacheHelper ?? /*CacheHelper.CreateDisabledCacheHelper()*/
                    GetCacheHelper(),
                new ProfilingLogger(
                    logger ?? Mock.Of<ILogger>(),
                    profiler ?? Mock.Of<IProfiler>()), true);
        }

        public static UmbracoContext GetUmbracoContext(ApplicationContext applicationContext = null, IWebRoutingSection webRoutingSettings = null, HttpContextBase httpContext = null, WebSecurity webSecurity = null,
            IUmbracoSettingsSection settingsSection = null, IEnumerable<IUrlProvider> urlProviders = null)
        {
            return UmbracoContext.EnsureContext(
                httpContext ?? new Mock<HttpContextBase>().Object,
                applicationContext ?? GetApplicationContext(),
                webSecurity ?? new Mock<WebSecurity>(null, null).Object,
                settingsSection ?? Mock.Of<IUmbracoSettingsSection>(section => section.WebRouting == (webRoutingSettings ?? GetBasicWebRoutingSettings())),
                urlProviders ?? Enumerable.Empty<IUrlProvider>(),
                true);
        }

        public static CacheHelper GetCacheHelper(IRuntimeCacheProvider httpCache = null, ICacheProvider staticCache = null, ICacheProvider requestCache = null, IsolatedRuntimeCache isolastedRuntime = null)
        {
            return new CacheHelper(httpCache ?? Mock.Of<IRuntimeCacheProvider>(),
                        staticCache ?? Mock.Of<ICacheProvider>(),
                        requestCache ?? Mock.Of<ICacheProvider>(),
                        isolastedRuntime ?? GetIsolatedRuntimeCache());
        }

        public static IsolatedRuntimeCache GetIsolatedRuntimeCache(IRuntimeCacheProvider cacheFactory = null)
        {
            return new IsolatedRuntimeCache(f => cacheFactory ?? Mock.Of<IRuntimeCacheProvider>());
        }

        public static DatabaseContext GetDatabaseContext(IDatabaseFactory factory = null, ILogger logger = null, SqlSyntaxProviders sqlSyntaxProviers = null)
        {
            return new DatabaseContext(factory ?? Mock.Of<IDatabaseFactory>(), logger ?? Mock.Of<ILogger>(), sqlSyntaxProviers ?? new SqlSyntaxProviders(new[] { Mock.Of<ISqlSyntaxProvider>() }));
        }

        /// <summary>
        /// This will take some to build out...
        /// </summary>
        public static ServiceContext GetServiceContext(MockServiceContext mockServiceContext = null)
        {
            return mockServiceContext != null ? mockServiceContext.ServiceContext : new ServiceContext();
        }

        private static IWebRoutingSection GetBasicWebRoutingSettings()
        {
            return GetBasicWebRoutingSettings(UrlProviderMode.AutoLegacy);
        }

        private static IWebRoutingSection GetBasicWebRoutingSettings(UrlProviderMode mode = default(UrlProviderMode))
        {
            return Mock.Of<IWebRoutingSection>(section => section.UrlProviderMode == (mode.ToString())); //should default to AutoLegacy
        }

        public static UmbracoHelper GetUmbracoHelper(UmbracoContext context, ICultureDictionary cultureDictionary = null, MembershipHelper membershipHelper = null, UrlProvider urlProvider = null,
            IPublishedContent content = null, ITypedPublishedContentQuery typedQuery = null, IDynamicPublishedContentQuery dynamicQuery = null, ITagQuery tagQuery = null, IDataTypeService typeService = null,
            IUmbracoComponentRenderer componentRenderer = null)
        {
            return new UmbracoHelper(context,
                content ?? Mock.Of<IPublishedContent>(),
                typedQuery ?? Mock.Of<ITypedPublishedContentQuery>(),
                dynamicQuery ?? Mock.Of<IDynamicPublishedContentQuery>(),
                tagQuery ?? Mock.Of<ITagQuery>(),
                typeService ?? Mock.Of<IDataTypeService>(),
                urlProvider ?? GetUmbracoUrlProvider(context),
                cultureDictionary ?? Mock.Of<ICultureDictionary>(),
                componentRenderer ?? Mock.Of<IUmbracoComponentRenderer>(),
                membershipHelper ?? GetUmbracoMembershipHelper(context));
        }

        public static MembershipHelper GetUmbracoMembershipHelper(UmbracoContext context, MembershipProvider membershipProvider = null, RoleProvider roleProvider = null)
        {
            return new MembershipHelper(context, membershipProvider ?? Mock.Of<MembershipProvider>(), roleProvider ?? Mock.Of<RoleProvider>());
        }

        public static UrlProvider GetUmbracoUrlProvider(UmbracoContext context, IWebRoutingSection routingSection = null, IEnumerable<IUrlProvider> urlProviders = null)
        {
            return new UrlProvider(context, routingSection ?? GetBasicWebRoutingSettings(UrlProviderMode.Auto), urlProviders ?? new[] { Mock.Of<IUrlProvider>() });
        }

        public static IPublishedContent GetPublishedContent()
        {
            return GetPublishedContentMock().Object;
        }

        public static Mock<IPublishedContent> GetPublishedContentMock(string name = null, int? id = null, string path = null, string url = null, int? templateId = null, DateTime? updateDate = null, DateTime? createDate = null, PublishedContentType contentType = null, IPublishedContent parent = null, IEnumerable<IPublishedContent> Children = null)
        {
            var mock = new Mock<IPublishedContent>();
            mock.Setup(s => s.Name).Returns(name);
            if (id.HasValue)
                mock.Setup(s => s.Id).Returns(id.Value);
            mock.Setup(s => s.Path).Returns(path);
            mock.Setup(s => s.Url).Returns(url);
            if (createDate.HasValue)
                mock.Setup(s => s.CreateDate).Returns(createDate.Value);
            if (updateDate.HasValue)
                mock.Setup(s => s.UpdateDate).Returns(updateDate.Value);
            if (templateId.HasValue)
                mock.Setup(s => s.TemplateId).Returns(templateId.Value);
            if (contentType != null)
                mock.Setup(s => s.ContentType).Returns(contentType);
            else
                mock.Setup(s => s.ContentType).Returns(GetPublishedContentType);
            if (parent != null)
                mock.Setup(s => s.Parent).Returns(parent);
            else
                mock.Setup(s => s.Parent).Returns(GetPublishedContent);
            if (Children != null)
                mock.Setup(s => s.Children).Returns(Children);
            else
                mock.Setup(s => s.Children).Returns(() => new[] { GetPublishedContent() });
            return mock;
        }

        public static T GetContentTypeComposition<T>(int? id = null, string alias = "default", string name = null, IEnumerable<PropertyType> propertyTypes = null)
            where T : class, IContentTypeComposition
        {
            var mock = new Mock<T>();
            mock.Setup(s => s.Id).Returns(id.HasValue ? id.Value : new Random().Next());
            mock.Setup(s => s.Alias).Returns(alias);
            mock.Setup(s => s.Name).Returns(name);
            mock.Setup(s => s.CompositionPropertyTypes).Returns(propertyTypes ?? new PropertyType[] { GetPropertyType() }); //Issue gettting converters
            return mock.Object;
        }
        public static PublishedContentType GetPublishedContentType()
        {
            return GetPublishedContentType(PublishedItemType.Content);
        }

        public static PublishedContentType GetPublishedContentType(PublishedItemType type = PublishedItemType.Content, string alias = "default", PublishedPropertyType DefaultType = null)
        {
            return PublishedContentType.Get(type, alias);
        }

        public static PublishedPropertyType GetPublishedPropertyType(PublishedContentType contentType, PropertyType propertyType = null)
        {
            return new PublishedPropertyType(contentType, propertyType ?? GetPropertyType());
        }

        public static PropertyType GetPropertyType(IDataTypeDefinition dataTypeDef = null, string alias = null)
        {
            return new PropertyType(dataTypeDef ?? Mock.Of<IDataTypeDefinition>(d => d.PropertyEditorAlias == "default"), string.IsNullOrEmpty(alias) ? "_umb_default" : alias); //use _umb_ to avoid StringExtentions (causes config loading errors)
        }

        public static ControllerContext GetControllerContext(UmbracoContext context, Controller controller, PublishedContentRequest publishedContentRequest = null)
        {
            var contextBase = context.HttpContext;

            var pcr = publishedContentRequest ?? context.PublishedContentRequest ?? GetPublishedContentRequest(context);

            var routeDefinition = new RouteDefinition
            {
                PublishedContentRequest = pcr
            };

            var routeData = new RouteData();
            routeData.DataTokens.Add("umbraco-route-def", routeDefinition);
            return new ControllerContext(contextBase, routeData, controller);
        }

        public static PublishedContentRequest SetPublishedContentRequest(UmbracoContext context = null, PublishedContentRequest request = null)
        {
            return (context ?? UmbracoContext.Current).PublishedContentRequest = request ?? GetPublishedContentRequest();
        }

        public static PublishedContentRequest GetPublishedContentRequest(UmbracoContext context = null, string url = null, IWebRoutingSection routingSection = null, IEnumerable<string> rolesForLogic = null, IPublishedContent currentContent = null)
        {
            return new PublishedContentRequest(new Uri(string.IsNullOrEmpty(url) ? "http://localhost/test" : url),
                (context ?? UmbracoContext.Current).RoutingContext,
                routingSection ?? GetBasicWebRoutingSettings(),
                s => rolesForLogic ?? Enumerable.Empty<string>())
            {
                PublishedContent = currentContent ?? Mock.Of<IPublishedContent>(publishedContent => publishedContent.Id == 12345)
            };
        }

        /// <summary>
        /// To allow Helper.GetPublishedContentType and PublishedContentType.Get to work
        /// 
        /// https://github.com/umbraco/Umbraco-CMS/blob/67c3ea7c00f44cf3426a37c2cc62e7b561fc859a/src/Umbraco.Core/Models/PublishedContent/PublishedContentType.cs ln 133-170
        /// </summary>
        /// <param name="mockServiceContext"></param>
        public static void SetupServicesForPublishedContentResolution(MockServiceContext mockServiceContext)
        {
            mockServiceContext.ContentTypeService.Setup(s => s.GetContentType(It.IsAny<string>())).Returns<string>(s => GetContentTypeComposition<IContentType>(alias: s));
            mockServiceContext.ContentTypeService.Setup(s => s.GetMediaType(It.IsAny<string>())).Returns<string>(s => GetContentTypeComposition<IMediaType>(alias: s));
            mockServiceContext.MemberTypeService.Setup(s => s.Get(It.IsAny<string>())).Returns<string>(s => GetContentTypeComposition<IMemberType>(alias: s));
        }

    }
}
