using Ardalis.Specification;
using Catalog.API.ReadModels;
using Contracts.Plates;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Specifications.Plates
{
    public class GetPlatesSpecification : Specification<Plate>
    {
        private readonly int maxDistance = 3;

        public GetPlatesSpecification(PlatesFilter filter, bool isAdmin)
        {
            Query.AsNoTracking();
            Query.Where(
                p => p.StatusId == (int)PlateStatusEnum.Available ||
                     p.StatusId == (int)PlateStatusEnum.Reserved,
                !isAdmin);

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                Query.AsNoTracking();
                Query.Where(p =>
                    (
                        // Contains score (0.3 weight)
                        (EF.Functions.Like(p.Registration, $"%{filter.SearchTerm}%") ? 1.0 : 0.0) * 0.3 +

                        // Letters Levenshtein (highest weight - 0.3)
                        (p.Letters != null ?
                            (1.0 - (double)CustomDbFunctions.Levenshtein(p.Letters, filter.SearchTerm, maxDistance) / maxDistance) * 0.3 :
                            0.0) +

                        // Regular Levenshtein score (0.2 weight)
                        (1.0 - (CustomDbFunctions.Levenshtein(p.Registration, filter.SearchTerm, maxDistance) / maxDistance) * 0.2 +

                        // Normalized Levenshtein score (0.2 weight)
                        (1.0 - (CustomDbFunctions.Levenshtein(
                            CustomDbFunctions.NormalizePlateNumber(p.Registration),
                            CustomDbFunctions.NormalizePlateNumber(filter.SearchTerm), maxDistance) / maxDistance) * 0.2)
                    ) >= 0.3)); // Minimum combined score threshold

                // Set the filter to order by the score if no sort order provided
                filter.SortBy ??= "matchScore";
            }

            AppySorting(filter.SearchTerm, filter.SortBy, filter.OrderBy);

            ApplyDiscountCode(filter.DiscountCode);
        }

        private void ApplyDiscountCode(string? discountCode)
        {
            Query.PostProcessingAction(x => x.Select(plate =>
            {
                plate.ApplyDiscount(discountCode);
                return plate;
            })
            .ToList());
        }

        private void AppySorting(string? searchTerm, string? sortColumn, string? sortDirection)
        {
            if (sortColumn is null)
            {
                Query.OrderBy(o => o.Id);
                return;
            }

            var isAscending = !(sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false);

            _ = sortColumn switch
            {
                "salePrice" => isAscending ? Query.OrderBy(x => x.SalePrice) : Query.OrderByDescending(x => x.SalePrice),
                "purchasePrice" => isAscending ? Query.OrderBy(x => x.PurchasePrice) : Query.OrderByDescending(x => x.PurchasePrice),
                "status" => isAscending ? Query.OrderBy(x => x.Status) : Query.OrderByDescending(x => x.Status),
                "matchScore" when !string.IsNullOrWhiteSpace(searchTerm) =>
                    Query.OrderByDescending(p =>
                        (EF.Functions.Like(p.Registration, $"%{searchTerm}%") ? 1.0 : 0.0) * 0.3 +
                        (p.Letters != null ?
                            (1.0 - (double)CustomDbFunctions.Levenshtein(p.Letters, searchTerm, maxDistance) / maxDistance) * 0.3 :
                            0.0) +
                        (1.0 - (double)CustomDbFunctions.Levenshtein(p.Registration, searchTerm, maxDistance) / maxDistance) * 0.2 +
                        (1.0 - (double)CustomDbFunctions.Levenshtein(
                            CustomDbFunctions.NormalizePlateNumber(p.Registration),
                            CustomDbFunctions.NormalizePlateNumber(searchTerm),
                            maxDistance) / maxDistance) * 0.2),
                _ => Query.OrderBy(p => p.Id)
            };
        }
    }
}
