// models/Notificacion.cs
using System;

namespace GESTIONSUBSCRIPCIONES.models
{
    public class Notificacion
    {
        public int ID_Notificacion { get; set; } // int
        public string TipoAlerta { get; set; } // string (e.g., "Cobro", "Advertencia")
        public string Mensaje { get; set; } // string
        public bool Leida { get; set; } = false; // boolean
        public DateTime FechaEnvio { get; set; } // DateTime
        public int Prioridad { get; set; } // int (1=Baja, 5=Alta)
        public decimal TiempoRetencionHoras { get; set; } // decimal (6 atributos)

        // Relación (Clave Foránea)
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!; // Relación M a 1 con Usuario
    }
}