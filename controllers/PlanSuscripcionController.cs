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
    [Route("api/[controller]")] // Ruta base: /api/PlanSuscripcion
    public class PlanSuscripcionController : ControllerBase
    {
        private readonly IPlanSuscripcionService _planService;

        public PlanSuscripcionController(IPlanSuscripcionService planService)
        {
            _planService = planService;
        }
        [HttpGet]
        // Se usa el DTO único para la respuesta
        [ProducesResponseType(200, Type = typeof(IEnumerable<PlanSuscripcionDTO>))] 
        public async Task<IActionResult> GetAllPlans()
        {
            var planes = await _planService.GetAllAsync();
            
            // Mapeo (manual) de Modelo a DTO único
            var dtos = planes.Select(p => new PlanSuscripcionDTO // <-- USAMOS DTO ÚNICO
            {
                ID_Plan = p.ID_Plan,
                Nombre = p.Nombre,
                PrecioMensual = p.PrecioMensual,
                MaxDispositivos = p.MaxDispositivos,
                CalidadMaxStreaming = p.CalidadMaxStreaming,
                PermiteDescarga = p.PermiteDescarga,
                FechaUltimaRevision = p.FechaUltimaRevision
            }).ToList();

            return Ok(dtos);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PlanSuscripcionDTO))] // <-- USAMOS DTO ÚNICO
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _planService.GetByIdAsync(id);
            
            if (plan == null) return NotFound($"Plan con ID {id} no encontrado."); 
            
            // Mapeo (manual) de Modelo a DTO único
            var dto = new PlanSuscripcionDTO // <-- USAMOS DTO ÚNICO
            {
                ID_Plan = plan.ID_Plan,
                Nombre = plan.Nombre,
                PrecioMensual = plan.PrecioMensual,
                MaxDispositivos = plan.MaxDispositivos,
                CalidadMaxStreaming = plan.CalidadMaxStreaming,
                PermiteDescarga = plan.PermiteDescarga,
                FechaUltimaRevision = plan.FechaUltimaRevision
            };
            
            return Ok(dto);
        }
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PlanSuscripcionDTO))] // <-- USAMOS DTO ÚNICO
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePlan([FromBody] PlanSuscripcionDTO dto) // <-- USAMOS DTO ÚNICO
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Mapeo (manual) de DTO a Modelo
            var plan = new PlanSuscripcion
            {
                // NO mapeamos ID_Plan aquí, la DB lo genera.
                Nombre = dto.Nombre,
                PrecioMensual = dto.PrecioMensual,
                MaxDispositivos = dto.MaxDispositivos,
                CalidadMaxStreaming = dto.CalidadMaxStreaming,
                PermiteDescarga = dto.PermiteDescarga,
                FechaUltimaRevision = DateTime.Now 
            };

            try
            {
                await _planService.AddAsync(plan);
                
                // Crea el DTO de respuesta para devolver el objeto creado (incluyendo el ID generado)
                var responseDto = new PlanSuscripcionDTO // <-- USAMOS DTO ÚNICO
                {
                    ID_Plan = plan.ID_Plan, 
                    Nombre = plan.Nombre,
                    PrecioMensual = plan.PrecioMensual,
                    MaxDispositivos = plan.MaxDispositivos,
                    CalidadMaxStreaming = plan.CalidadMaxStreaming,
                    PermiteDescarga = plan.PermiteDescarga,
                    FechaUltimaRevision = plan.FechaUltimaRevision
                };
                
                return CreatedAtAction(nameof(GetPlanById), new { id = plan.ID_Plan }, responseDto);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(204)] 
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] PlanSuscripcionDTO dto) // <-- USAMOS DTO ÚNICO
        {
            if (id <= 0 || !ModelState.IsValid) return BadRequest(ModelState);
            
            var planToUpdate = new PlanSuscripcion
            {
                ID_Plan = id, // ¡Es fundamental pasar el ID al modelo!
                Nombre = dto.Nombre,
                PrecioMensual = dto.PrecioMensual,
                MaxDispositivos = dto.MaxDispositivos,
                CalidadMaxStreaming = dto.CalidadMaxStreaming,
                PermiteDescarga = dto.PermiteDescarga,
                FechaUltimaRevision = DateTime.Now // Actualizamos la fecha
            };

            try
            {
                await _planService.UpdateAsync(planToUpdate);
                return NoContent(); 
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
        
        [HttpDelete("{id}")]
        // ... (DELETE no cambia)
        public async Task<IActionResult> DeletePlan(int id)
        {
            try
            {
                await _planService.DeleteAsync(id);
                return NoContent(); 
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
    }
}