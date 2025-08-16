namespace API.Produtos.DTOs
{
    // DTO para entrada (POST/PUT)
    public class ProdutoInputDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int DepartamentoId { get; set; }
        public decimal Preco { get; set; }
        public bool Status { get; set; } = true;
    }
    
    // DTO para saída (GET)
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int DepartamentoId { get; set; }
        public decimal Preco { get; set; }
        public bool Status { get; set; }
        public string? DepartamentoNome { get; set; }
    }
    
    // DTO para listagem com informações do departamento
    public class ProdutoListDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int DepartamentoId { get; set; }
        public string DepartamentoNome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public bool Status { get; set; }
    }
}
