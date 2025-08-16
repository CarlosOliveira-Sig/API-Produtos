using API.Produtos.DTOs;
using System.Data;

namespace API.Produtos.Services
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDatabaseService _databaseService;

        public DepartamentoService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<IEnumerable<DepartamentoDTO>> GetDepartamentosAsync()
        {
            var departamentos = new List<DepartamentoDTO>();
            
            using var connection = await _databaseService.GetConnectionAsync();
            var query = "SELECT id, nome FROM departamentos ORDER BY nome";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                departamentos.Add(new DepartamentoDTO
                {
                    Id = reader.GetInt32("id"),
                    Nome = reader.GetString("nome")
                });
            }
            
            return departamentos;
        }

        public async Task<DepartamentoDTO?> GetDepartamentoByIdAsync(int id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = "SELECT id, nome FROM departamentos WHERE id = @id";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DepartamentoDTO
                {
                    Id = reader.GetInt32("id"),
                    Nome = reader.GetString("nome")
                };
            }
            
            return null;
        }

        public async Task<DepartamentoDTO> CreateDepartamentoAsync(DepartamentoInputDTO departamentoInput)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = @"
                INSERT INTO departamentos (nome)
                VALUES (@nome)
                RETURNING id";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@nome", departamentoInput.Nome);
            
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            
            return new DepartamentoDTO
            {
                Id = id,
                Nome = departamentoInput.Nome
            };
        }

        public async Task<DepartamentoDTO?> UpdateDepartamentoAsync(int id, DepartamentoInputDTO departamentoInput)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = @"
                UPDATE departamentos 
                SET nome = @nome
                WHERE id = @id
                RETURNING id";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@nome", departamentoInput.Nome);
            
            var result = await command.ExecuteScalarAsync();
            if (result != null)
            {
                return new DepartamentoDTO
                {
                    Id = id,
                    Nome = departamentoInput.Nome
                };
            }
            
            return null;
        }

        public async Task<bool> DeleteDepartamentoAsync(int id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            
            // Verificar se há produtos associados
            var checkQuery = "SELECT COUNT(*) FROM produtos WHERE departamento_id = @id";
            using var checkCommand = new Npgsql.NpgsqlCommand(checkQuery, connection);
            checkCommand.Parameters.AddWithValue("@id", id);
            
            var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
            if (count > 0)
            {
                return false; // Não pode excluir departamento com produtos
            }
            
            // Excluir departamento
            var deleteQuery = "DELETE FROM departamentos WHERE id = @id";
            using var deleteCommand = new Npgsql.NpgsqlCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@id", id);
            
            var rowsAffected = await deleteCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
