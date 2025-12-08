using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTIONSUBSCRIPCIONES.Services
{
    public interface IMetodoPagoService
    {
        Task<List<MetodoPago>> GetAllAsync();
        Task<MetodoPago?> GetByIdAsync(int id);
        Task AddAsync(MetodoPago metodoPago);
        Task UpdateAsync(MetodoPago metodoPago);
        Task DeleteAsync(int id);
        
        // Operación del servicio para el recurso asociado
        Task<List<MetodoPago>> GetMetodosByUsuario(int usuarioId);
    }
}