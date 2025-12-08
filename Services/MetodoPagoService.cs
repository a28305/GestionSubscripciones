using GESTIONSUBSCRIPCIONES.Repositories;
using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GESTIONSUBSCRIPCIONES.Services
{
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly IMetodoPagoRepository _pagoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public MetodoPagoService(IMetodoPagoRepository pagoRepository, IUsuarioRepository usuarioRepository)
        {
            _pagoRepository = pagoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<MetodoPago>> GetAllAsync() => await _pagoRepository.GetAllAsync();

        public async Task<MetodoPago?> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID no válido.");
            return await _pagoRepository.GetByIdAsync(id);
        }

        // Implementación del requisito de Recursos asociados (validación de existencia de usuario)
        public async Task<List<MetodoPago>> GetMetodosByUsuario(int usuarioId)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("ID de usuario no válido.");
            
            // Lógica de Negocio: Verificar que el usuario exista
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                 throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado.");
                 
            return await _pagoRepository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task AddAsync(MetodoPago metodoPago)
        {
            // Lógica de Negocio: Verificar que el Usuario asociado exista
            if (await _usuarioRepository.GetByIdAsync(metodoPago.UsuarioId) == null)
                throw new KeyNotFoundException($"Usuario con ID {metodoPago.UsuarioId} no existe.");

            // Tarea: Añadir validaciones de negocio adicionales aquí (ej: longitud de tarjeta, fecha de caducidad)
            
            await _pagoRepository.AddAsync(metodoPago);
        }

        // Tarea: Implementar UpdateAsync y DeleteAsync con validaciones de existencia (KeyNotFoundException)
        public Task UpdateAsync(MetodoPago metodoPago) { return Task.CompletedTask; }
        public Task DeleteAsync(int id) { return Task.CompletedTask; }
    }
}