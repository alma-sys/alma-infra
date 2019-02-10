namespace Alma.ApiExtensions.Controllers
{
    public abstract class RequestBase
    {
        public RequestBase()
        {
            Page = 1;
            PageSize = 10;
            PagedResult = true;
        }
        public int Page { get; set; }

        public bool PagedResult { get; set; }
        public int PageSize { get; set; }
    }
}
