using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace SAS.Jakyl.TestWeb.Test
{
    public class TestPublishedContent : IPublishedContent
    {
        public string Url { get; set; }
        public PublishedItemType ItemType { get; set; }

        IPublishedContent IPublishedContent.Parent
        {
            get { return Parent; }
        }

        IEnumerable<IPublishedContent> IPublishedContent.Children
        {
            get { return Children; }
        }

        public IPublishedContent Parent { get; set; }
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
        public string DocumentTypeAlias { get; set; }
        public int DocumentTypeId { get; set; }
        public string WriterName { get; set; }
        public string CreatorName { get; set; }
        public int WriterId { get; set; }
        public int CreatorId { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public Guid Version { get; set; }
        public int Level { get; set; }
        public bool IsDraft { get; set; }
        public int GetIndex() { throw new NotImplementedException(); }

        public ICollection<IPublishedProperty> Properties { get; set; }

        public object this[string propertyAlias]
        {
            get { return GetProperty(propertyAlias).Value; }
        }

        public IEnumerable<IPublishedContent> Children { get; set; }

        public IPublishedProperty GetProperty(string alias)
        {
            return Properties.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        public IPublishedProperty GetProperty(string alias, bool recurse)
        {
            var property = GetProperty(alias);
            if (recurse == false) return property;

            IPublishedContent content = this;
            while (content != null && (property == null || property.HasValue == false))
            {
                content = content.Parent;
                property = content == null ? null : content.GetProperty(alias);
            }

            return property;
        }

        public IEnumerable<IPublishedContent> ContentSet
        {
            get { throw new NotImplementedException(); }
        }

        public PublishedContentType ContentType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
