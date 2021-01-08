using System;

namespace Conexus.Opal.Dtos
{
    public class IdentityInfo
    {
        public Guid ExtId { get; set; }

        public int OwnerId { get; set; }

        public int CustomerId { get; set; }

        public string Archetype { get; set; }

        public int Id { get; set; }
    }
}
