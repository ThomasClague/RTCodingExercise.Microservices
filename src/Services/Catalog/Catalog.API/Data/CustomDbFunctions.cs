using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public static class CustomDbFunctions
    {
        [DbFunction("Levenshtein", Schema = "dbo")]
        public static int Levenshtein(string source, string target, int maxDistance)
            => throw new NotSupportedException();

        [DbFunction("NormalizePlateNumber", Schema = "dbo")]
        public static string NormalizePlateNumber(string plate)
            => throw new NotSupportedException();
    }
}
