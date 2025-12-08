// models/Factura.cs
using System;
using System.ComponentModel.DataAnnotations; // Usaremos Data Annotations en el DTO

namespace GESTIONSUBSCRIPCIONES.models
{
    public class Factura
    {
        public int ID_Factura { get; set; } // int
        public decimal MontoTotal { get; set; } // decimal
        public string Estado { get; set; } // string (e.g., "Pagada", "Pendiente")
        public DateTime FechaEmision { get; set; } // DateTime
        public bool EsPagoTardio { get; set; } // boolean
        public string DetallesConcepto { get; set; } // string
        public int DescuentoAplicado { get; set; } // int (6 atributos)

        // Relaciones (Claves Foráneas)
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!; // Relación M a 1 con Usuario
        
        public int PlanId { get; set; }
        public PlanSuscripcion Plan { get; set; } = null!; // Relación M a 1 con Plan
    }
}