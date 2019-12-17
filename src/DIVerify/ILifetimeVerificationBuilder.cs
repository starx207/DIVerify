using Microsoft.Extensions.DependencyInjection;

namespace DIVerify {
    public interface ILifetimeVerificationBuilder : IVerificationBuilder {
        ICountVerificationBuilder WithLifetime(ServiceLifetime lifetime);
    }

    public interface INegatedLifetimeVerificationBuilder : IVerificationBuilder {
        IVerificationBuilder WithLifetime(ServiceLifetime lifetime);
    }
}
