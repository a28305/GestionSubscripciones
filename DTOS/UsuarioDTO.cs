using System.ComponentModel.DataAnnotations;
using System;

namespace GESTIONSUBSCRIPCIONES.DTOs
{
    public class UsuarioDTO
    {
        // ID_Usuario: Incluido para GET y PUT. Se ignora en POST.
        public int ID_Usuario { get; set; }

        // --- Propiedades con VALIDACIÓN (usadas en POST/PUT) ---
        
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } 
        
        [Required(ErrorMessage = "El nombre del perfil es obligatorio.")]
        [MaxLength(50)]
        public string NombrePerfil { get; set; } 
        
        [Range(0, 10, ErrorMessage = "Dispositivos activos debe ser entre 0 y 10.")]
        public int DispositivosActivos { get; set; } 
        
        // Clave Foránea: Es el dato que el cliente envía para asociar el plan.
        [Required(ErrorMessage = "Se requiere el ID del plan de suscripción.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del plan debe ser válido.")]
        public int PlanActualId { get; set; } 
        
        public bool PremiumActivo { get; set; } = false; 
        
        // --- Propiedades de Salida (usadas para GET) ---
        
        public decimal MontoPagadoAcumulado { get; set; } 
        public DateTime FechaRegistro { get; set; } 

        // Relación: Usamos el DTO único del plan para mostrar los detalles del plan en la respuesta.
        public PlanSuscripcionDTO PlanActual { get; set; } 
    }
}