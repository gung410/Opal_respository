namespace cxOrganization.Client.Account
{
    public class LogonRequestDto : AccountBaseDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
