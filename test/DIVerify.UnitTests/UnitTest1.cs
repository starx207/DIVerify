using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Sample.AspNetCore.Api;
using Sample.AspNetCore.Api.Controllers;
using Xunit;

namespace DIVerify.UnitTests
{
    public class UnitTest1 {

        #region Private Members

        private readonly Startup _startup;
        private readonly VerifiableServiceCollection _testServices;
        private readonly Mock<IConfiguration> _mockConfig;

        #endregion

        #region Constructors

        public UnitTest1() {
            _mockConfig = new Mock<IConfiguration>();
            _startup = new Startup(_mockConfig.Object);
            _testServices = new VerifiableServiceCollection();
        }

        #endregion

        [Fact]
        public void Test1() {
            _testServices
                .Expect(typeof(IOptionsSnapshot<>))
                .ToBeRegistered()
                .AsService()
                .With(typeof(OptionsManager<>))
                .WithLifetime(ServiceLifetime.Scoped)
                .Exactly(2)
                ;
            
            _startup.ConfigureServices(_testServices);

            _testServices.VerifyExpecations();
        }
    }
}
