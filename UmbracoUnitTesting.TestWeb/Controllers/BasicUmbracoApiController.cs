using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.Membership;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace UmbracoUnitTesting.TestWeb.Controllers
{
    public class BasicUmbracoApiController : UmbracoApiController
    {
        public BasicUmbracoApiController()
        {

        }
        public BasicUmbracoApiController(UmbracoContext context, UmbracoHelper helper) : base(context, helper)
        {

        }

        public string BasicApiCall()
        {
            return string.Empty;
        }

        public string BasicDictionaryAction(string key)
        {
            return this.Umbraco.GetDictionaryValue(key);
        }

        public Tuple<string, string> BasicTypedContentMediaAction(int contentId, int mediaId)
        {
            return new Tuple<string, string>(this.Umbraco.TypedContent(contentId).Name, this.Umbraco.TypedMedia(mediaId).Name);
        }

        public Tuple<string, string> BasicDynamicContentMediaAction(int contentId, int mediaId)
        {
            return new Tuple<string, string>(this.Umbraco.Content(contentId).Name, this.Umbraco.Media(mediaId).Name);
        }

        public int BasicTypedSearchAction(string search_term)
        {
            return Umbraco.TypedSearch(search_term).Count();
        }

        public string BasicContentTypeAction(int id)
        {
            var type = Umbraco.TypedContent(id);

            var alias = type.ContentType.Alias;

            return alias;
        }

        public bool BasicHasPropertyAction(int id, string property)
        {
            var type = Umbraco.TypedContent(id);

            var hasProperty = type.HasProperty(property);

            return hasProperty;
        }

        public object BasicGetPropertyAction(int id, string property)
        {
            var type = Umbraco.TypedContent(id);

            var val = type.GetProperty(property).Value;

            return val;
        }

        public bool BasicPositionAction(int id)
        {
            var type = Umbraco.TypedContent(id);

            return type.IsFirst();
        }

        ///Don't think we can test this ... Get culture requires UmbracoContext.ContentCache to be not null
        public CultureInfo BasicGetCultureAction(int id)
        {
            var type = Umbraco.TypedContent(id);

            return type.GetCulture();
        }

        public IUser BasicUserAction()
        {
            return Security.CurrentUser;
        }

        public bool BasicIsAuthenticatedAction()
        {
            return Security.IsAuthenticated();
        }

    }
}