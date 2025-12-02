using GESTIONSUBSCRIPCIONES.Repositories;
using GESTIONSUBSCRIPCIONES.models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Mail; // Para validación de email

namespace GESTIONSUBSCRIPCIONES.Services
{
    // NOTA: El nombre de tu clase en el archivo subido es 'UsuarioService' y la interfaz 'IUsuarioservices'
    public class UsuarioService : IUsuarioservices
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPlanSuscripcionRepository _planRepository; // Inyección crucial para la lógica de negocio

        // El servicio inyecta los repositorios que necesita para operar.
        public UsuarioService(IUsuarioRepository usuarioRepository, IPlanSuscripcionRepository planRepository)
        {
            _usuarioRepository = usuarioRepository;
            _planRepository = planRepository;
        }

        // --- GET ALL ASYNC (Listar) ---
        public async Task<List<Usuario>> GetAllAsync()
        {
            // Lógica de negocio sencilla: simplemente devolver todos.
            return await _usuarioRepository.GetAllAsync();
        }

        // --- GET BY ID ASYNC (Consultar) ---
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            // Validación de entrada
            if (id <= 0)
                throw new ArgumentException("El ID de Usuario debe ser un número positivo.");

            return await _usuarioRepository.GetByIdAsync(id);
        }

        // --- ADD ASYNC (Crear) ---
        public async Task AddAsync(Usuario usuario)
        {
            // 1. VALIDACIÓN DE ENTRADA (Datos del usuario)
            if (string.IsNullOrWhiteSpace(usuario.NombrePerfil))
                throw new ArgumentException("El nombre del perfil no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new ArgumentException("El email no puede estar vacío.");
            
            if (usuario.DispositivosActivos < 0)
                throw new ArgumentException("El número de dispositivos activos no puede ser negativo.");
            
            // 2. VALIDACIÓN DE NEGOCIO (Reglas de la aplicación)

            // Validación de Email (opcional, pero buena práctica)
            try {
                var addr = new MailAddress(usuario.Email);
            } catch {
                throw new ArgumentException("El formato del email no es válido.");
            }

            // Orquestación entre Repositorios: Asegurar que el plan existe.
            if (usuario.PlanActual != null) 
            {
                var planExiste = await _planRepository.GetByIdAsync(usuario.PlanActual.ID_Plan);
                if (planExiste == null)
                    throw new KeyNotFoundException($"El Plan de Suscripción con ID {usuario.PlanActual.ID_Plan} no existe.");
                
                // Aplicar regla de negocio inicial: Si el usuario se registra con un plan, 
                // sus dispositivos iniciales no pueden superar el máximo del plan.
                if (usuario.DispositivosActivos > planExiste.MaxDispositivos)
                     throw new ArgumentException($"Los dispositivos activos iniciales ({usuario.DispositivosActivos}) superan el máximo del plan ({planExiste.MaxDispositivos}).");
            }
            
            // 3. LLAMADA AL REPOSITORIO para la persistencia
            await _usuarioRepository.AddAsync(usuario);
        }

        // --- UPDATE ASYNC (Actualizar) ---
        public async Task UpdateAsync(Usuario usuario)
        {
            // Validación de ID
            if (usuario.ID_Usuario <= 0)
                throw new ArgumentException("El ID no es válido para la actualización.");

            // Puedes añadir una validación para asegurar que el usuario realmente existe antes de actualizar:
            var existingUser = await _usuarioRepository.GetByIdAsync(usuario.ID_Usuario);
            if (existingUser == null)
                throw new KeyNotFoundException($"No se encontró ningún usuario con ID {usuario.ID_Usuario}.");
            
            // VALIDACIÓN DE NEGOCIO (Ejemplo de una regla de negocio dinámica):
            // Si el usuario tiene un Plan actual, comprueba que los dispositivos no superen el límite.
            if (usuario.PlanActual != null) 
            {
                var planDetails = await _planRepository.GetByIdAsync(usuario.PlanActual.ID_Plan);
                if (planDetails == null)
                    throw new KeyNotFoundException($"El nuevo Plan de Suscripción con ID {usuario.PlanActual.ID_Plan} no existe.");

                if (usuario.DispositivosActivos > planDetails.MaxDispositivos)
                     throw new ArgumentException($"No se puede actualizar. Los dispositivos activos ({usuario.DispositivosActivos}) superan el máximo permitido por el plan ({planDetails.MaxDispositivos}).");
            }
            
            // 3. LLAMADA AL REPOSITORIO para la persistencia
            await _usuarioRepository.UpdateAsync(usuario);
        }

        // --- DELETE ASYNC (Eliminar) ---
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.");

            // Lógica de negocio: Podrías añadir una regla para impedir eliminar usuarios con MontoPagadoAcumulado alto, por ejemplo.
            
            await _usuarioRepository.DeleteAsync(id);
        }

        // --- INICIALIZAR DATOS ASYNC ---
        public async Task InicializarDatosAsync() {
            await _usuarioRepository.InicializarDatosAsync();
        }
    }
}