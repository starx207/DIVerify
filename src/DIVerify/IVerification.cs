using Microsoft.Extensions.DependencyInjection;

namespace DIVerify {
    public interface IVerification {
        VerificationResult Verify(IServiceCollection services, string failureMessage);
    }
}
