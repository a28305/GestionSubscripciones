using System.ComponentModel.DataAnnotations;
using System;

namespace GESTIONSUBSCRIPCIONES.DTOs
{
    public class PlanSuscripcionDTO
    {
        // ID_Plan: Incluido para GET y PUT. Se ignora en POST (creación).
        public int ID_Plan { get; set; } 

        // --- Propiedades con VALIDACIÓN (usadas en POST/PUT) ---
        
        [Required(ErrorMessage = "El nombre del plan es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El precio mensual es obligatorio.")]
        // Rango: Cumple con el requisito de validación de entrada.
        [Range(0.01, 1000.00, ErrorMessage = "El precio debe estar entre 0.01 y 1000.")]
        public decimal PrecioMensual { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Se deben permitir entre 1 y 10 dispositivos.")]
        public int MaxDispositivos { get; set; }

        [Required(ErrorMessage = "La calidad de streaming es obligatoria.")]
        public string CalidadMaxStreaming { get; set; }

        public bool PermiteDescarga { get; set; } = false;
        
        // --- Propiedad de Salida (usada para GET) ---
        // El Controller/Service lo llena para la respuesta.
        public DateTime FechaUltimaRevision { get; set; } 
    }
}