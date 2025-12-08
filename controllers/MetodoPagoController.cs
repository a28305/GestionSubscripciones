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
    [Route("api/[controller]")] // Ruta base: /api/MetodoPago
    public class MetodoPagoController : ControllerBase
    {
        private readonly IMetodoPagoService _pagoService;

        public MetodoPagoController(IMetodoPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        // --- Método Auxiliar de Mapeo (Modelo a DTO) ---
        private MetodoPagoDTO MapToDto(MetodoPago m)
        {
             return new MetodoPagoDTO 
             {
                ID_Metodo = m.ID_Metodo,
                Tipo = m.Tipo,
                UltimosDigitos = m.UltimosDigitos,
                EsPrincipal = m.EsPrincipal,
                FechaCaducidad = m.FechaCaducidad,
                CodigoSeguridad = m.CodigoSeguridad,
                LimiteTransaccion = m.LimiteTransaccion,
                UsuarioId = m.UsuarioId 
             };
        }

        // --- 1. GET ALL (Recurso general) ---
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MetodoPagoDTO>))] 
        public async Task<IActionResult> GetAllMetodos()
        {
            var metodos = await _pagoService.GetAllAsync();
            var dtos = metodos.Select(MapToDto).ToList();
            return Ok(dtos);
        }

        // --- 2. GET BY ID (Recurso general) ---
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(MetodoPagoDTO))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMetodoById(int id)
        {
            try
            {
                var metodo = await _pagoService.GetByIdAsync(id);
                if (metodo == null)
                {
                    return NotFound($"Método de pago con ID {id} no encontrado."); 
                }
                return Ok(MapToDto(metodo));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // --- 3. GET By User ID (Recurso asociado a un usuario) ---
        [HttpGet("Usuario/{usuarioId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MetodoPagoDTO>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMetodosByUsuario(int usuarioId)
        {
            try
            {
                var metodos = await _pagoService.GetMetodosByUsuario(usuarioId);
                var dtos = metodos.Select(MapToDto).ToList();
                return Ok(dtos);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        // --- 4. POST (Create) ---
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(MetodoPagoDTO))] 
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateMetodoPago([FromBody] MetodoPagoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var metodoPago = new MetodoPago
            {
                Tipo = dto.Tipo,
                UltimosDigitos = dto.UltimosDigitos,
                EsPrincipal = dto.EsPrincipal,
                FechaCaducidad = dto.FechaCaducidad,
                CodigoSeguridad = dto.CodigoSeguridad,
                LimiteTransaccion = dto.LimiteTransaccion,
                UsuarioId = dto.UsuarioId 
            };

            try
            {
                await _pagoService.AddAsync(metodoPago);
                return CreatedAtAction(nameof(GetMetodoById), new { id = metodoPago.ID_Metodo }, MapToDto(metodoPago));
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
        public async Task<IActionResult> UpdateMetodoPago(int id, [FromBody] MetodoPagoDTO dto)
        {
            if (id <= 0 || !ModelState.IsValid) return BadRequest(ModelState);
            
            var metodoToUpdate = new MetodoPago
            {
                ID_Metodo = id,
                Tipo = dto.Tipo,
                UltimosDigitos = dto.UltimosDigitos,
                EsPrincipal = dto.EsPrincipal,
                FechaCaducidad = dto.FechaCaducidad,
                CodigoSeguridad = dto.CodigoSeguridad,
                LimiteTransaccion = dto.LimiteTransaccion,
                UsuarioId = dto.UsuarioId 
            };

            try
            {
                await _pagoService.UpdateAsync(metodoToUpdate);
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
        public async Task<IActionResult> DeleteMetodoPago(int id)
        {
            try
            {
                await _pagoService.DeleteAsync(id);
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