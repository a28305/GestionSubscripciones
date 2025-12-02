namespace GESTIONSUBSCRIPCIONES.models
{
    public class Usuario
    {
       
        public int ID_Usuario { get; set; } // int (Clave Primaria)
        public string Email { get; set; } 
        public string NombrePerfil { get; set; } 
        public int DispositivosActivos { get; set; } 
        public decimal MontoPagadoAcumulado { get; set; } 
        public bool PremiumActivo { get; set; } 
        public DateTime FechaRegistro { get; set; } 

        // 💡 Propiedad para la Clave Foránea (FK) del PlanSuscripcion
        public int PlanActualId { get; set; }

        // Relación Muchos a Uno: Un Usuario tiene un PlanSuscripcion
        // 💡 Esta propiedad se llena con el Plan, el ID se usa en el Repositorio.
        public PlanSuscripcion PlanActual { get; set; } = null!;
    }
}