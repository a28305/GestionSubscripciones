namespace GESTIONSUBSCRIPCIONES.models
{
    public class PlanSuscripcion
    {
        
        public int ID_Plan { get; set; } // int (Clave Primaria)
        public string Nombre { get; set; } 
        public decimal PrecioMensual { get; set; } 
        public int MaxDispositivos { get; set; } 
        public string CalidadMaxStreaming { get; set; } 
        public bool PermiteDescarga { get; set; } 
        public DateTime FechaUltimaRevision { get; set; } // DateTime 

        // Relación Uno a Muchos: Un Plan puede tener muchos Usuarios
        public List<Usuario> UsuariosAsociados { get; set; }
    
    }
}