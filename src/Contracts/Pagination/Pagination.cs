using Newtonsoft.Json;

namespace Contracts.Pagination
{
    public class Pagination
    {
        [JsonIgnore]
        private readonly PaginationSettings _settings;

        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int StartItem { get; set; }
        public int EndItem { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }

        [JsonIgnore]
        public int Take { get; }

        [JsonIgnore]
        public int Skip { get; }

        public Pagination(int totalItems, int totalPages, int pageSize, int page, int startItem, int endItem, bool hasPrevious, bool hasNext)
        {
            _settings = new PaginationSettings(20, 100);
            TotalItems = totalItems;
            TotalPages = totalPages;
            PageSize = pageSize;
            Page = page;
            StartItem = startItem;
            EndItem = endItem;
            HasPrevious = hasPrevious;
            HasNext = hasNext;
        }

        public Pagination(int itemsCount, BaseFilter baseFilter)
            : this(new PaginationSettings(20, 100), itemsCount, baseFilter.PageSize, baseFilter.Page)
        {
        }

        public Pagination(PaginationSettings paginationSettings, int itemsCount, BaseFilter baseFilter)
            : this(paginationSettings, itemsCount, baseFilter.PageSize, baseFilter.Page)
        {
        }

        public Pagination(PaginationSettings paginationSettings, int itemsCount, int? pageSize, int? page)
        {
            _settings = paginationSettings;

            TotalItems = GetHandledTotalItems(itemsCount);
            PageSize = GetHandledPageSize(pageSize);
            TotalPages = GetHandledTotalPages();
            Page = GetHandledPage(page);
            HasNext = CalculateHasNext();
            HasPrevious = CalculateHasPrevious();
            StartItem = CalculateStartItem();
            EndItem = CalculateEndItem();

            Take = PageSize;
            Skip = (Page - 1) * PageSize;
        }

        public Pagination()
        {
            
        }

        private bool CalculateHasNext()
        {
            return Page != TotalPages;
        }

        private bool CalculateHasPrevious()
        {
            return Page != 1;
        }

        private int CalculateStartItem()
        {
            return TotalItems == 0
                ? 0
                : (Page - 1) * PageSize + 1;
        }

        private int CalculateEndItem()
        {
            return PageSize * Page > TotalItems
                ? TotalItems
                : PageSize * Page;
        }

        private int GetHandledTotalPages()
        {
            return TotalItems == 0 ? 1 : (int)Math.Ceiling((decimal)TotalItems / GetHandledPageSize(PageSize));
        }

        private int GetHandledPageSize(int? pageSize)
        {
            if (!pageSize.HasValue || pageSize <= 0) return _settings.DefaultPageSize;
            
            if (pageSize > _settings.DefaultPageSizeLimit) return _settings.DefaultPageSizeLimit;

            return pageSize.Value;
        }

        private int GetHandledPage(int? page)
        {
            if (!page.HasValue || page <= 0) return _settings.DefaultPage;

            if (page > TotalPages) return TotalPages;

            return page.Value;
        }

        private int GetHandledTotalItems(int itemsCount)
        {
            return itemsCount < 0 ? 0 : itemsCount;
        }
    }
}
