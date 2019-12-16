namespace DIVerify {
    public interface IRegistrationVerificationBuilder {
        ITypeVerificationBuilder ToBeRegistered();
        INegatedTypeVerificationBuilder NotToBeRegistered();
    }
}