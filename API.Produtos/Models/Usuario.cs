using System.ComponentModel.DataAnnotations;

namespace API.Produtos.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Senha { get; set; } = string.Empty;
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;
    }
}
