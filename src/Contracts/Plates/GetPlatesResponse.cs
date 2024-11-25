using Contracts.Pagination;

namespace Contracts.Plates
{
    public class GetPlatesResponse 
    {
        public PagedResponse<PlateDTO> PagedResponse { get; set; }
    }
}
