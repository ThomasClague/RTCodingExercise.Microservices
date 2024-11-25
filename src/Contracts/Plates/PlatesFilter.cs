using Contracts.Pagination;

namespace Contracts.Plates
{
    public class PlatesFilter : BaseFilter
    {
        public string? SearchTerm { get; set; }
        public string? DiscountCode { get; set; }
    }
}
