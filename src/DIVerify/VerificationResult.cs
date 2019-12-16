namespace DIVerify {
    public struct VerificationResult
    {
        public bool Success { get; }

        public string? FailureMessage { get; }

        private VerificationResult(bool success, string? failureMessage) {
            Success = success;
            FailureMessage = failureMessage;
        }

        public static readonly VerificationResult Successful = new VerificationResult(true, null);

        public static VerificationResult Failure(string failureMessage) => new VerificationResult(false, failureMessage);
    }
}