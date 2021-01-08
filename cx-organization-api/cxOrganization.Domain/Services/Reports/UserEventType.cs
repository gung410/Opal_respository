namespace cxOrganization.Domain.Services.Reports
{
    public enum UserEventType
    {
        Unknown,
        RegisteredSuccess,
        DeletedSuccess,
        LoginSuccess,
        LoginFail,
        LogoutSuccess,
        LogoutFail,
        Locked,
        ResetPasswordSuccess,
        ChangePasswordSuccess,
        SetPasswordSuccess,
        ForgetPasswordSuccess,
        ForgetPasswordFail,
        ClaimOtpEmailSuccess,
        ClaimOtpEmailFail,
        ClaimOtpPhoneSuccess,
        ClaimOtPhoneFail,
        Claim2faSuccess,
        Claim2faFail,
        Enable2faSuccess,
        Disable2faSuccess,
        Verify2faSuccess,
        Verify2faFail

    }
}