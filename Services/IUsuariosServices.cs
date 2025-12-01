using GESTIONSUBSCRIPCIONES.models;

namespace GESTIONSUBSCRIPCIONES.Services
{
    public interface IUsuarioservices
    {
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();

    }
}
