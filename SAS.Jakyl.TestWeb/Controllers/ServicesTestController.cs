using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace SAS.Jakyl.TestWeb.Controllers
{
    public class ServicesTestController : Umbraco.Web.WebApi.UmbracoAuthorizedApiController
    {

        public ServicesTestController(UmbracoContext context, UmbracoHelper helper) : base(context, helper) { }

        public IContent ContentGetById(int id)
        {
            var contentService = ApplicationContext.Services.ContentService;
            var content = contentService.GetById(id);

            return content;
        }

        public void ContentSetValue(int id, string propAlias, string propValue)
        {
            var contentService = ApplicationContext.Services.ContentService;
            var content = contentService.GetById(id);

            content.SetValue(propAlias, propValue);
        }

        public IContent ContentCreateContent(string name, string alias)
        {
            var contentService = ApplicationContext.Services.ContentService;
            var content = contentService.CreateContent(name, null, alias);

            return content;
        }

        public void ContentSaveAndPublishWithStatus(int id)
        {
            var contentService = ApplicationContext.Services.ContentService;
            var content = contentService.GetById(id);

            contentService.SaveAndPublishWithStatus(content);
        }

    }
}