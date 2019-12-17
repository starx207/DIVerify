using System;
using Microsoft.Extensions.DependencyInjection;

namespace DIVerify
{
    public class Verification : IVerification {

        #region Public Properties
            
        public Func<IServiceCollection, bool>? ServiceVerification { get; set; }

        #endregion

        #region Public Methods

        public VerificationResult Verify(IServiceCollection services, string failureMessage) {
            if (ServiceVerification is null) {
                throw new InvalidOperationException("No verification defined");
            }
            return ServiceVerification(services)
                ? VerificationResult.Successful
                : VerificationResult.Failure(failureMessage);
        }

        #endregion
    }
}
