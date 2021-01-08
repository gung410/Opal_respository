using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Uploader.Application.Models;
using Microservice.Uploader.Domain.Entities;
using Microservice.Uploader.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Uploader.Application.Queries.QueryHandlers
{
    public class GetPersonalFileByIdQueryHandler : BaseQueryHandler<GetPersonalFileByIdQuery, PersonalFileModel>
    {
        private readonly IRepository<PersonalFile> _personalFileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public GetPersonalFileByIdQueryHandler(IAccessControlContext accessControlContext, IRepository<PersonalFile> personalFileRepository, IUnitOfWorkManager unitOfWorkManager) : base(accessControlContext, unitOfWorkManager)
        {
            _personalFileRepository = personalFileRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected override async Task<PersonalFileModel> HandleAsync(GetPersonalFileByIdQuery query, CancellationToken cancellationToken)
        {
            var fileQuery = _personalFileRepository
                .GetAll()
                .Where(PersonalFileExpressions.HasPermissionToSeePersonalFile(CurrentUserId));

            var file = await fileQuery.FirstOrDefaultAsync(file => file.Id == query.Id);

            if (file == null)
            {
                throw new PersonalFileAccessDeniedException();
            }

            var fileModel = new PersonalFileModel(file);

            return fileModel;
        }
    }
}
