using API.Produtos.DTOs;

namespace API.Produtos.Services
{
    /// <summary>
    /// Interface para o serviço de gerenciamento de departamentos
    /// </summary>
    public interface IDepartamentoService
    {
        /// <summary>
        /// Obtém todos os departamentos cadastrados
        /// </summary>
        /// <returns>Lista de departamentos</returns>
        Task<IEnumerable<DepartamentoDTO>> GetDepartamentosAsync();

        /// <summary>
        /// Obtém um departamento específico pelo ID
        /// </summary>
        /// <param name="id">ID do departamento</param>
        /// <returns>Departamento encontrado ou null</returns>
        Task<DepartamentoDTO?> GetDepartamentoByIdAsync(int id);

        /// <summary>
        /// Cria um novo departamento
        /// </summary>
        /// <param name="departamentoInput">Dados do departamento a ser criado</param>
        /// <returns>Departamento criado com ID</returns>
        Task<DepartamentoDTO> CreateDepartamentoAsync(DepartamentoInputDTO departamentoInput);

        /// <summary>
        /// Atualiza um departamento existente
        /// </summary>
        /// <param name="id">ID do departamento a ser atualizado</param>
        /// <param name="departamentoInput">Novos dados do departamento</param>
        /// <returns>Departamento atualizado ou null se não encontrado</returns>
        Task<DepartamentoDTO?> UpdateDepartamentoAsync(int id, DepartamentoInputDTO departamentoInput);

        /// <summary>
        /// Exclui um departamento pelo ID
        /// </summary>
        /// <param name="id">ID do departamento a ser excluído</param>
        /// <returns>True se excluído com sucesso, False caso contrário</returns>
        Task<bool> DeleteDepartamentoAsync(int id);
    }
}
