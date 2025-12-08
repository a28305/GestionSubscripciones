using System.ComponentModel.DataAnnotations;
using System;

namespace GESTIONSUBSCRIPCIONES.DTOs
{
    public class FacturaDTO
    {
        // ID_Factura: Usado para GET y PUT. Se ignora en POST.
        public int ID_Factura { get; set; }

        // --- Propiedades con VALIDACIÓN ---
        
        [Required(ErrorMessage = "El monto total es obligatorio.")]
        [Range(0.01, 5000.00, ErrorMessage = "El monto debe estar entre 0.01 y 5000.")]
        public decimal MontoTotal { get; set; } // decimal

        [Required(ErrorMessage = "El estado de la factura es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El estado no puede exceder 30 caracteres.")]
        public string Estado { get; set; } // string 
        
        // El controller o service debe establecer la fecha.
        public DateTime FechaEmision { get; set; } = DateTime.Now; // DateTime

        public bool EsPagoTardio { get; set; } = false; // boolean

        [MaxLength(255, ErrorMessage = "El concepto es demasiado largo.")]
        public string DetallesConcepto { get; set; } // string

        [Range(0, 100, ErrorMessage = "El descuento debe ser entre 0 y 100%.")]
        public int DescuentoAplicado { get; set; } // int
        
        // --- Claves Foráneas (Asociaciones) ---
        
        [Required(ErrorMessage = "Se requiere el ID del usuario asociado.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de Usuario debe ser válido.")]
        public int UsuarioId { get; set; } // Clave Foránea: Usuario

        [Required(ErrorMessage = "Se requiere el ID del plan de suscripción.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de Plan debe ser válido.")]
        public int PlanId { get; set; } // Clave Foránea: PlanSuscripcion
        
        // Propiedades de navegación (Opcional para simplificar el DTO de entrada)
        // Podrías añadir DTOs de Plan y Usuario si quieres que el GET devuelva detalles.
    }
}