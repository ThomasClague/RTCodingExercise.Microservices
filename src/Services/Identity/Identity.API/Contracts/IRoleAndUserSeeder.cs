namespace Identity.API.Contracts
{
    public interface IRoleAndUserSeeder
    {
        Task SeedRolesAndUsersAsync();
    }
}
