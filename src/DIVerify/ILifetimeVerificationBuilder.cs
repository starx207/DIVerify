using Microsoft.Extensions.DependencyInjection;

namespace DIVerify {
    public interface ILifetimeVerificationBuilder {
        ICountVerificationBuilder WithLifetime(ServiceLifetime lifetime);
    }

    public interface INegatedLifetimeVerificationBuilder {
        IVerificationBuilder WithLifetime(ServiceLifetime lifetime);
    }
}