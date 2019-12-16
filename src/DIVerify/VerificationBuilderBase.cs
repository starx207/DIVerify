using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace DIVerify {
    public abstract class VerificationBuilderBase {

        protected Type TypeToVerify;

        public VerificationBuilderBase(Type typeToVerify) => TypeToVerify = typeToVerify;

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> AsImplementationExpr()
            => (svc => svc.Where(s => s.ImplementationType == TypeToVerify));

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> AsSelfExpr()
            => (svc => svc.Where(s => s.ServiceType == TypeToVerify && s.ImplementationType == TypeToVerify));

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> AsServiceExpr()
            => (svc => svc.Where(s => s.ServiceType == TypeToVerify));

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> ForExpr(Type serviceType)
            => (svc => svc.Where(s => s.ServiceType == serviceType));

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> WithExpr(Type implementationType)
            => (svc => svc.Where(s => s.ImplementationType == implementationType));

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> WithFactoryExpr(Func<IServiceProvider, object> expectedFactory) {
            // TODO: figure out how to check if the factory functions are equal
            return (svc => svc.Where(s => s.ImplementationFactory == expectedFactory));
        }

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> WithInstanceObjExpr<TInstance>(TInstance instance)
            => (svc => svc.Where(s => s.ImplementationInstance != null && s.ImplementationInstance.Equals(instance)));

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> WithInstanceFuncExpr(Func<object, bool> instanceMatch)
            => (svc => svc.Where(s => s.ImplementationInstance != null && instanceMatch(s.ImplementationInstance)));

        protected Expression<Func<IServiceCollection, IEnumerable<ServiceDescriptor>>> WithLifetimeExpr(ServiceLifetime lifetime)
            => (svc => svc.Where(s => s.Lifetime == lifetime));

    }
}