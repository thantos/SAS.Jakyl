using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Dictionary;
using Umbraco.Core.ObjectResolution;

namespace UmbracoUnitTesting.TestWeb.Test.TestClasses
{
    class TestDictResolver : SingleObjectResolverBase<TestDictResolver, ICultureDictionaryFactory>
    {
        public TestDictResolver(ICultureDictionaryFactory factory)
            : base(factory)
        {
        }

        [Obsolete("Use SetDictionaryFactory instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetContentStore(ICultureDictionaryFactory factory)
        {
            Value = factory;
        }

        /// <summary>
        /// Can be used by developers at runtime to set their ICultureDictionaryFactory at app startup
        /// </summary>
        /// <param name="factory"></param>
        public void SetDictionaryFactory(ICultureDictionaryFactory factory)
        {
            Value = factory;
        }

        /// <summary>
        /// Returns the ICultureDictionaryFactory
        /// </summary>
        public ICultureDictionaryFactory Factory
        {
            get { return Value; }
        }
    }
}
