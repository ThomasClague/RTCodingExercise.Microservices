using Contracts.Pagination;
using Contracts.Plates;

namespace WebMVC.Models
{
    public class PlatesViewModel
    {
        public PagedResponse<PlateDTO>? PagedResponse { get; set; }
        public PlatesFilter PlatesFilter { get; set; } = new();
    }
}
