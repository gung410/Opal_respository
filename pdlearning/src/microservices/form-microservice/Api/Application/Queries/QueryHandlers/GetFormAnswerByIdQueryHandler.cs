using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Queries
{
    public class GetFormAnswerByIdQueryHandler : BaseQueryHandler<GetFormAnswerByIdQuery, FormAnswerModel>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormAnswer> _formAnswerRepository;
        private readonly IRepository<FormQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IRepository<FormAnswerAttachment> _formAttachmentRepository;

        public GetFormAnswerByIdQueryHandler(
            IAccessControlContext accessControlContext,
            IRepository<FormEntity> formRepository,
            IRepository<FormAnswer> formAnswerRepository,
            IRepository<FormQuestionAnswer> formQuestionAnswerRepository,
            IRepository<FormAnswerAttachment> formAttachmentRepository) : base(accessControlContext)
        {
            _formRepository = formRepository;
            _formAnswerRepository = formAnswerRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _formAttachmentRepository = formAttachmentRepository;
        }

        protected override async Task<FormAnswerModel> HandleAsync(GetFormAnswerByIdQuery query, CancellationToken cancellationToken)
        {
            var formAnswer = await _formAnswerRepository
                .GetAll()
                .Where(p => p.Id == query.FormAnswerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (formAnswer is null)
            {
                throw new FormAccessDeniedException();
            }

            if (!(await _formRepository.GetAll()
                .ApplyAccessControl(AccessControlContext, (p) => formAnswer.CreatedBy == query.UserId)
                .AnyAsync(p => p.Id == formAnswer.FormId, cancellationToken)))
            {
                throw new FormAccessDeniedException();
            }

            var formQuestionAnswers = await _formQuestionAnswerRepository
                .GetAll()
                .Where(p => p.FormAnswerId == query.FormAnswerId)
                .ToListAsync(cancellationToken);

            var formQuestionAnswersIds = formQuestionAnswers.Select(qa => qa.Id);

            var formAnswerAttachments = await _formAttachmentRepository.GetAll()
                .Where(p => formQuestionAnswersIds.Contains(p.FormQuestionAnswerId))
                .ToListAsync(cancellationToken);

            var formAnswerAttachmentModels = formAnswerAttachments.Select(x => new FormAnswerAttachmentModel(x));

            var formQuestionAnswerModels = formQuestionAnswers
                .Select(p => new FormQuestionAnswerModel(p, formAnswerAttachmentModels.Where(x => x.FormQuestionAnswerId == p.Id).ToList()));

            return new FormAnswerModel(formAnswer, formQuestionAnswerModels);
        }
    }
}
