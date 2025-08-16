using Microsoft.AspNetCore.Mvc;
using API.Produtos.DTOs;
using API.Produtos.Services;

namespace API.Produtos.Controllers
{
    /// <summary>
    /// Controlador para autenticação e gerenciamento de usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza o login do usuário no sistema
        /// </summary>
        /// <param name="loginDTO">Dados de login (email e senha)</param>
        /// <returns>Token JWT e dados do usuário autenticado</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Dados de login inválidos</response>
        /// <response code="401">Email ou senha incorretos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var authResponse = await _authService.LoginAsync(loginDTO);
                if (authResponse == null)
                {
                    return Unauthorized("Email ou senha inválidos");
                }

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="registroDTO">Dados do usuário a ser registrado</param>
        /// <returns>Token JWT e dados do usuário registrado</returns>
        /// <response code="201">Usuário registrado com sucesso</response>
        /// <response code="400">Dados inválidos ou email já cadastrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("registro")]
        [ProducesResponseType(typeof(AuthResponseDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AuthResponseDTO>> Registro([FromBody] RegistroDTO registroDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var authResponse = await _authService.RegistroAsync(registroDTO);
                if (authResponse == null)
                {
                    return BadRequest("Email já cadastrado");
                }

                return CreatedAtAction(nameof(Login), authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém os dados de um usuário específico pelo seu ID
        /// </summary>
        /// <param name="id">ID único do usuário</param>
        /// <returns>Dados do usuário solicitado</returns>
        /// <response code="200">Usuário encontrado e retornado com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("usuario/{id}")]
        [ProducesResponseType(typeof(UsuarioDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _authService.GetUsuarioAsync(id);
                if (usuario == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário com ID {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
