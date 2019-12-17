using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DIVerify {
    public class VerifiableServiceCollection : IServiceCollection {

        #region Private Members
            
        private readonly List<ServiceDescriptor> _services = new List<ServiceDescriptor>();
        private readonly List<VerificationBuilderBase> _builders = new List<VerificationBuilderBase>();

        #endregion

        #region IServiceCollection Properties

        public ServiceDescriptor this[int index] { 
            get => _services[index]; 
            set => _services[index] = value; 
        }

        public int Count => _services.Count;

        public bool IsReadOnly => false;

        #endregion

        #region IServiceCollection Methods

        public void Add(ServiceDescriptor item) => _services.Add(item);

        public void Clear() => _services.Clear();

        public bool Contains(ServiceDescriptor item) => _services.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _services.CopyTo(array, arrayIndex);

        public IEnumerator<ServiceDescriptor> GetEnumerator() => _services.GetEnumerator();

        public int IndexOf(ServiceDescriptor item) => _services.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item) => _services.Insert(index, item);

        public bool Remove(ServiceDescriptor item) => _services.Remove(item);

        public void RemoveAt(int index) => _services.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Public Methods

        public IRegistrationVerificationBuilder Expect<T>() where T : class
            => Expect(typeof(T));
            
        public IRegistrationVerificationBuilder Expect(Type typeToVerify) {
            var factory = new VerificationBuilderFactory(typeToVerify);
            factory.Callback(builder => _builders.Add(builder));

            return factory;
        }

        public void VerifyExpecations() {
            var results = _builders.Select(b => b.Build().Verify(this, b.FailureMessage ?? b.DefaultMessage));
            var failed = results.Where(r => !r.Success);
            if (failed.Any()) {
                throw new ServiceVerificationException(
                    $"Verification failed with {failed.Count()} failure{(failed.Count() == 1 ? "" : "s")}:" + Environment.NewLine +
                    failed.Select(f => f.FailureMessage)
                        .Aggregate((left, right) => left + Environment.NewLine + right) 
                    ?? "No details provided"
                );
            }
        }

        #endregion
    }
}
