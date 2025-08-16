namespace API.Produtos.Services
{
    public interface IDatabaseService
    {
        Task InitializeDatabaseAsync();
        Task<Npgsql.NpgsqlConnection> GetConnectionAsync();
    }
}
