using GESTIONSUBSCRIPCIONES.Repositories;
using GESTIONSUBSCRIPCIONES.models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GESTIONSUBSCRIPCIONES.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPlanSuscripcionRepository _planRepository;

        // Inyección de Repositorios para orquestación
        public FacturaService(IFacturaRepository facturaRepository, 
                              IUsuarioRepository usuarioRepository, 
                              IPlanSuscripcionRepository planRepository)
        {
            _facturaRepository = facturaRepository;
            _usuarioRepository = usuarioRepository;
            _planRepository = planRepository;
        }

        public async Task<List<Factura>> GetAllAsync() => await _facturaRepository.GetAllAsync();

        public async Task<Factura?> GetByIdAsync(int id) => await _facturaRepository.GetByIdAsync(id);

        // Implementación del requisito de recursos asociados
        public async Task<List<Factura>> GetFacturasByUsuario(int usuarioId)
        {
             if (usuarioId <= 0)
                throw new ArgumentException("ID de usuario no válido.");
            
            // Lógica de negocio: Confirmar que el usuario existe antes de buscar sus facturas
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                 throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado.");
                 
            return await _facturaRepository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task AddAsync(Factura factura)
        {
            // Validación de Negocio: Asegurar que el Usuario y Plan existen
            if (await _usuarioRepository.GetByIdAsync(factura.UsuarioId) == null)
                throw new KeyNotFoundException($"Usuario con ID {factura.UsuarioId} no existe.");

            if (await _planRepository.GetByIdAsync(factura.PlanId) == null)
                throw new KeyNotFoundException($"Plan con ID {factura.PlanId} no existe.");

            // Más validaciones de negocio (ej: no emitir factura con MontoTotal cero)
            if (factura.MontoTotal <= 0)
                 throw new ArgumentException("El monto total debe ser positivo.");
                 
            await _facturaRepository.AddAsync(factura);
        }
        
        // Implementaciones de UpdateAsync y DeleteAsync omitidas por brevedad, 
        // pero deben seguir el mismo patrón de validación.
        public Task UpdateAsync(Factura factura) {
            // ... Implementación con validaciones de existencia (KeyNotFoundException)
            return Task.CompletedTask; 
        }

        public Task DeleteAsync(int id) {
             // ... Implementación con validaciones de existencia y argumento
             return Task.CompletedTask; 
        }
    }
}