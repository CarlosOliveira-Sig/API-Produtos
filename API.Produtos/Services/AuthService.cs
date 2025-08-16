using API.Produtos.DTOs;
using API.Produtos.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Produtos.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        public AuthService(IDatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = "SELECT id, nome, email, senha FROM usuarios WHERE email = @email AND ativo = true";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@email", loginDTO.Email);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var usuario = new Usuario
                {
                    Id = reader.GetInt32("id"),
                    Nome = reader.GetString("nome"),
                    Email = reader.GetString("email"),
                    Senha = reader.GetString("senha")
                };

                if (ValidatePassword(loginDTO.Senha, usuario.Senha))
                {
                    var token = GenerateJwtToken(usuario);
                    return new AuthResponseDTO
                    {
                        Token = token,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        ExpiraEm = DateTime.UtcNow.AddHours(24)
                    };
                }
            }
            
            return null;
        }

        public async Task<AuthResponseDTO?> RegistroAsync(RegistroDTO registroDTO)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            
            // Verificar se email já existe
            var checkQuery = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
            using (var checkCommand = new Npgsql.NpgsqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@email", registroDTO.Email);
                var exists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;
                if (exists) return null;
            }

            // Inserir novo usuário
            var insertQuery = @"
                INSERT INTO usuarios (nome, email, senha, data_criacao, ativo)
                VALUES (@nome, @email, @senha, @data_criacao, @ativo)
                RETURNING id";
            
            var hashSenha = HashPassword(registroDTO.Senha);
            using var command = new Npgsql.NpgsqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@nome", registroDTO.Nome);
            command.Parameters.AddWithValue("@email", registroDTO.Email);
            command.Parameters.AddWithValue("@senha", hashSenha);
            command.Parameters.AddWithValue("@data_criacao", DateTime.UtcNow);
            command.Parameters.AddWithValue("@ativo", true);
            
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            
            var usuario = new Usuario
            {
                Id = id,
                Nome = registroDTO.Nome,
                Email = registroDTO.Email
            };

            var token = GenerateJwtToken(usuario);
            return new AuthResponseDTO
            {
                Token = token,
                Nome = usuario.Nome,
                Email = usuario.Email,
                ExpiraEm = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<UsuarioDTO?> GetUsuarioAsync(int id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            var query = "SELECT id, nome, email, data_criacao, ativo FROM usuarios WHERE id = @id AND ativo = true";
            
            using var command = new Npgsql.NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UsuarioDTO
                {
                    Id = reader.GetInt32("id"),
                    Nome = reader.GetString("nome"),
                    Email = reader.GetString("email"),
                    DataCriacao = reader.GetDateTime("data_criacao"),
                    Ativo = reader.GetBoolean("ativo")
                };
            }
            
            return null;
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "SuaChaveSecretaSuperSeguraParaJWT2024";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "API-Produtos";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "Angular-Client";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("userId", usuario.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidatePassword(string senha, string hashSenha)
        {
            var hashInput = HashPassword(senha);
            return hashInput == hashSenha;
        }

        public string HashPassword(string senha)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
