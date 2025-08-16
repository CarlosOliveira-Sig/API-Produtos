using API.Produtos.DTOs;

namespace API.Produtos.Services
{
    /// <summary>
    /// Interface para o serviço de gerenciamento de produtos
    /// </summary>
    public interface IProdutoService
    {
        /// <summary>
        /// Obtém todos os produtos cadastrados
        /// </summary>
        /// <returns>Lista de produtos</returns>
        Task<IEnumerable<ProdutoListDTO>> GetProdutosAsync();

        /// <summary>
        /// Obtém um produto específico pelo ID
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <returns>Produto encontrado ou null</returns>
        Task<ProdutoDTO?> GetProdutoByIdAsync(int id);

        /// <summary>
        /// Cria um novo produto
        /// </summary>
        /// <param name="produtoInput">Dados do produto a ser criado</param>
        /// <returns>Produto criado com ID</returns>
        Task<ProdutoDTO> CreateProdutoAsync(ProdutoInputDTO produtoInput);

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado</param>
        /// <param name="produtoInput">Novos dados do produto</param>
        /// <returns>Produto atualizado ou null se não encontrado</returns>
        Task<ProdutoDTO?> UpdateProdutoAsync(int id, ProdutoInputDTO produtoInput);

        /// <summary>
        /// Exclui um produto pelo ID
        /// </summary>
        /// <param name="id">ID do produto a ser excluído</param>
        /// <returns>True se excluído com sucesso, False caso contrário</returns>
        Task<bool> DeleteProdutoAsync(int id);
    }
}
