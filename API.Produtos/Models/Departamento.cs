using System.ComponentModel.DataAnnotations;

namespace API.Produtos.Models
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        // Navigation Property
        public virtual ICollection<Produto>? Produtos { get; set; }
    }
}
