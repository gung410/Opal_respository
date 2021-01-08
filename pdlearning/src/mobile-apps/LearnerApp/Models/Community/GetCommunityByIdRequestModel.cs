namespace LearnerApp.Models
{
    public class GetCommunityByIdRequestModel
    {
        public GetCommunityByIdRequestModel(string[] guids)
        {
            Guids = guids;
        }

        public string[] Guids { get; set; }
    }
}
