using Microsoft.EntityFrameworkCore;
using CSharpFunctionalExtensions;

namespace MicroMarket.Services.SharedCore.Pagination
{
    public static class Pagination<T>
    {
        public class PaginatedList
        {
            public ICollection<T> Items { get; init; }
            public int ItemsCount { get; init; }
            public int ItemsOnPage { get; init; }
            public int PageNumber { get; init; }
            public bool HasNextPage { get; init; }
            public bool HasPreviousPage { get; init; }
        }

        public static async Task<Result<PaginatedList>> Paginate(IQueryable<T> query, int page, int entriesPerPage)
        {
            if ( page < 0 || entriesPerPage <= 0 )
                return Result.Failure<PaginatedList>("Page number and items per page must be positive value");

            var count = await query.CountAsync();
            if (count / entriesPerPage < page)
                return Result.Failure<PaginatedList>("Requested page doesn't exist");

            var items = await query
                .Skip(page * entriesPerPage)
                .Take(entriesPerPage)
                .ToListAsync();
            return new PaginatedList()
            {
                Items = items,
                ItemsCount = count,
                ItemsOnPage = entriesPerPage,
                PageNumber = page,
                HasNextPage = (count / entriesPerPage) > (page + 1),
                HasPreviousPage = page != 0
            };
        }
    }
}
