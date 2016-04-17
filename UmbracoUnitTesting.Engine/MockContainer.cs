using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoUnitTesting.Engine
{
    public class MockContainer
    {
        private const string __global = "__global";
        readonly Dictionary<string, Dictionary<Type, Mock>> _mocks;

        internal MockContainer()
        {
            _mocks = new Dictionary<string, Dictionary<Type, Mock>>();
            _mocks[__global] = new Dictionary<Type, Mock>();
        }

        public Mock<T> Resolve<T>(params object[] parameters)
            where T : class
        {
            return this.Resolve<T>(__global, parameters);
        }

        public Mock<T> Resolve<T>(string scope, params object[] parameters)
            where T : class
        {
            if (!_mocks.ContainsKey(scope))
            {
                _mocks[scope] = new Dictionary<Type, Mock>();
            }
            if (!_mocks[scope].ContainsKey(typeof(T)))
            {
                _mocks[scope][typeof(T)] = new Mock<T>(parameters);
            }
            return _mocks[scope][typeof(T)] as Mock<T>;
        }

        public T ResolveObject<T>(params object[] parameters)
            where T : class
        {
            return this.ResolveObject<T>(__global);
        }

        public T ResolveObject<T>(string scope, params object[] parameters)
            where T: class
        {
            return Resolve<T>(scope, parameters).Object;
        }

    }
}
