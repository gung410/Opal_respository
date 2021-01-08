using System.Threading.Tasks;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface IGetBadgeCriteriaLogic<T> where T : BaseBadgeCriteria
    {
        /// <summary>
        /// Return badge criteria value from given badge model.
        /// </summary>
        /// <returns>Badge criteria with value.</returns>
        Task<T> ExecuteAsync();
    }
}
