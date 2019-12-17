using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DIVerify {
    public abstract class VerificationBuilderBase {

        protected Type TypeToVerify;
        public string? FailureMessage { get; set; }
        public string DefaultMessage { get; protected set; } = "Verification Failed";

        public VerificationBuilderBase(Type typeToVerify) => TypeToVerify = typeToVerify;

        public abstract IVerification Build();

        protected Expression<Func<ServiceDescriptor, bool>> AsImplementationExpr()
            => s => s.ImplementationType == TypeToVerify;

        protected Expression<Func<ServiceDescriptor, bool>> AsSelfExpr()
            => s => s.ServiceType == TypeToVerify && s.ImplementationType == TypeToVerify;

        protected Expression<Func<ServiceDescriptor, bool>> AsServiceExpr()
            => s => s.ServiceType == TypeToVerify;

        protected Expression<Func<ServiceDescriptor, bool>> ForExpr(Type serviceType)
            => s => s.ServiceType == serviceType;

        protected Expression<Func<ServiceDescriptor, bool>> WithExpr(Type implementationType)
            => s => s.ImplementationType == implementationType;


        protected Expression<Func<ServiceDescriptor, bool>> WithFactoryExpr(Func<IServiceProvider, object> expectedFactory) {
            // TODO: figure out how to check if the factory functions are equal
#pragma warning disable IDE0022 // Use expression body for methods (Disabled because this method is not fully implemented. Need to test some stuff)
            return s => s.ImplementationFactory == expectedFactory;
#pragma warning restore IDE0022 // Use expression body for methods
        }

        protected Expression<Func<ServiceDescriptor, bool>> WithInstanceObjExpr<TInstance>(TInstance instance)
            => s => s.ImplementationInstance != null && s.ImplementationInstance.Equals(instance);

        protected Expression<Func<ServiceDescriptor, bool>> WithInstanceFuncExpr(Func<object, bool> instanceMatch)
            => s => s.ImplementationInstance != null && instanceMatch(s.ImplementationInstance);

        protected Expression<Func<ServiceDescriptor, bool>> WithLifetimeExpr(ServiceLifetime lifetime)
            => s => s.Lifetime == lifetime;
    }
}
