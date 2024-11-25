using Ardalis.Specification;
using Audit.API.Models;

namespace Audit.API.Specifications
{
    public class GetPlatePurchaseRecordsSpecification : Specification<AuditRecord, string>
    {
        public GetPlatePurchaseRecordsSpecification()
        {
            Query.Where(x => x.EventType == "PlateSoldEvent");

            Query.Select(x => x.Data);
        }
    }
} 