namespace DIVerify
{
    public interface ITypeVerificationBuilder : 
        IGenericTypeVerificationBuilder<IImplementationVerificationBuilder, IServiceVerificationBuilder, ILifetimeVerificationBuilder>,
        IDescriptorVerificationBuilder {
    }

    public interface INegatedTypeVerificationBuilder : 
        IGenericTypeVerificationBuilder<INegatedImplementationVerificationBuilder, INegatedServiceVerificationBuilder, INegatedLifetimeVerificationBuilder>,
        INegatedLifetimeVerificationBuilder {
    }

    public interface IGenericTypeVerificationBuilder<TImplementationReturnType, TServiceReturnType, TLifetimeReturnType>
        where TImplementationReturnType : class
        where TServiceReturnType : class
        where TLifetimeReturnType : class {
        
        TImplementationReturnType AsImplementation();
        TLifetimeReturnType AsSelf();
        TServiceReturnType AsService();
    }
}