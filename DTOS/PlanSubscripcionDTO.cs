namespace GESTIONSUBSCRIPCIONES.DTOs
{
    public class PlanSuscripcionResponseDTO
    {
        public int ID_Plan { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioMensual { get; set; }
        public int MaxDispositivos { get; set; }
        public string CalidadMaxStreaming { get; set; }
        public bool PermiteDescarga { get; set; }
        public DateTime FechaUltimaRevision { get; set; }
    }
}