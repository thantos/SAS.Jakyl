using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace SAS.Jakyl.TestWeb.Controllers
{
    public class BasicTestSurfaceController : SurfaceController
    {
        public BasicTestSurfaceController() { }

        public BasicTestSurfaceController(UmbracoContext context, UmbracoHelper helper) : base(context, helper) { }

        public PartialViewResult BasicTestAction()
        {
            return PartialView(new { });
        }

        public PartialViewResult BasicCurrentPageAction()
        {
            return PartialView(null, CurrentPage.Name);
        }

        public PartialViewResult BasicPublishedContentAction()
        {
            return PartialView(null, this.Umbraco.AssignedContentItem.Name);
        }

        public PartialViewResult BasicDictionaryAction()
        {
            return PartialView(null,this.Umbraco.GetDictionaryValue("Test Key"));
        }

        public PartialViewResult BasicTypedContentMediaAction(int contentId, int mediaId)
        {
            return PartialView(null, new Tuple<string, string>(this.Umbraco.TypedContent(contentId).Name, this.Umbraco.TypedMedia(mediaId).Name));
        }

        public PartialViewResult BasicDynamicContentMediaAction(int contentId, int mediaId)
        {
            return PartialView(null, new Tuple<string, string>(this.Umbraco.Content(contentId).Name, this.Umbraco.Media(mediaId).Name));
        }

        public PartialViewResult BasicTypedSearchAction(string search_term)
        {
            return PartialView(null, Umbraco.TypedSearch(search_term).Count());
        }

        public PartialViewResult BasicContentTypeAction(int id)
        {
            var type = Umbraco.TypedContent(id);

            var alias = type.ContentType.Alias;

            return PartialView(null, alias);
        }

        public PartialViewResult BasicHasPropertyAction(int id, string property)
        {
            var type = Umbraco.TypedContent(id);

            var hasProperty = type.HasProperty(property);

            return PartialView(null, hasProperty);
        }

        public PartialViewResult BasicGetPropertyAction(int id, string property)
        {
            var type = Umbraco.TypedContent(id);

            var val = type.GetProperty(property).Value;

            return PartialView(null, val);
        }

        public PartialViewResult BasicGetPropertyValueAction(int id, string property)
        {
            var type = Umbraco.TypedContent(id);

            var val = type.GetPropertyValue(property);

            return PartialView(null, val);
        }

        public PartialViewResult BasicGetPropertyValueTypeAction(int id, string property)
        {
            var type = Umbraco.TypedContent(id);

            var val = type.GetPropertyValue<string>(property);

            return PartialView(null, val);
        }

        public PartialViewResult BasicPositionAction(int id)
        {
            var type = Umbraco.TypedContent(id);

            return PartialView(null, type.IsFirst());
        }

        ///Don't think we can test this ... Get culture requires UmbracoContext.ContentCache to be not null
        public PartialViewResult BasicGetCultureAction(int id)
        {
            var type = Umbraco.TypedContent(id);

            return PartialView(null,type.GetCulture());
        }

        public PartialViewResult RelationChildAction(int child)
        {
            var relationService = ApplicationContext.Services.RelationService;

            var rel = relationService.GetByChildId(child);

            return PartialView(null, rel.First().Id);
        }

        public PartialViewResult RelationParentAction(int parent)
        {
            var relationService = ApplicationContext.Services.RelationService;

            var rel = relationService.GetByParentId(parent);

            return PartialView(null, rel.First().Id);
        }

        public PartialViewResult RelationAliasAction(string alias)
        {
            var relationService = ApplicationContext.Services.RelationService;

            var rel = relationService.GetByRelationTypeAlias(alias);

            return PartialView(null, rel.First().Id);
        }

    }
}