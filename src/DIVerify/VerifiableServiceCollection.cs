using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DIVerify {
    public class VerifiableServiceCollection : IServiceCollection {

        #region Private Members
            
        private readonly List<ServiceDescriptor> _services = new List<ServiceDescriptor>();
        private List<(IVerificationBuilder builder, string? failureMessage)> _builders = new List<(IVerificationBuilder, string?)>();

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
            
        public IRegistrationVerificationBuilder Expect(Type typeToVerify, string? failureMessage = null) {
            var factory = new VerificationBuilderFactory(typeToVerify);
            factory.Callback(builder => _builders.Add((builder, failureMessage)));

            return factory;
        }

        public void Verify() {
            var results = _builders.Select(b => b.builder.Build().Verify(this, b.failureMessage));
            var failed = results.Where(r => !r.Success);
            if (failed.Any()) {
                throw new ServiceVerificationException(
                    $"Verification failed with {failed.Count()} failures:" + Environment.NewLine +
                    failed.Select(f => f.FailureMessage)
                        .Aggregate((left, right) => left + Environment.NewLine + right) 
                    ?? "No details provided"
                );
            }
        }

        #endregion
    }
}