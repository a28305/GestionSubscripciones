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
    [Route("api/[controller]")] // Ruta base: /api/Factura
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        // --- Método Auxiliar de Mapeo (Modelo a DTO único) ---
        private FacturaDTO MapToDto(Factura f)
        {
             return new FacturaDTO 
             {
                ID_Factura = f.ID_Factura,
                MontoTotal = f.MontoTotal,
                Estado = f.Estado,
                FechaEmision = f.FechaEmision,
                EsPagoTardio = f.EsPagoTardio,
                DetallesConcepto = f.DetallesConcepto,
                DescuentoAplicado = f.DescuentoAplicado,
                UsuarioId = f.UsuarioId, 
                PlanId = f.PlanId
             };
        }

        // --- 1. GET ALL (Recurso general) ---
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FacturaDTO>))] 
        public async Task<IActionResult> GetAllFacturas()
        {
            var facturas = await _facturaService.GetAllAsync();
            var dtos = facturas.Select(MapToDto).ToList();
            return Ok(dtos);
        }

        // --- 2. GET BY ID (Recurso general) ---
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(FacturaDTO))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFacturaById(int id)
        {
            var factura = await _facturaService.GetByIdAsync(id);
            
            if (factura == null)
            {
                return NotFound($"Factura con ID {id} no encontrada."); 
            }
            return Ok(MapToDto(factura));
        }

        // --- 3. GET By User ID (Recurso asociado a un usuario, Requisito AA1) ---
        [HttpGet("Usuario/{usuarioId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FacturaDTO>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetFacturasByUsuario(int usuarioId)
        {
            try
            {
                var facturas = await _facturaService.GetFacturasByUsuario(usuarioId);
                var dtos = facturas.Select(MapToDto).ToList();
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
        [ProducesResponseType(201, Type = typeof(FacturaDTO))] 
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFactura([FromBody] FacturaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var factura = new Factura
            {
                MontoTotal = dto.MontoTotal,
                Estado = dto.Estado,
                FechaEmision = DateTime.Now,
                EsPagoTardio = dto.EsPagoTardio,
                DetallesConcepto = dto.DetallesConcepto,
                DescuentoAplicado = dto.DescuentoAplicado,
                UsuarioId = dto.UsuarioId, 
                PlanId = dto.PlanId
            };

            try
            {
                await _facturaService.AddAsync(factura);
                return CreatedAtAction(nameof(GetFacturaById), new { id = factura.ID_Factura }, MapToDto(factura));
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
        public async Task<IActionResult> UpdateFactura(int id, [FromBody] FacturaDTO dto)
        {
            if (id <= 0 || !ModelState.IsValid) return BadRequest(ModelState);
            
            var facturaToUpdate = new Factura
            {
                ID_Factura = id,
                MontoTotal = dto.MontoTotal,
                Estado = dto.Estado,
                FechaEmision = dto.FechaEmision, // Se mantiene la fecha del DTO
                EsPagoTardio = dto.EsPagoTardio,
                DetallesConcepto = dto.DetallesConcepto,
                DescuentoAplicado = dto.DescuentoAplicado,
                UsuarioId = dto.UsuarioId, 
                PlanId = dto.PlanId
            };

            try
            {
                await _facturaService.UpdateAsync(facturaToUpdate);
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
        public async Task<IActionResult> DeleteFactura(int id)
        {
            try
            {
                await _facturaService.DeleteAsync(id);
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