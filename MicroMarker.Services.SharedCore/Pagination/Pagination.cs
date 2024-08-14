using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.SharedCore.Pagination
{
    public static class Pagination<T>
    {
        public class Page
        {
            public ICollection<T> Items { get; init; } = new List<T>();
            public int TotalItemsCount { get; init; }
            public int ItemsPerPage { get; init; }
            public int PageNumber { get; init; }
            public bool HasNextPage { get; init; }
            public bool HasPreviousPage { get; init; }

            public Pagination<TResult>.Page ConvertTo<TResult>(Func<T, TResult> converter)
            {
                return new Pagination<TResult>.Page()
                {
                    Items = Items.Select(i => converter(i)).ToList(),
                    TotalItemsCount = TotalItemsCount,
                    ItemsPerPage = ItemsPerPage,
                    PageNumber = PageNumber,
                    HasNextPage = HasNextPage,
                    HasPreviousPage = HasPreviousPage
                };
            }
        }

        public static async Task<Result<Page>> Paginate(IQueryable<T> query, int page, int entriesPerPage)
        {
            if (page < 0 || entriesPerPage <= 0)
                return Result.Failure<Page>("Page number and items per page must be positive value");

            var count = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (double)entriesPerPage);

            if (page >= totalPages)
                return Result.Failure<Page>("Requested page doesn't exist");

            var items = await query
                .Skip(page * entriesPerPage)
                .Take(entriesPerPage)
                .ToListAsync();

            return Result.Success(new Page()
            {
                Items = items,
                TotalItemsCount = count,
                ItemsPerPage = entriesPerPage,
                PageNumber = page,
                HasNextPage = page < totalPages - 1,
                HasPreviousPage = page > 0
            });
        }
    }
}
