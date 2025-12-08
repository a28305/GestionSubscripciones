using System.ComponentModel.DataAnnotations;
using System;

namespace GESTIONSUBSCRIPCIONES.DTOs
{
    public class MetodoPagoDTO
    {
        public int ID_Metodo { get; set; } 

        [Required(ErrorMessage = "El tipo de pago es obligatorio.")]
        [MaxLength(20)]
        public string Tipo { get; set; } // string (e.g., "Tarjeta", "PayPal")
        
        [Required(ErrorMessage = "Los últimos dígitos son obligatorios.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Debe tener 4 dígitos.")]
        public string UltimosDigitos { get; set; } // string
        
        public bool EsPrincipal { get; set; } = false; // boolean
        
        [DataType(DataType.Date), Required]
        public DateTime FechaCaducidad { get; set; } // DateTime
        
        [Range(100, 999, ErrorMessage = "El código de seguridad debe ser de 3 dígitos.")]
        public int CodigoSeguridad { get; set; } // int
        
        [Range(0.01, 10000.00)]
        public decimal LimiteTransaccion { get; set; } // decimal
        
        // Clave Foránea
        [Required, Range(1, int.MaxValue)]
        public int UsuarioId { get; set; } 
    }
}