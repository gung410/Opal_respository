using System;
using System.Collections.Generic;
using cxOrganization.Domain;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.WebServiceAPI.Test.Helpers
{
    public class LearnerSeedingData
    {
        public const int LEARNER_TEST_DEPARTMENT_ID = 1;
        public static void RunSeedDataForLearner(OrganizationDbContext db)
        {
            db.Users.AddRange(GetSeedingLearners());
            db.Departments.AddRange(GetSeedingDepartments());
            db.SaveChanges();
        }
        public static List<DepartmentEntity> GetSeedingDepartments()
        {
            return new List<DepartmentEntity>()
            {
                new DepartmentEntity()
                {
                    DepartmentId = LEARNER_TEST_DEPARTMENT_ID,
                    ArchetypeId = (int)ArchetypeEnum.Class,
                    City = "Test city",
                    CountryCode = 84,
                    Adress = "Test",
                    Created = DateTime.Now,
                    Deleted = null,
                    EntityStatusId = (int)EntityStatusEnum.Active,
                    CustomerId = 51,
                    Description= "Test",
                    EntityStatusReasonId = null,
                    ExtId = "LEARNER_TEST_DEPARTMENT_EXTID",
                    LanguageId = 1,
                    LastUpdatedBy = null,
                    Name = "Test",
                    Locked = 0,
                    OwnerId = 9,
                    Tag = "Tag"
                }
            };
        }
        public static List<UserEntity> GetSeedingLearners()
        {
            return new List<UserEntity>()
            {
                new UserEntity()
                {
                    UserName = "Learner test user",
                    ArchetypeId = (int)ArchetypeEnum.Learner,
                    CountryCode = 84,
                    Email = "test@email.com",
                    Created = DateTime.Now,
                    DateOfBirth = DateTime.Now,
                    SSN = "LEARNER_TEST_USER_SSN",
                    ExtId = "LEARNER_TEST_USER_EXTID",
                    FirstName = "Learner FirstName",
                    LastName = "Learner LastName",
                    Gender = (int)Gender.Male,
                    LanguageId = 1,
                    Mobile = "123456789",
                    OwnerId = 9,
                    CustomerId = 51,
                    DepartmentId = LEARNER_TEST_DEPARTMENT_ID,
                    EntityStatusId = (int)EntityStatusEnum.Active,
                    UserId = 1,
                    Tag = "test",
                    RoleId = null,
                    Deleted = null,
                    LastUpdatedBy = null,
                    EntityStatusReasonId = null,
                    Password = "Test",
                    OneTimePassword = "Test"
                }
            };
        }
    }
}
