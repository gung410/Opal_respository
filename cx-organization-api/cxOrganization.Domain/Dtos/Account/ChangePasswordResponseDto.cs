namespace cxOrganization.Client.Account
{
    public class ChangePasswordResponseDto : AccountBaseDto
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public int UserId { get; set; }
    }
}
