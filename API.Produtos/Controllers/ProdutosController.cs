using Microsoft.AspNetCore.Mvc;
using API.Produtos.DTOs;
using API.Produtos.Services;

namespace API.Produtos.Controllers
{
    /// <summary>
    /// Controlador para gerenciamento de produtos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly ILogger<ProdutosController> _logger;

        public ProdutosController(IProdutoService produtoService, ILogger<ProdutosController> logger)
        {
            _produtoService = produtoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os produtos cadastrados no sistema
        /// </summary>
        /// <returns>Lista de produtos com informações básicas</returns>
        /// <response code="200">Lista de produtos retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProdutoListDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProdutoListDTO>>> GetProdutos()
        {
            try
            {
                var produtos = await _produtoService.GetProdutosAsync();
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar produtos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Busca um produto específico pelo seu ID
        /// </summary>
        /// <param name="id">ID único do produto</param>
        /// <returns>Dados completos do produto</returns>
        /// <response code="200">Produto encontrado e retornado com sucesso</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProdutoDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProdutoDTO>> GetProduto(int id)
        {
            try
            {
                var produto = await _produtoService.GetProdutoByIdAsync(id);
                if (produto == null)
                {
                    return NotFound($"Produto com ID {id} não encontrado");
                }
                return Ok(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Cria um novo produto no sistema
        /// </summary>
        /// <param name="produtoInput">Dados do produto a ser criado</param>
        /// <returns>Produto criado com ID gerado</returns>
        /// <response code="201">Produto criado com sucesso</response>
        /// <response code="400">Dados inválidos ou código já existente</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProdutoDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProdutoDTO>> CreateProduto([FromBody] ProdutoInputDTO produtoInput)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var produto = await _produtoService.CreateProdutoAsync(produtoInput);
                return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualiza um produto existente pelo seu ID
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado</param>
        /// <param name="produtoInput">Novos dados do produto</param>
        /// <returns>Produto atualizado</returns>
        /// <response code="200">Produto atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProdutoDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProdutoDTO>> UpdateProduto(int id, [FromBody] ProdutoInputDTO produtoInput)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var produto = await _produtoService.UpdateProdutoAsync(id, produtoInput);
                if (produto == null)
                {
                    return NotFound($"Produto com ID {id} não encontrado");
                }

                return Ok(produto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Exclui um produto do sistema pelo seu ID
        /// </summary>
        /// <param name="id">ID do produto a ser excluído</param>
        /// <returns>Sem conteúdo em caso de sucesso</returns>
        /// <response code="204">Produto excluído com sucesso</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteProduto(int id)
        {
            try
            {
                var deleted = await _produtoService.DeleteProdutoAsync(id);
                if (!deleted)
                {
                    return NotFound($"Produto com ID {id} não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir produto com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
