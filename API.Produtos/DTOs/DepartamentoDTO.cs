namespace API.Produtos.DTOs
{
    // DTO para entrada (POST/PUT)
    public class DepartamentoInputDTO
    {
        public string Nome { get; set; } = string.Empty;
    }
    
    // DTO para saída (GET)
    public class DepartamentoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
