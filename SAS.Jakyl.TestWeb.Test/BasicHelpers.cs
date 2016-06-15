using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace SAS.Jakyl.TestWeb.Test
{
    public static class BasicHelpers
    {
        public static Mock<IPublishedContent> GetPublishedContentMock(string name = null, int? id = null,
            string path = null, string url = null, int? templateId = null, DateTime? updateDate = null, 
            DateTime? createDate = null, PublishedContentType contentType = null, 
            IPublishedContent parent = null, IEnumerable<IPublishedContent> Children = null)
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
            mock.Setup(s => s.ContentType).Returns(contentType);
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

        public static IPublishedContent GetPublishedContent()
        {
            return GetPublishedContentMock().Object;
        }
    }
}
