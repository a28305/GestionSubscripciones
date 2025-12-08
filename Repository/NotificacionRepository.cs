using GESTIONSUBSCRIPCIONES.models;
using GESTIONSUBSCRIPCIONES.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient; // Necesario para ADO.NET
using System.Linq; 

namespace GESTIONSUBSCRIPCIONES.Repository
{
    // Nota: El namespace sigue la convención de tu UsuarioRepository.cs (singular)
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly string _connectionString;

        public NotificacionRepository(IConfiguration configuration)
        {
             // Usar la cadena de conexión definida en appsettings/docker-compose
             _connectionString = configuration.GetConnectionString("GestionServiceDB") ?? 
                                 throw new ArgumentNullException(nameof(configuration), "La cadena de conexión 'GestionServiceDB' no se encontró.");
        }

        // --- Método Auxiliar para mapear un SqlDataReader a un objeto Notificacion ---
        private Notificacion MapNotificacionFromReader(SqlDataReader reader)
        {
            return new Notificacion
            {
                ID_Notificacion = reader.GetInt32(reader.GetOrdinal("ID_Notificacion")),
                TipoAlerta = reader.GetString(reader.GetOrdinal("TipoAlerta")),
                Mensaje = reader.GetString(reader.GetOrdinal("Mensaje")),
                Leida = reader.GetBoolean(reader.GetOrdinal("Leida")),
                FechaEnvio = reader.GetDateTime(reader.GetOrdinal("FechaEnvio")),
                Prioridad = reader.GetInt32(reader.GetOrdinal("Prioridad")),
                TiempoRetencionHoras = reader.GetDecimal(reader.GetOrdinal("TiempoRetencionHoras")),
                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId"))
            };
        }

        // 1. GET ALL ASYNC (Listar todas las Notificaciones)
        public async Task<List<Notificacion>> GetAllAsync()
        {
            var notificaciones = new List<Notificacion>();
            string query = "SELECT * FROM Notificacion"; 

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            notificaciones.Add(MapNotificacionFromReader(reader));
                        }
                    }
                }
            }
            return notificaciones;
        }

        // 2. GET BY ID ASYNC (Consultar Notificacion por ID)
        public async Task<Notificacion?> GetByIdAsync(int id)
        {
            Notificacion? notificacion = null;
            string query = "SELECT * FROM Notificacion WHERE ID_Notificacion = @Id"; 

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            notificacion = MapNotificacionFromReader(reader);
                        }
                    }
                }
            }
            return notificacion;
        }

        // 3. GET BY USUARIO ID ASYNC (Recurso Asociado - Requisito AA1)
        public async Task<List<Notificacion>> GetByUsuarioIdAsync(int usuarioId)
        {
            var notificaciones = new List<Notificacion>();
            // 💡 Nota: Asumo que la tabla Notificacion tiene una columna UsuarioId.
            string query = "SELECT * FROM Notificacion WHERE UsuarioId = @UsuarioId ORDER BY FechaEnvio DESC"; 

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            notificaciones.Add(MapNotificacionFromReader(reader));
                        }
                    }
                }
            }
            return notificaciones;
        }

        // 4. ADD ASYNC (Crear Notificacion)
        public async Task AddAsync(Notificacion notificacion)
        {
            string query = @"
                INSERT INTO Notificacion (TipoAlerta, Mensaje, Leida, FechaEnvio, Prioridad, TiempoRetencionHoras, UsuarioId) 
                VALUES (@TipoAlerta, @Mensaje, @Leida, @FechaEnvio, @Prioridad, @TiempoRetencionHoras, @UsuarioId);
                SELECT CAST(SCOPE_IDENTITY() as int);";
                    
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TipoAlerta", notificacion.TipoAlerta);
                    command.Parameters.AddWithValue("@Mensaje", notificacion.Mensaje);
                    command.Parameters.AddWithValue("@Leida", notificacion.Leida);
                    command.Parameters.AddWithValue("@FechaEnvio", notificacion.FechaEnvio);
                    command.Parameters.AddWithValue("@Prioridad", notificacion.Prioridad);
                    command.Parameters.AddWithValue("@TiempoRetencionHoras", notificacion.TiempoRetencionHoras);
                    command.Parameters.AddWithValue("@UsuarioId", notificacion.UsuarioId);

                    // Recuperar el ID generado por la base de datos
                    notificacion.ID_Notificacion = (int)await command.ExecuteScalarAsync();
                }
            }
        }

        // 5. UPDATE ASYNC (Actualizar Notificacion)
        public async Task UpdateAsync(Notificacion notificacion) 
        {
            string query = @"
                UPDATE Notificacion 
                SET TipoAlerta = @TipoAlerta, 
                    Mensaje = @Mensaje, 
                    Leida = @Leida,
                    FechaEnvio = @FechaEnvio,
                    Prioridad = @Prioridad,
                    TiempoRetencionHoras = @TiempoRetencionHoras,
                    UsuarioId = @UsuarioId
                WHERE ID_Notificacion = @Id";
                    
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", notificacion.ID_Notificacion);
                    command.Parameters.AddWithValue("@TipoAlerta", notificacion.TipoAlerta);
                    command.Parameters.AddWithValue("@Mensaje", notificacion.Mensaje);
                    command.Parameters.AddWithValue("@Leida", notificacion.Leida);
                    command.Parameters.AddWithValue("@FechaEnvio", notificacion.FechaEnvio);
                    command.Parameters.AddWithValue("@Prioridad", notificacion.Prioridad);
                    command.Parameters.AddWithValue("@TiempoRetencionHoras", notificacion.TiempoRetencionHoras);
                    command.Parameters.AddWithValue("@UsuarioId", notificacion.UsuarioId);

                    int affectedRows = await command.ExecuteNonQueryAsync();
                    if (affectedRows == 0)
                    {
                        throw new KeyNotFoundException($"Notificación con ID {notificacion.ID_Notificacion} no encontrada.");
                    }
                }
            }
        }

        // 6. DELETE ASYNC (Eliminar Notificacion)
        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM Notificacion WHERE ID_Notificacion = @Id";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    if (affectedRows == 0)
                    {
                        throw new KeyNotFoundException($"Notificación con ID {id} no encontrada.");
                    }
                }
            }
        }
    }
}