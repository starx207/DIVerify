using System;

namespace DIVerify {
    public class VerificationBuilderFactory : IRegistrationVerificationBuilder {

        #region Private Members

        private readonly Type _typeToVerify;
        private Action<VerificationBuilderBase>? _callbackMethod;    
            
        #endregion

        #region Constructors
            
        public VerificationBuilderFactory(Type typeToVerify) => _typeToVerify = typeToVerify;

        #endregion

        #region Public Methods

        public INegatedTypeVerificationBuilder NotToBeRegistered() {
            var builder = new NegatedVerificationBuilder(_typeToVerify);
            if (_callbackMethod is { }) {
                _callbackMethod(builder);
            }
            return builder;
        }

        public ITypeVerificationBuilder ToBeRegistered() {
            var builder = new VerificationBuilder(_typeToVerify);
            if (_callbackMethod is { }) {
                _callbackMethod(builder);
            }
            return builder;
        }

        public void Callback(Action<VerificationBuilderBase> callbackMethod) => _callbackMethod = callbackMethod;

        #endregion
    }
}
