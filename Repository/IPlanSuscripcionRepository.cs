using GESTIONSUBSCRIPCIONES.models;
namespace GESTIONSUBSCRIPCIONES.Repositories
{
    public interface IPlanSuscripcionRepository
    {
        Task<List<PlanSuscripcion>> GetAllAsync();
        Task<PlanSuscripcion?> GetByIdAsync(int id);
        Task AddAsync(PlanSuscripcion plan);
        Task UpdateAsync(PlanSuscripcion plan);
        Task DeleteAsync(int id);
         Task InicializarDatosAsync();

        
    }
}