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
    [Route("api/[controller]")] // Ruta base: /api/Usuario
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioservices _userService;

        public UsuarioController(IUsuarioservices userService)
        {
            _userService = userService;
        }

        // --- Método Auxiliar de Mapeo (Modelo a DTO único) ---
        private UsuarioDTO MapToDto(Usuario u)
        {
             // Mapeo del Plan asociado usando el DTO de Plan único
             PlanSuscripcionDTO planDto = null;
             if (u.PlanActual != null)
             {
                 planDto = new PlanSuscripcionDTO 
                 {
                     ID_Plan = u.PlanActual.ID_Plan,
                     Nombre = u.PlanActual.Nombre,
                     PrecioMensual = u.PlanActual.PrecioMensual,
                     MaxDispositivos = u.PlanActual.MaxDispositivos,
                     CalidadMaxStreaming = u.PlanActual.CalidadMaxStreaming,
                     PermiteDescarga = u.PlanActual.PermiteDescarga,
                     FechaUltimaRevision = u.PlanActual.FechaUltimaRevision
                 };
             }

             return new UsuarioDTO 
             {
                ID_Usuario = u.ID_Usuario,
                Email = u.Email,
                NombrePerfil = u.NombrePerfil,
                DispositivosActivos = u.DispositivosActivos,
                MontoPagadoAcumulado = u.MontoPagadoAcumulado,
                PremiumActivo = u.PremiumActivo,
                FechaRegistro = u.FechaRegistro,
                PlanActualId = u.PlanActual?.ID_Plan ?? 0, 
                PlanActual = planDto 
             };
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UsuarioDTO>))] 
        public async Task<IActionResult> GetAllUsuarios()
        {
            var usuarios = await _userService.GetAllAsync();
            var dtos = usuarios.Select(MapToDto).ToList();
            return Ok(dtos);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UsuarioDTO))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            try
            {
                var usuario = await _userService.GetByIdAsync(id);
                
                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado."); 
                }
                
                return Ok(MapToDto(usuario));
            }
            catch (ArgumentException ex)
            {
                // Captura IDs inválidos (ej: id <= 0) del servicio
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(UsuarioDTO))] 
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Mapeo DTO a Modelo
            var usuario = new Usuario
            {
                Email = dto.Email,
                NombrePerfil = dto.NombrePerfil,
                DispositivosActivos = dto.DispositivosActivos,
                MontoPagadoAcumulado = 0.00M, 
                PremiumActivo = dto.PremiumActivo,
                FechaRegistro = DateTime.Now,
                PlanActual = new PlanSuscripcion { ID_Plan = dto.PlanActualId }
            };

            try
            {
                await _userService.AddAsync(usuario);
                return CreatedAtAction(nameof(GetUsuarioById), new { id = usuario.ID_Usuario }, MapToDto(usuario));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is KeyNotFoundException) 
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioDTO dto)
        {
            if (id <= 0 || !ModelState.IsValid) return BadRequest(ModelState);
            
            // Mapeo DTO a Modelo con el ID
            var usuarioToUpdate = new Usuario
            {
                ID_Usuario = id,
                Email = dto.Email,
                NombrePerfil = dto.NombrePerfil,
                DispositivosActivos = dto.DispositivosActivos,
                MontoPagadoAcumulado = dto.MontoPagadoAcumulado,
                PremiumActivo = dto.PremiumActivo,
                // Si la fecha no se envió en el DTO, la obtenemos del registro existente.
                FechaRegistro = dto.FechaRegistro != default ? dto.FechaRegistro : (await _userService.GetByIdAsync(id))?.FechaRegistro ?? DateTime.Now,
                PlanActual = new PlanSuscripcion { ID_Plan = dto.PlanActualId }
            };

            try
            {
                await _userService.UpdateAsync(usuarioToUpdate);
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
        
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
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