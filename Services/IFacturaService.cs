using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTIONSUBSCRIPCIONES.Services
{
    public interface IFacturaService
    {
        // CRUD Básico
        Task<List<Factura>> GetAllAsync();
        Task<Factura?> GetByIdAsync(int id);
        Task AddAsync(Factura factura);
        Task UpdateAsync(Factura factura);
        Task DeleteAsync(int id);
        
        // Requisito: Recursos asociados a un usuario concreto
        Task<List<Factura>> GetFacturasByUsuario(int usuarioId);
    }
}