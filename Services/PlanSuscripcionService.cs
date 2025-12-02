using GESTIONSUBSCRIPCIONES.Repositories;
using GESTIONSUBSCRIPCIONES.models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GESTIONSUBSCRIPCIONES.Services
{
    // Debes crear la interfaz IPlanSuscripcionService con los mismos métodos
    public class PlanSuscripcionService : IPlanSuscripcionService
    {
        private readonly IPlanSuscripcionRepository _planRepository;

        // Inyección del Repositorio
        public PlanSuscripcionService(IPlanSuscripcionRepository planRepository)
        {
            _planRepository = planRepository;
        }

        // --- LECTURA (Read) ---
        public async Task<List<PlanSuscripcion>> GetAllAsync()
        {
            // Lógica de negocio sencilla: simplemente devolver todos.
            return await _planRepository.GetAllAsync();
        }

        public async Task<PlanSuscripcion?> GetByIdAsync(int id)
        {
            // Validación de entrada simple
            if (id <= 0)
                throw new ArgumentException("El ID del plan debe ser un número positivo.");

            return await _planRepository.GetByIdAsync(id);
        }

        // --- CREACIÓN (Create) ---
        public async Task AddAsync(PlanSuscripcion plan)
        {
            // VALIDACIÓN: Reglas de negocio (Ejemplos)
            if (string.IsNullOrWhiteSpace(plan.Nombre))
                throw new ArgumentException("El nombre del plan no puede estar vacío.");
            
            if (plan.PrecioMensual <= 0)
                throw new ArgumentException("El precio mensual debe ser mayor que cero.");

            if (plan.MaxDispositivos < 1)
                throw new ArgumentException("Un plan debe permitir al menos 1 dispositivo.");

            // Si pasa las validaciones, se llama al Repositorio.
            await _planRepository.AddAsync(plan);
        }

        // --- ACTUALIZACIÓN (Update) ---
        public async Task UpdateAsync(PlanSuscripcion plan)
        {
            // VALIDACIÓN: El ID debe existir y ser válido
            if (plan.ID_Plan <= 0)
                throw new ArgumentException("El ID del plan no es válido para la actualización.");

            // Puedes añadir una validación para asegurar que el plan realmente existe antes de actualizar:
            var existingPlan = await _planRepository.GetByIdAsync(plan.ID_Plan);
            if (existingPlan == null)
                throw new KeyNotFoundException($"No se encontró ningún plan con ID {plan.ID_Plan}.");
            
            // ... (Otras validaciones como en AddAsync)

            await _planRepository.UpdateAsync(plan);
        }

        // --- ELIMINACIÓN (Delete) ---
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.");

            // Validación de negocio: Verificar que el plan no esté asociado a usuarios activos
            // if (await _usuarioRepository.IsPlanUsed(id)) // Requiere inyectar UsuarioRepository
            //     throw new InvalidOperationException("No se puede eliminar un plan con usuarios activos.");

            await _planRepository.DeleteAsync(id);
        }
        
        public async Task InicializarDatosAsync() {
            await _planRepository.InicializarDatosAsync();
        }
    }
}