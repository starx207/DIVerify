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
        INegatedServiceVerificationBuilder {
        
        #region Private Members
            
        private Expression<Func<ServiceDescriptor, bool>> _initialVerification;
        private readonly List<Expression<Func<ServiceDescriptor, bool>>> _intermediateVerifications;
        private readonly Expression<Func<IEnumerable<ServiceDescriptor>, bool>> _finalVerification;

        #endregion

        #region Constructors

        public NegatedVerificationBuilder(Type typeToVerify)
            : base(typeToVerify) {
            
            _initialVerification = s => s.ServiceType == TypeToVerify || s.ImplementationType == TypeToVerify;
            _finalVerification = d => d.Count() == 0;
            _intermediateVerifications = new List<Expression<Func<ServiceDescriptor, bool>>>();
            DefaultMessage = $"{typeToVerify.Name} registered";
        }

        #endregion

        #region Public Methods

        public INegatedImplementationVerificationBuilder AsImplementation() {
            DefaultMessage += " as an implementation";
            _initialVerification = AsImplementationExpr();
            return this;
        }

        public INegatedLifetimeVerificationBuilder AsSelf() {
            DefaultMessage += " as itself";
            _initialVerification = AsSelfExpr();
            return this;
        }

        public INegatedServiceVerificationBuilder AsService() {
            DefaultMessage += " as a service";
            _initialVerification = AsServiceExpr();
            return this;
        }

        public override IVerification Build() {
            DefaultMessage += " when to shouldn't have been.";

            var descriptorExpr = _intermediateVerifications.Count > 0
                ? _initialVerification.CombineWithAnd(_intermediateVerifications.Aggregate((left, right) => left.CombineWithAnd(right)))
                : _initialVerification;

            return new Verification() {
                ServiceVerification = svc => {
                    var descriptors = svc.Where(descriptorExpr.Compile());
                    return _finalVerification.Compile().Invoke(descriptors);
                }
            };
        }

        public INegatedLifetimeVerificationBuilder For(Type serviceType) {
            DefaultMessage += $" for service {serviceType.Name}";
            _intermediateVerifications.Add(ForExpr(serviceType));
            return this;
        }

        public INegatedLifetimeVerificationBuilder With(Type implementationType) {
            DefaultMessage += $" with implementation {implementationType.Name}";
            _intermediateVerifications.Add(WithExpr(implementationType));
            return this;
        }

        public INegatedLifetimeVerificationBuilder WithFactory(Func<IServiceProvider, object> expectedFactoryFunc) {
            DefaultMessage += " with the given factory";
            _intermediateVerifications.Add(WithFactoryExpr(expectedFactoryFunc));
            return this;
        }

        public INegatedLifetimeVerificationBuilder WithInstance<TInstance>(TInstance instance) {
            DefaultMessage += " with the given instance";
            _intermediateVerifications.Add(WithInstanceObjExpr(instance));
            return this;
        }

        public INegatedLifetimeVerificationBuilder WithInstance(Func<object, bool> instanceMatcher) {
            DefaultMessage += " with a matching instance";
            _intermediateVerifications.Add(WithInstanceFuncExpr(instanceMatcher));
            return this;
        }

        public IVerificationBuilder WithLifetime(ServiceLifetime lifetime) {
            DefaultMessage += $" with {lifetime} lifetime";
            _intermediateVerifications.Add(WithLifetimeExpr(lifetime));
            return this;
        }

        public IVerificationBuilder WithFailureMessage(string failureMessage) {
            FailureMessage = failureMessage;
            return this;
        }

        #endregion
    }
}
