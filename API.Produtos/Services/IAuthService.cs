using API.Produtos.DTOs;
using API.Produtos.Models;

namespace API.Produtos.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDTO);
        Task<AuthResponseDTO?> RegistroAsync(RegistroDTO registroDTO);
        Task<UsuarioDTO?> GetUsuarioAsync(int id);
        string GenerateJwtToken(Usuario usuario);
        bool ValidatePassword(string senha, string hashSenha);
        string HashPassword(string senha);
    }
}
