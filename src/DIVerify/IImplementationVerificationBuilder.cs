using System;

namespace DIVerify {
    public interface IImplementationVerificationBuilder : 
        IGenericImplementationVerificationBuilder<IDescriptorVerificationBuilder>, 
        IDescriptorVerificationBuilder {
    }

    public interface INegatedImplementationVerificationBuilder : 
        IGenericImplementationVerificationBuilder<INegatedLifetimeVerificationBuilder>, 
        INegatedLifetimeVerificationBuilder {
    }

    public interface IGenericImplementationVerificationBuilder<TReturnType>
        where TReturnType : class {
        
        TReturnType For(Type serviceType);
        TReturnType For<TService>() where TService : class => For(typeof(TService));
    }
}