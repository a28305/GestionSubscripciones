using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTIONSUBSCRIPCIONES.Repositories
{
    public interface INotificacionRepository
    {
        Task<List<Notificacion>> GetAllAsync();
        Task<Notificacion?> GetByIdAsync(int id);
        Task AddAsync(Notificacion notificacion);
        Task UpdateAsync(Notificacion notificacion);
        Task DeleteAsync(int id);
        
        // Operación para cumplir con el requisito de "Recursos asociados a un usuario"
        Task<List<Notificacion>> GetByUsuarioIdAsync(int usuarioId);
    }
}