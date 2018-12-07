using System.Collections.Generic;

namespace Alma.Core
{
    public interface IPagedList<T> : IPagedList where T : class
    {
        IReadOnlyList<T> List { get; }
    }

    public interface IPagedList
    {
        int CurrentPage { get; }
        int PageSize { get; }
        long TotalCount { get; }
    }
}