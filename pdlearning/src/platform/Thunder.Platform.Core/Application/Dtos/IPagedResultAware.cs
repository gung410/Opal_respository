namespace Thunder.Platform.Core.Application.Dtos
{
    public interface IPagedResultAware
    {
        PagedResultRequestDto PageInfo { get; set; }
    }
}
