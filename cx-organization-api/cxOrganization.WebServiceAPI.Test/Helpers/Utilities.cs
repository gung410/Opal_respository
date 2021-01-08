using System;
using System.Collections.Generic;
using cxOrganization.Domain;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.WebServiceAPI.Test.Helpers
{
    public static class Utilities
    {
        public static void InitializeDbForTests(OrganizationDbContext db)
        {
            db.LanguageEntities.AddRange(GetSeedingLanguages());
            db.SaveChanges();
            LearnerSeedingData.RunSeedDataForLearner(db);
        }

        public static List<LanguageEntity> GetSeedingLanguages()
        {
            return new List<LanguageEntity>()
            {
                new LanguageEntity(){ LanguageId = 1, LanguageCode ="en-US", Name= "Englsish" }
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
                    DepartmentId = 1,
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
