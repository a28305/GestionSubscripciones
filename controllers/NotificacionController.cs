using Microsoft.AspNetCore.Mvc;
using GESTIONSUBSCRIPCIONES.Services;
using GESTIONSUBSCRIPCIONES.DTOs;
using GESTIONSUBSCRIPCIONES.models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GESTIONSUBSCRIPCIONES.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] // Ruta base: /api/Notificacion
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        // --- Método Auxiliar de Mapeo (Modelo a DTO) ---
        private NotificacionDTO MapToDto(Notificacion n)
        {
             return new NotificacionDTO 
             {
                ID_Notificacion = n.ID_Notificacion,
                TipoAlerta = n.TipoAlerta,
                Mensaje = n.Mensaje,
                Leida = n.Leida,
                FechaEnvio = n.FechaEnvio,
                Prioridad = n.Prioridad,
                TiempoRetencionHoras = n.TiempoRetencionHoras,
                UsuarioId = n.UsuarioId 
             };
        }

        // --- 1. GET ALL (Recurso general) ---
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NotificacionDTO>))] 
        public async Task<IActionResult> GetAllNotificaciones()
        {
            var notificaciones = await _notificacionService.GetAllAsync();
            var dtos = notificaciones.Select(MapToDto).ToList();
            return Ok(dtos);
        }

        // --- 2. GET BY ID (Recurso general) ---
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(NotificacionDTO))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNotificacionById(int id)
        {
            try
            {
                var notificacion = await _notificacionService.GetByIdAsync(id);
                if (notificacion == null)
                {
                    return NotFound($"Notificación con ID {id} no encontrada."); 
                }
                return Ok(MapToDto(notificacion));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // --- 3. GET By User ID (Recurso asociado a un usuario - Requisito AA1) ---
        [HttpGet("Usuario/{usuarioId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NotificacionDTO>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetNotificacionesByUsuario(int usuarioId)
        {
            try
            {
                // Este método debe validar si el usuario existe (lógica en el Service)
                var notificaciones = await _notificacionService.GetNotificacionesByUsuario(usuarioId);
                var dtos = notificaciones.Select(MapToDto).ToList();
                return Ok(dtos);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // Usuario no encontrado (404)
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // ID de usuario inválido (400)
            }
        }

        // --- 4. POST (Create) ---
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NotificacionDTO))] 
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateNotificacion([FromBody] NotificacionDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var notificacion = new Notificacion
            {
                TipoAlerta = dto.TipoAlerta,
                Mensaje = dto.Mensaje,
                Leida = dto.Leida,
                FechaEnvio = DateTime.Now, // La fecha de envío se establece al crear
                Prioridad = dto.Prioridad,
                TiempoRetencionHoras = dto.TiempoRetencionHoras,
                UsuarioId = dto.UsuarioId 
            };

            try
            {
                await _notificacionService.AddAsync(notificacion);
                return CreatedAtAction(nameof(GetNotificacionById), new { id = notificacion.ID_Notificacion }, MapToDto(notificacion));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException) 
            {
                return BadRequest(ex.Message);
            }
        }
        
        // --- 5. PUT (Update) ---
        [HttpPut("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateNotificacion(int id, [FromBody] NotificacionDTO dto)
        {
            if (id <= 0 || !ModelState.IsValid) return BadRequest(ModelState);
            
            var notificacionToUpdate = new Notificacion
            {
                ID_Notificacion = id,
                TipoAlerta = dto.TipoAlerta,
                Mensaje = dto.Mensaje,
                Leida = dto.Leida,
                FechaEnvio = dto.FechaEnvio, // Se asume que se mantiene la fecha del DTO/registro original
                Prioridad = dto.Prioridad,
                TiempoRetencionHoras = dto.TiempoRetencionHoras,
                UsuarioId = dto.UsuarioId 
            };

            try
            {
                await _notificacionService.UpdateAsync(notificacionToUpdate);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // --- 6. DELETE (Delete) ---
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteNotificacion(int id)
        {
            try
            {
                await _notificacionService.DeleteAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                 return BadRequest(ex.Message);
            }
        }
    }
}