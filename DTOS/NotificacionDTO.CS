using System.ComponentModel.DataAnnotations;
using System;

namespace GESTIONSUBSCRIPCIONES.DTOs
{
    public class NotificacionDTO
    {
        public int ID_Notificacion { get; set; } 

        [Required]
        public string TipoAlerta { get; set; } // string (e.g., "Cobro", "Advertencia")
        
        [Required]
        [MaxLength(500)]
        public string Mensaje { get; set; } // string
        
        public bool Leida { get; set; } = false; // boolean
        
        public DateTime FechaEnvio { get; set; } // DateTime (Salida)
        
        [Range(1, 5, ErrorMessage = "La prioridad debe ser entre 1 y 5.")]
        public int Prioridad { get; set; } // int (1=Baja, 5=Alta)
        
        [Range(0.0, 720.0, ErrorMessage = "La retención máxima es 720 horas (30 días).")]
        public decimal TiempoRetencionHoras { get; set; } // decimal

        // Clave Foránea
        [Required, Range(1, int.MaxValue)]
        public int UsuarioId { get; set; } 
    }
}