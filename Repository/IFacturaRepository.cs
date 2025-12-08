using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTIONSUBSCRIPCIONES.Repositories
{
    public interface IFacturaRepository
    {
        // CRUD Básico
        Task<List<Factura>> GetAllAsync();
        Task<Factura?> GetByIdAsync(int id);
        Task AddAsync(Factura factura);
        Task UpdateAsync(Factura factura);
        Task DeleteAsync(int id);
        
        // Requisito: Filtrado de recursos asociados a un usuario
        Task<List<Factura>> GetByUsuarioIdAsync(int usuarioId);
    }
}