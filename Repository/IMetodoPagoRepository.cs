using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTIONSUBSCRIPCIONES.Repositories
{
    public interface IMetodoPagoRepository
    {
        Task<List<MetodoPago>> GetAllAsync();
        Task<MetodoPago?> GetByIdAsync(int id);
        Task AddAsync(MetodoPago metodoPago);
        Task UpdateAsync(MetodoPago metodoPago);
        Task DeleteAsync(int id);
        
        // Operación para cumplir con el requisito de "Recursos asociados a un usuario"
        Task<List<MetodoPago>> GetByUsuarioIdAsync(int usuarioId);
    }
}