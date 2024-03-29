﻿namespace DIVerify {
    public interface ICountVerificationBuilder : IVerificationBuilder {
        ILifetimeVerificationBuilder AtLeast(int minimumRegistrations);
        ILifetimeVerificationBuilder AtLeastOnce() => AtLeast(1);
        ILifetimeVerificationBuilder AtMost(int maximumRegistrations);
        ILifetimeVerificationBuilder Exactly(int numRegistrations);
        ILifetimeVerificationBuilder Once() => Exactly(1);
    }
}
