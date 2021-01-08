using System;
using System.Collections.Generic;

namespace Communication.Business.HttpClients
{
    public class UserIdmResponseInfo
    {
        public PagingHeader pagingHeader { get; set; }
        public List<UserIdmDto> Items { get; set; }
    }


    public class UserIdmDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public string[] Role { get; set; }
        public string[] Roles { get; set; }
        public string[] Orgs { get; set; }
        public string[] AccessPlans { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public short? Gender { get; set; }
        public bool IsLocked { get; set; }
        public long Status { get; set; }
        public string EncryptedNric { get; set; }
    }

    public class PagingHeader
    {
        public int totalItems { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
    }
}