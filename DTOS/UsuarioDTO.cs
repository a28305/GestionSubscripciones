using System.ComponentModel.DataAnnotations;
using System;

namespace GESTIONSUBSCRIPCIONES.DTOs
{
    public class PlanSuscripcionCreateDTO
    {
        // El nombre es obligatorio
        [Required(ErrorMessage = "El nombre del plan es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; }

        // El precio debe ser un rango válido
        [Required(ErrorMessage = "El precio mensual es obligatorio.")]
        [Range(0.01, 1000.00, ErrorMessage = "El precio debe estar entre 0.01 y 1000.")]
        public decimal PrecioMensual { get; set; }

        // Mínimo 1 dispositivo
        [Required]
        [Range(1, 10, ErrorMessage = "Se deben permitir entre 1 y 10 dispositivos.")]
        public int MaxDispositivos { get; set; }

        [Required]
        public string CalidadMaxStreaming { get; set; }

        public bool PermiteDescarga { get; set; } = false;
        
        // No incluyes FechaUltimaRevision, el servicio puede establecerla a DateTime.Now.
    }
}