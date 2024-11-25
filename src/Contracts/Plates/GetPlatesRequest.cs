namespace Contracts.Plates
{
    public class GetPlatesRequest
    {
        public PlatesFilter PlatesFilter { get; init; }
        public bool IsAdmin { get; init; }

        public GetPlatesRequest(PlatesFilter platesFilter, bool isAdmin)
        {
            PlatesFilter = platesFilter;
            IsAdmin = isAdmin;
        }
    }
}
