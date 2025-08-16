using Microsoft.Extensions.Configuration;

namespace API.Produtos.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<Npgsql.NpgsqlConnection> GetConnectionAsync()
        {
            var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task InitializeDatabaseAsync()
        {
            using var connection = await GetConnectionAsync();
            
            // Criar tabela Departamentos
            var createDepartamentosTable = @"
                CREATE TABLE IF NOT EXISTS departamentos (
                    id SERIAL PRIMARY KEY,
                    nome VARCHAR(100) NOT NULL UNIQUE
                );";
            
            using (var command = new Npgsql.NpgsqlCommand(createDepartamentosTable, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Criar tabela Produtos
            var createProdutosTable = @"
                CREATE TABLE IF NOT EXISTS produtos (
                    id SERIAL PRIMARY KEY,
                    codigo VARCHAR(50) NOT NULL UNIQUE,
                    descricao VARCHAR(200) NOT NULL,
                    departamento_id INTEGER NOT NULL,
                    preco DECIMAL(18,2) NOT NULL CHECK (preco > 0),
                    status BOOLEAN NOT NULL DEFAULT true,
                    FOREIGN KEY (departamento_id) REFERENCES departamentos(id)
                );";
            
            using (var command = new Npgsql.NpgsqlCommand(createProdutosTable, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Criar tabela Usuários
            var createUsuariosTable = @"
                CREATE TABLE IF NOT EXISTS usuarios (
                    id SERIAL PRIMARY KEY,
                    nome VARCHAR(100) NOT NULL,
                    email VARCHAR(100) NOT NULL UNIQUE,
                    senha VARCHAR(255) NOT NULL,
                    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ativo BOOLEAN NOT NULL DEFAULT true
                );";
            
            using (var command = new Npgsql.NpgsqlCommand(createUsuariosTable, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Inserir departamentos iniciais se não existirem
            var checkDepartamentos = "SELECT COUNT(*) FROM departamentos";
            using (var command = new Npgsql.NpgsqlCommand(checkDepartamentos, connection))
            {
                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if (count == 0)
                {
                    var insertDepartamentos = @"
                        INSERT INTO departamentos (nome) VALUES 
                        ('Eletrônicos'),
                        ('Informática'),
                        ('Vestuário'),
                        ('Casa e Jardim'),
                        ('Esportes'),
                        ('Livros'),
                        ('Automotivo'),
                        ('Saúde e Beleza');";
                    
                    using var insertCommand = new Npgsql.NpgsqlCommand(insertDepartamentos, connection);
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }

            // Inserir produtos iniciais se não existirem
            var checkProdutos = "SELECT COUNT(*) FROM produtos";
            using (var command = new Npgsql.NpgsqlCommand(checkProdutos, connection))
            {
                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if (count == 0)
                {
                    var insertProdutos = @"
                        INSERT INTO produtos (codigo, descricao, departamento_id, preco, status) VALUES 
                        ('ELET001', 'Smartphone Samsung Galaxy S23', 1, 2999.99, true),
                        ('INFO001', 'Notebook Dell Inspiron 15', 2, 4599.99, true),
                        ('VEST001', 'Camiseta Básica Algodão', 3, 29.99, true);";
                    
                    using var insertCommand = new Npgsql.NpgsqlCommand(insertProdutos, connection);
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }

            // Inserir usuário padrão se não existir
            var checkUsuarios = "SELECT COUNT(*) FROM usuarios";
            using (var command = new Npgsql.NpgsqlCommand(checkUsuarios, connection))
            {
                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                if (count == 0)
                {
                    // Hash da senha "123456"
                    var hashSenha = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=";
                    
                    var insertUsuario = @"
                        INSERT INTO usuarios (nome, email, senha, data_criacao, ativo) VALUES 
                        ('Administrador', 'admin@produtos.com', @senha, @data_criacao, true);";
                    
                    using var insertCommand = new Npgsql.NpgsqlCommand(insertUsuario, connection);
                    insertCommand.Parameters.AddWithValue("@senha", hashSenha);
                    insertCommand.Parameters.AddWithValue("@data_criacao", DateTime.UtcNow);
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
