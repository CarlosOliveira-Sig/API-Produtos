using API.Produtos.DTOs;
using API.Produtos.Models;
using System.Data;

namespace API.Produtos.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IDatabaseService _databaseService;

        public ProdutoService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<IEnumerable<ProdutoListDTO>> GetProdutosAsync()
        {
            var produtos = new List<ProdutoListDTO>();
            
            using var connection = await _databaseService.GetConnectionAsync();
            var query = @"
                SELECT p.id, p.codigo, p.descricao, p.departamento_id, 
                       d.nome as departamento_nome, p.preco, p.status
                FROM produtos p
                INNER JOIN departamentos d ON p.departamento_id = d.id
                ORDER BY p.id";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                produtos.Add(new ProdutoListDTO
                {
                    Id = reader.GetInt32("id"),
                    Codigo = reader.GetString("codigo"),
                    Descricao = reader.GetString("descricao"),
                    DepartamentoId = reader.GetInt32("departamento_id"),
                    DepartamentoNome = reader.GetString("departamento_nome"),
                    Preco = reader.GetDecimal("preco"),
                    Status = reader.GetBoolean("status")
                });
            }
            
            return produtos;
        }

        public async Task<ProdutoDTO?> GetProdutoByIdAsync(int id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = @"
                SELECT p.id, p.codigo, p.descricao, p.departamento_id, 
                       d.nome as departamento_nome, p.preco, p.status
                FROM produtos p
                INNER JOIN departamentos d ON p.departamento_id = d.id
                WHERE p.id = @id";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ProdutoDTO
                {
                    Id = reader.GetInt32("id"),
                    Codigo = reader.GetString("codigo"),
                    Descricao = reader.GetString("descricao"),
                    DepartamentoId = reader.GetInt32("departamento_id"),
                    DepartamentoNome = reader.GetString("departamento_nome"),
                    Preco = reader.GetDecimal("preco"),
                    Status = reader.GetBoolean("status")
                };
            }
            
            return null;
        }

        public async Task<ProdutoDTO> CreateProdutoAsync(ProdutoInputDTO produtoInput)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = @"
                INSERT INTO produtos (codigo, descricao, departamento_id, preco, status)
                VALUES (@codigo, @descricao, @departamento_id, @preco, @status)
                RETURNING id";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@codigo", produtoInput.Codigo);
            command.Parameters.AddWithValue("@descricao", produtoInput.Descricao);
            command.Parameters.AddWithValue("@departamento_id", produtoInput.DepartamentoId);
            command.Parameters.AddWithValue("@preco", produtoInput.Preco);
            command.Parameters.AddWithValue("@status", produtoInput.Status);
            
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            
            // Buscar o nome do departamento para retornar no DTO
            var departamentoQuery = "SELECT nome FROM departamentos WHERE id = @departamento_id";
            using var deptCommand = new Npgsql.NpgsqlCommand(departamentoQuery, connection);
            deptCommand.Parameters.AddWithValue("@departamento_id", produtoInput.DepartamentoId);
            
            var departamentoNome = await deptCommand.ExecuteScalarAsync() as string ?? "N/A";
            
            return new ProdutoDTO
            {
                Id = id,
                Codigo = produtoInput.Codigo,
                Descricao = produtoInput.Descricao,
                DepartamentoId = produtoInput.DepartamentoId,
                DepartamentoNome = departamentoNome,
                Preco = produtoInput.Preco,
                Status = produtoInput.Status
            };
        }

        public async Task<ProdutoDTO?> UpdateProdutoAsync(int id, ProdutoInputDTO produtoInput)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            
            // Verificar se o produto existe
            var checkQuery = "SELECT COUNT(*) FROM produtos WHERE id = @id";
            using (var checkCommand = new Npgsql.NpgsqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@id", id);
                var exists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;
                if (!exists) return null;
            }
            
            var updateQuery = @"
                UPDATE produtos 
                SET codigo = @codigo, descricao = @descricao, 
                    departamento_id = @departamento_id, preco = @preco, status = @status
                WHERE id = @id";
            
            using var command = new Npgsql.NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@codigo", produtoInput.Codigo);
            command.Parameters.AddWithValue("@descricao", produtoInput.Descricao);
            command.Parameters.AddWithValue("@departamento_id", produtoInput.DepartamentoId);
            command.Parameters.AddWithValue("@preco", produtoInput.Preco);
            command.Parameters.AddWithValue("@status", produtoInput.Status);
            
            await command.ExecuteNonQueryAsync();
            
            // Buscar o nome do departamento para retornar no DTO
            var departamentoQuery = "SELECT nome FROM departamentos WHERE id = @departamento_id";
            using var deptCommand = new Npgsql.NpgsqlCommand(departamentoQuery, connection);
            deptCommand.Parameters.AddWithValue("@departamento_id", produtoInput.DepartamentoId);
            
            var departamentoNome = await deptCommand.ExecuteScalarAsync() as string ?? "N/A";
            
            return new ProdutoDTO
            {
                Id = id,
                Codigo = produtoInput.Codigo,
                Descricao = produtoInput.Descricao,
                DepartamentoId = produtoInput.DepartamentoId,
                DepartamentoNome = departamentoNome,
                Preco = produtoInput.Preco,
                Status = produtoInput.Status
            };
        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = "DELETE FROM produtos WHERE id = @id";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
