namespace cxOrganization.Client.Account
{
    public class LogonResponseDto : AccountBaseDto
    {
        public SignInStatus SignInStatus { get; set; }
        public string Message { get; set; }
    }
}
