// models/MetodoPago.cs
using System;

namespace GESTIONSUBSCRIPCIONES.models
{
    public class MetodoPago
    {
        public int ID_Metodo { get; set; } // int
        public string Tipo { get; set; } // string (e.g., "Tarjeta", "PayPal")
        public string UltimosDigitos { get; set; } // string
        public bool EsPrincipal { get; set; } = false; // boolean
        public DateTime FechaCaducidad { get; set; } // DateTime
        public int CodigoSeguridad { get; set; } // int
        public decimal LimiteTransaccion { get; set; } // decimal (6 atributos)

        // Relación (Clave Foránea)
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!; // Relación 1 a 1 o 1 a M con Usuario
    }
}