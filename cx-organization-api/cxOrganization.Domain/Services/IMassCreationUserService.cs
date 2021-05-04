
using cxOrganization.Domain.Dtos.Users;
using System.IO;
using System.Threading.Tasks;
using cxPlatform.Core;
using System.Collections.Generic;
using cxOrganization.Domain.AdvancedWorkContext;

namespace cxOrganization.Domain.Services
{
    public interface IMassCreationUserService
    {
        MassUserCreationValidationResultDto ValidateMassUserCreationFile(Stream fileOnMemory, string fileName);
        Task<MassUserCreationValidationResultDto> ValidateMassUserCreationData(
            IAdvancedWorkContext workContext,
            Stream fileOnMemory,
            string fileName);

        Task<List<UserGenericDto>> getUsersFromFileAsync(
            MassUserCreationValidationContract massUserCreationValidationContract,
            Stream fileOnMemory,
            IAdvancedWorkContext workContext);
        int GetNumberOfUserCreationRecord(Stream fileOnMemory);
    }
}
