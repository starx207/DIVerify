using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DIVerify
{
    public class VerificationBuilder : VerificationBuilderBase, 
        ITypeVerificationBuilder, 
        IImplementationVerificationBuilder, 
        IServiceVerificationBuilder, 
        IVerificationBuilder {
        
        #region Private Members
            
        private Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> _initialVerification;
        private readonly List<Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>>> _intermediateVerifications;
        private Expression<Func<IEnumerable<ServiceDescriptor>, bool>> _finalVerification;

        #endregion

        #region Constructors

        public VerificationBuilder(Type typeToVerify)
            : base(typeToVerify) {
            
            _initialVerification = svc => svc.Where(s => s.ServiceType == TypeToVerify || s.ImplementationType == TypeToVerify);
            _finalVerification = d => d.Count() > 0;
            _intermediateVerifications = new List<Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>>>();
        }

        #endregion

        #region Public Methods

        public IImplementationVerificationBuilder AsImplementation() {
            _initialVerification = AsImplementationExpr();
            return this;
        }

        public ILifetimeVerificationBuilder AsSelf() {
            _initialVerification = AsSelfExpr();
            return this;
        }

        public IServiceVerificationBuilder AsService() {
            _initialVerification = AsServiceExpr();
            return this;
        }

        public ILifetimeVerificationBuilder AtLeast(int minimumRegistrations) {
            _finalVerification = d => d.Count() >= minimumRegistrations;
            return this;
        }

        public ILifetimeVerificationBuilder AtMost(int maximumRegistrations) {
            _finalVerification = d => d.Count() <= maximumRegistrations;
            return this;
        }

        public IVerification Build() {
            var descriptorExpr = _intermediateVerifications.Count > 0
                ? _initialVerification.CombineWithAnd(_intermediateVerifications.Aggregate((left, right) => left.CombineWithAnd(right)))
                : _initialVerification;

            return new Verification() {
                ServiceVerification = svc => {
                    var descriptors = descriptorExpr.Compile().Invoke(svc);
                    return _finalVerification.Compile().Invoke(descriptors);
                }
            };
        }

        public ILifetimeVerificationBuilder Exactly(int numRegistrations) {
            _finalVerification = d => d.Count() == numRegistrations;
            return this;
        }

        public IDescriptorVerificationBuilder For(Type serviceType) {
            _intermediateVerifications.Add(ForExpr(serviceType));
            return this;
        }

        public IDescriptorVerificationBuilder With(Type implementationType) {
            _intermediateVerifications.Add(WithExpr(implementationType));
            return this;
        }

        public IDescriptorVerificationBuilder WithFactory(Func<IServiceProvider, object> expectedFactoryFunc) {
            _intermediateVerifications.Add(WithFactoryExpr(expectedFactoryFunc));
            return this;
        }

        public IDescriptorVerificationBuilder WithInstance<TInstance>(TInstance instance) {
            _intermediateVerifications.Add(WithInstanceObjExpr(instance));
            return this;
        }

        public IDescriptorVerificationBuilder WithInstance(Func<object, bool> instanceMatcher) {
            _intermediateVerifications.Add(WithInstanceFuncExpr(instanceMatcher));
            return this;
        }

        public ICountVerificationBuilder WithLifetime(ServiceLifetime lifetime) {
            _intermediateVerifications.Add(WithLifetimeExpr(lifetime));
            return this;
        }

        #endregion
    }
}