using Microsoft.Data.SqlClient;
using GESTIONSUBSCRIPCIONES.models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GESTIONSUBSCRIPCIONES.Repositories
{
    public class PlanSuscripcionRepository : IPlanSuscripcionRepository
    {
        private readonly string _connectionString;

        public PlanSuscripcionRepository(IConfiguration configuration)
        {
             // Asegúrate de que este nombre de conexión exista en tu appsettings.json
             _connectionString = configuration.GetConnectionString("GestionServiceDB") ?? 
                          throw new ArgumentNullException(nameof(configuration), "La cadena de conexión 'GestionServiceDB' no se encontró.");
                          
        }

        // --- Método Auxiliar para Mapear los datos de la base de datos a la clase PlanSuscripcion ---
        private PlanSuscripcion MapPlanFromReader(SqlDataReader reader)
        {
            // Asumo que las columnas en la tabla DB están en este orden:
            // ID_Plan (0), Nombre (1), PrecioMensual (2), MaxDispositivos (3), 
            // CalidadMaxStreaming (4), PermiteDescarga (5), FechaUltimaRevision (6)
            return new PlanSuscripcion
            {
                ID_Plan = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                // Usamos GetDecimal y luego Convert.ToDecimal si es necesario, dependiendo del tipo de DB
                PrecioMensual = reader.GetDecimal(2), 
                MaxDispositivos = reader.GetInt32(3),
                CalidadMaxStreaming = reader.GetString(4),
                PermiteDescarga = reader.GetBoolean(5),
                FechaUltimaRevision = reader.GetDateTime(6),
                
                // NOTA: No cargamos la lista de UsuariosAsociados aquí. 
                // Esto se hace típicamente en el UsuarioRepository (carga perezosa/lazy loading) 
                // o mediante un endpoint dedicado para evitar consultas excesivas.
                UsuariosAsociados = new List<Usuario>() 
            };
        }


        // --- GET ALL ASYNC (Listar) ---
        public async Task<List<PlanSuscripcion>> GetAllAsync()
        {
            var planes = new List<PlanSuscripcion>();
            string query = "SELECT ID_Plan, Nombre, PrecioMensual, MaxDispositivos, CalidadMaxStreaming, PermiteDescarga, FechaUltimaRevision FROM PlanSuscripcion"; 

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            planes.Add(MapPlanFromReader(reader));
                        }
                    }
                }
            }
            return planes;
        }

        // --- GET BY ID ASYNC (Consultar) ---
        public async Task<PlanSuscripcion?> GetByIdAsync(int id)
        {
            PlanSuscripcion? plan = null;
            string query = "SELECT ID_Plan, Nombre, PrecioMensual, MaxDispositivos, CalidadMaxStreaming, PermiteDescarga, FechaUltimaRevision FROM PlanSuscripcion WHERE ID_Plan = @Id"; 

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            plan = MapPlanFromReader(reader);
                        }
                    }
                }
            }
            return plan;
        }

        // --- ADD ASYNC (Crear) ---
        public async Task AddAsync(PlanSuscripcion plan) 
        {
            string query = @"
                INSERT INTO PlanSuscripcion (Nombre, PrecioMensual, MaxDispositivos, CalidadMaxStreaming, PermiteDescarga, FechaUltimaRevision) 
                VALUES (@Nombre, @PrecioMensual, @MaxDispositivos, @CalidadMaxStreaming, @PermiteDescarga, @FechaUltimaRevision)";
                    
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", plan.Nombre);
                    command.Parameters.AddWithValue("@PrecioMensual", plan.PrecioMensual);
                    command.Parameters.AddWithValue("@MaxDispositivos", plan.MaxDispositivos);
                    command.Parameters.AddWithValue("@CalidadMaxStreaming", plan.CalidadMaxStreaming);
                    command.Parameters.AddWithValue("@PermiteDescarga", plan.PermiteDescarga);
                    command.Parameters.AddWithValue("@FechaUltimaRevision", plan.FechaUltimaRevision);
                    
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        
        // --- UPDATE ASYNC (Actualizar) ---
        public async Task UpdateAsync(PlanSuscripcion plan) 
        {
            string query = @"
                UPDATE PlanSuscripcion 
                SET Nombre = @Nombre, 
                    PrecioMensual = @PrecioMensual, 
                    MaxDispositivos = @MaxDispositivos,
                    CalidadMaxStreaming = @CalidadMaxStreaming,
                    PermiteDescarga = @PermiteDescarga,
                    FechaUltimaRevision = @FechaUltimaRevision
                WHERE ID_Plan = @Id";
                    
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", plan.ID_Plan);
                    command.Parameters.AddWithValue("@Nombre", plan.Nombre);
                    command.Parameters.AddWithValue("@PrecioMensual", plan.PrecioMensual);
                    command.Parameters.AddWithValue("@MaxDispositivos", plan.MaxDispositivos);
                    command.Parameters.AddWithValue("@CalidadMaxStreaming", plan.CalidadMaxStreaming);
                    command.Parameters.AddWithValue("@PermiteDescarga", plan.PermiteDescarga);
                    command.Parameters.AddWithValue("@FechaUltimaRevision", plan.FechaUltimaRevision); 

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // --- DELETE ASYNC (Eliminar) ---
        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM PlanSuscripcion WHERE ID_Plan = @Id";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        
        // --- INICIALIZAR DATOS ASYNC ---
        public async Task InicializarDatosAsync() 
        {
            // Lógica para crear un plan de prueba si no existe
            var planBasico = new PlanSuscripcion
            {
                Nombre = "Plan Básico",
                PrecioMensual = 7.99M,
                MaxDispositivos = 1,
                CalidadMaxStreaming = "HD",
                PermiteDescarga = false,
                FechaUltimaRevision = DateTime.Now
            };

            // Para evitar duplicados, puedes verificar si ya existe un plan con ese nombre antes de agregarlo.
            // Para simplificar, asumiremos que se puede agregar si el contador es 0.
            var existing = await GetAllAsync();
            if (existing.Count == 0)
            {
                await AddAsync(planBasico);
            }
        }
    }
}