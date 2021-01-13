using System.ComponentModel.DataAnnotations;

namespace Thunder.Platform.Core.Application.Dtos
{
    /// <summary>
    /// Simply implements <see cref="ILimitedResultRequest"/>.
    /// </summary>
    public class LimitedResultRequestDto : ILimitedResultRequest
    {
        [Range(0, int.MaxValue)]
        public virtual int MaxResultCount { get; set; } = 10;
    }
}