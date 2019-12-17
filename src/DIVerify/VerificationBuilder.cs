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
        IServiceVerificationBuilder {
        
        #region Private Members
            
        private Expression<Func<ServiceDescriptor, bool>> _initialVerification;
        private readonly List<Expression<Func<ServiceDescriptor, bool>>> _intermediateVerifications;
        private Expression<Func<IEnumerable<ServiceDescriptor>, bool>> _finalVerification;
        private int? _minCount;
        private int? _maxCount;

        #endregion

        #region Constructors

        public VerificationBuilder(Type typeToVerify)
            : base(typeToVerify) {
            
            _initialVerification = s => s.ServiceType == TypeToVerify || s.ImplementationType == TypeToVerify;
            _finalVerification = d => d.Count() > 0;
            _intermediateVerifications = new List<Expression<Func<ServiceDescriptor, bool>>>();
            DefaultMessage = $"{typeToVerify.Name} not registered";
        }

        #endregion

        #region Public Methods

        public IImplementationVerificationBuilder AsImplementation() {
            DefaultMessage += " as an implementation";
            _initialVerification = AsImplementationExpr();
            return this;
        }

        public ILifetimeVerificationBuilder AsSelf() {
            DefaultMessage += " as itself";
            _initialVerification = AsSelfExpr();
            return this;
        }

        public IServiceVerificationBuilder AsService() {
            DefaultMessage += " as a service";
            _initialVerification = AsServiceExpr();
            return this;
        }

        public ILifetimeVerificationBuilder AtLeast(int minimumRegistrations) {
            _minCount = minimumRegistrations;
            _maxCount = null;
            _finalVerification = d => d.Count() >= minimumRegistrations;
            return this;
        }

        public ILifetimeVerificationBuilder AtMost(int maximumRegistrations) {
            _maxCount = maximumRegistrations;
            _minCount = null;
            _finalVerification = d => d.Count() <= maximumRegistrations;
            return this;
        }

        public override IVerification Build() {
            // TODO: This method is being called multiple times. Perhaps because of the way I'm returning "this" all the time?
            var timesMsg = (_minCount, _maxCount) switch
            {
                (null, null) => string.Empty,
                (null, { } max) => $" at most {FormatNumberForMessage(max)}",
                ({ } min, null) => $" at least {FormatNumberForMessage(min)}",
                ({ } min, { } max) => $" {FormatNumberForMessage(min)}"
            };
            DefaultMessage += timesMsg + ".";

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

        public ILifetimeVerificationBuilder Exactly(int numRegistrations) {
            _minCount = numRegistrations;
            _maxCount = numRegistrations;
            _finalVerification = d => d.Count() == numRegistrations;
            return this;
        }

        public IDescriptorVerificationBuilder For(Type serviceType) {
            DefaultMessage += $" for service {serviceType.Name}";
            _intermediateVerifications.Add(ForExpr(serviceType));
            return this;
        }

        public IDescriptorVerificationBuilder With(Type implementationType) {
            DefaultMessage += $" with implementation {implementationType.Name}";
            _intermediateVerifications.Add(WithExpr(implementationType));
            return this;
        }

        public IDescriptorVerificationBuilder WithFactory(Func<IServiceProvider, object> expectedFactoryFunc) {
            DefaultMessage += " with the given factory";
            _intermediateVerifications.Add(WithFactoryExpr(expectedFactoryFunc));
            return this;
        }

        public IDescriptorVerificationBuilder WithInstance<TInstance>(TInstance instance) {
            DefaultMessage += " with the given instance";
            _intermediateVerifications.Add(WithInstanceObjExpr(instance));
            return this;
        }

        public IDescriptorVerificationBuilder WithInstance(Func<object, bool> instanceMatcher) {
            DefaultMessage += " with a matching instance";
            _intermediateVerifications.Add(WithInstanceFuncExpr(instanceMatcher));
            return this;
        }

        public ICountVerificationBuilder WithLifetime(ServiceLifetime lifetime) {
            DefaultMessage += $" with {lifetime} lifetime";
            _intermediateVerifications.Add(WithLifetimeExpr(lifetime));
            return this;
        }

        public IVerificationBuilder WithFailureMessage(string failureMessage) {
            FailureMessage = failureMessage;
            return this;
        }

        #endregion

        #region Private Helpers

        private string FormatNumberForMessage(int num)
            => num switch
            {
                1 => "once",
                2 => "twice",
                _ => $"{num} times"
            };

        #endregion
    }
}
