using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DIVerify
{
    public class NegatedVerificationBuilder : VerificationBuilderBase, 
        INegatedTypeVerificationBuilder, 
        INegatedImplementationVerificationBuilder, 
        INegatedServiceVerificationBuilder, 
        IVerificationBuilder {
        
        #region Private Members
            
        private Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> _initialVerification;
        private readonly List<Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>>> _intermediateVerifications;
        private Expression<Func<IEnumerable<ServiceDescriptor>, bool>> _finalVerification;

        #endregion

        #region Constructors

        public NegatedVerificationBuilder(Type typeToVerify)
            : base(typeToVerify) {
            
            _initialVerification = svc => svc.Where(s => s.ServiceType == TypeToVerify || s.ImplementationType == TypeToVerify);
            _finalVerification = d => d.Count() == 0;
            _intermediateVerifications = new List<Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>>>();
        }

        #endregion

        #region Public Methods

        public INegatedImplementationVerificationBuilder AsImplementation() {
            _initialVerification = AsImplementationExpr();
            return this;
        }

        public INegatedLifetimeVerificationBuilder AsSelf() {
            _initialVerification = AsSelfExpr();
            return this;
        }

        public INegatedServiceVerificationBuilder AsService() {
            _initialVerification = AsServiceExpr();
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

        public INegatedLifetimeVerificationBuilder For(Type serviceType) {
            _intermediateVerifications.Add(ForExpr(serviceType));
            return this;
        }

        public INegatedLifetimeVerificationBuilder With(Type implementationType) {
            _intermediateVerifications.Add(WithExpr(implementationType));
            return this;
        }

        public INegatedLifetimeVerificationBuilder WithFactory(Func<IServiceProvider, object> expectedFactoryFunc) {
            _intermediateVerifications.Add(WithFactoryExpr(expectedFactoryFunc));
            return this;
        }

        public INegatedLifetimeVerificationBuilder WithInstance<TInstance>(TInstance instance) {
            _intermediateVerifications.Add(WithInstanceObjExpr(instance));
            return this;
        }

        public INegatedLifetimeVerificationBuilder WithInstance(Func<object, bool> instanceMatcher) {
            _intermediateVerifications.Add(WithInstanceFuncExpr(instanceMatcher));
            return this;
        }

        public IVerificationBuilder WithLifetime(ServiceLifetime lifetime) {
            _intermediateVerifications.Add(WithLifetimeExpr(lifetime));
            return this;
        }

        #endregion
    }
}