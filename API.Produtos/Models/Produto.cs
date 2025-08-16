using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Produtos.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Descricao { get; set; } = string.Empty;
        
        [Required]
        public int DepartamentoId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal Preco { get; set; }
        
        public bool Status { get; set; } = true; // true = Ativo, false = Inativo
        
        // Navigation Property
        [ForeignKey("DepartamentoId")]
        public virtual Departamento? Departamento { get; set; }
    }
}
