namespace Communication.Business.Models.FirebaseCloudMessage
{
    using Newtonsoft.Json.Linq;
    public class FirebaseAppInstancesInfo
    {
        public string Application { get; set; }
        public string AuthorizedEntity { get; set; }
        public string Platform { get; set; }
        public string AttestStatus { get; set; }
        public string AppSigner { get; set; }
        public string ConnectionType { get; set; }
        public Relation Rel { get; set; }

    }
    public class Relation
    {
        public JObject Topics { get; set; }
    }
}