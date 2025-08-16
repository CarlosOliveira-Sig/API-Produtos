using Microsoft.AspNetCore.Mvc;
using API.Produtos.DTOs;
using API.Produtos.Services;

namespace API.Produtos.Controllers
{
    /// <summary>
    /// Controlador para gerenciamento de departamentos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DepartamentosController : ControllerBase
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly ILogger<DepartamentosController> _logger;

        public DepartamentosController(IDepartamentoService departamentoService, ILogger<DepartamentosController> logger)
        {
            _departamentoService = departamentoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os departamentos disponíveis no sistema
        /// </summary>
        /// <returns>Lista de departamentos com seus dados básicos</returns>
        /// <response code="200">Lista de departamentos retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartamentoDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> GetDepartamentos()
        {
            try
            {
                var departamentos = await _departamentoService.GetDepartamentosAsync();
                return Ok(departamentos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar departamentos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
