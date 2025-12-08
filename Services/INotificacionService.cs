using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTIONSUBSCRIPCIONES.Services
{
    public interface INotificacionService
    {
        Task<List<Notificacion>> GetAllAsync();
        Task<Notificacion?> GetByIdAsync(int id);
        Task AddAsync(Notificacion notificacion);
        Task UpdateAsync(Notificacion notificacion);
        Task DeleteAsync(int id);
        
        // Operación del servicio para el recurso asociado
        Task<List<Notificacion>> GetNotificacionesByUsuario(int usuarioId);
    }
}