using GESTIONSUBSCRIPCIONES.models;
using GESTIONSUBSCRIPCIONES.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq; // Necesario para algunas operaciones de filtrado

namespace GESTIONSUBSCRIPCIONES.Repository
{
    // NOTA: Usamos el namespace 'GESTIONSUBSCRIPCIONES.Repository' para mantener la coherencia con tu UsuarioReposirtory.cs
    public class MetodoPagoRepository : IMetodoPagoRepository
    {
        private readonly string _connectionString;

        public MetodoPagoRepository(IConfiguration configuration)
        {
             _connectionString = configuration.GetConnectionString("GestionServiceDB") ?? 
                                 throw new ArgumentNullException(nameof(configuration), "La cadena de conexión 'GestionServiceDB' no se encontró.");
        }

        public Task<List<MetodoPago>> GetAllAsync()
        {
            // 💡 TAREA: Implementación de ADO.NET para SELECT * FROM MetodoPago
            var metodos = new List<MetodoPago>(); 
            return Task.FromResult(metodos); 
        }

        public Task<MetodoPago?> GetByIdAsync(int id)
        {
            // 💡 TAREA: Implementación de ADO.NET para SELECT * WHERE ID_Metodo = @Id
            return Task.FromResult<MetodoPago?>(null); 
        }

        public Task AddAsync(MetodoPago metodoPago)
        {
            // 💡 TAREA: Implementación de ADO.NET para INSERT INTO MetodoPago
            // Asegúrate de que, al crear, devuelves el ID generado a metodoPago.ID_Metodo
            return Task.CompletedTask;
        }

        public Task UpdateAsync(MetodoPago metodoPago)
        {
            // 💡 TAREA: Implementación de ADO.NET para UPDATE MetodoPago WHERE ID_Metodo = @Id
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            // 💡 TAREA: Implementación de ADO.NET para DELETE FROM MetodoPago WHERE ID_Metodo = @Id
            return Task.CompletedTask;
        }
        
        // Implementación del requisito de Recursos asociados (filtrado por FK)
        public Task<List<MetodoPago>> GetByUsuarioIdAsync(int usuarioId)
        {
            // 💡 TAREA: Implementación de ADO.NET para SELECT * FROM MetodoPago WHERE UsuarioId = @UsuarioId
            var metodos = new List<MetodoPago>();
            return Task.FromResult(metodos);
        }
    }
}