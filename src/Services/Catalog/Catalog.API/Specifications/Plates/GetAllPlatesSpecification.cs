using Ardalis.Specification;

namespace Catalog.API.Specifications.Plates
{
    public class GetAllPlatesSpecification : Specification<Plate>
    {
        public GetAllPlatesSpecification()
        {
            Query.AsNoTracking();
        }
    }
} 