using GESTIONSUBSCRIPCIONES.Repositories;
using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace GESTIONSUBSCRIPCIONES.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public NotificacionService(INotificacionRepository notificacionRepository, IUsuarioRepository usuarioRepository)
        {
            _notificacionRepository = notificacionRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<Notificacion>> GetAllAsync() => await _notificacionRepository.GetAllAsync();

        public Task<Notificacion?> GetByIdAsync(int id)
        {
             if (id <= 0) throw new ArgumentException("ID no válido.");
             return _notificacionRepository.GetByIdAsync(id);
        }

        // Implementación del requisito de Recursos asociados (validación de existencia de usuario)
        public async Task<List<Notificacion>> GetNotificacionesByUsuario(int usuarioId)
        {
             if (usuarioId <= 0)
                throw new ArgumentException("ID de usuario no válido.");
            
            // Lógica de Negocio: Verificar que el usuario exista
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                 throw new KeyNotFoundException($"Usuario con ID {usuarioId} no encontrado.");
                 
            return await _notificacionRepository.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task AddAsync(Notificacion notificacion)
        {
            // Lógica de Negocio: Verificar que el Usuario asociado exista
            if (await _usuarioRepository.GetByIdAsync(notificacion.UsuarioId) == null)
                throw new KeyNotFoundException($"Usuario con ID {notificacion.UsuarioId} no existe.");

            await _notificacionRepository.AddAsync(notificacion);
        }
        
        // Tarea: Implementar UpdateAsync y DeleteAsync
        public Task UpdateAsync(Notificacion notificacion) { return Task.CompletedTask; }
        public Task DeleteAsync(int id) { return Task.CompletedTask; }
    }
}