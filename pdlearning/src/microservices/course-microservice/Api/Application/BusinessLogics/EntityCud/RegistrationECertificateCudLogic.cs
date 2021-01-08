using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class RegistrationECertificateCudLogic : BaseEntityCudLogic<RegistrationECertificate>
    {
        private readonly IReadOnlyRepository<RegistrationECertificate> _readOnlyRepository;

        public RegistrationECertificateCudLogic(
            IWriteOnlyRepository<RegistrationECertificate> rootRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IReadOnlyRepository<RegistrationECertificate> readOnlyRepository) : base(rootRepository, thunderCqrs, userContext)
        {
            _readOnlyRepository = readOnlyRepository;
        }

        public async Task InsertAsync(RegistrationECertificate entity)
        {
            await RootRepository.InsertAsync(entity);
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> registrationIds)
        {
            var entities = _readOnlyRepository.GetAll().Where(p => registrationIds.Contains(p.Id));
            await RootRepository.DeleteManyAsync(entities);
        }
    }
}
