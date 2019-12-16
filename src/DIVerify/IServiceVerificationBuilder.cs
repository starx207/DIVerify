using System;

namespace DIVerify {
    public interface IServiceVerificationBuilder : 
        IGenericServiceVerificationBuilder<IDescriptorVerificationBuilder>,
        IDescriptorVerificationBuilder {
    }

    public interface INegatedServiceVerificationBuilder : 
        IGenericServiceVerificationBuilder<INegatedLifetimeVerificationBuilder>,
        INegatedLifetimeVerificationBuilder {
    }

    public interface IGenericServiceVerificationBuilder<TReturnType>
        where TReturnType : class {
        
        TReturnType With(Type implementationType);
        TReturnType With<TImplementation>() where TImplementation : class => With(typeof(TImplementation));
        TReturnType WithFactory(Func<IServiceProvider, object> expectedFactoryFunc);
        TReturnType WithInstance<TInstance>(TInstance instance);
        TReturnType WithInstance(Func<object, bool> instanceMatcher);   
    }
}