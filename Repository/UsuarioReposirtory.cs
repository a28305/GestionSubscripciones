using Microsoft.Data.SqlClient;
using GESTIONSUBSCRIPCIONES.models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq; // Necesario para LINQ (First, etc.)
using GESTIONSUBSCRIPCIONES.Repositories; // Asegúrate de usar el namespace correcto

namespace GESTIONSUBSCRIPCIONES.Repository
{
    // El nombre del archivo es UsuarioReposirtory.cs, pero la clase es UsuarioRepository
    public class UsuarioRepository : IUsuarioRepository 
    {
        private readonly string _connectionString;
        
        // Asumo que el PlanSuscripcionRepository será necesario para buscar el plan,
        // pero por simplicidad de la consulta SQL (JOIN), lo haremos en una sola.
        // Si no existe IPlanSuscripcionRepository, se puede inyectar IConfiguration
        // directamente y hacer la consulta simple. Vamos a simplificar.

        public UsuarioRepository(IConfiguration configuration)
        {
             // 💡 Usamos la clave correcta del appsettings.json
             _connectionString = configuration.GetConnectionString("GestionServiceDB") ?? 
                                 throw new ArgumentNullException(nameof(configuration), "La cadena de conexión 'GestionServiceDB' no se encontró.");
        }
        
        // --- GET ALL ASYNC (Listar) ---
        public async Task<List<Usuario>> GetAllAsync()
        {
            var usuarios = new List<Usuario>();

            // 💡 Consulta SQL que une Usuario y PlanSuscripcion
            // Asumo que tu tabla de PlanSuscripcion tiene una columna PlanActualId (int)
            // que es una FK a la tabla PlanSuscripcion.
            string query = @"
                SELECT 
                    u.ID_Usuario, u.Email, u.NombrePerfil, u.DispositivosActivos, 
                    u.MontoPagadoAcumulado, u.PremiumActivo, u.FechaRegistro, 
                    p.ID_Plan, p.Nombre as PlanNombre, p.PrecioMensual, 
                    p.MaxDispositivos, p.CalidadMaxStreaming, p.PermiteDescarga, 
                    p.FechaUltimaRevision
                FROM 
                    Usuario u
                JOIN 
                    PlanSuscripcion p ON u.PlanActualId = p.ID_Plan; -- 💡 ASUMO QUE EXISTE PlanActualId EN LA TABLA Usuario
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var plan = new PlanSuscripcion
                            {
                                ID_Plan = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                PrecioMensual = reader.GetDecimal(9),
                                MaxDispositivos = reader.GetInt32(10),
                                CalidadMaxStreaming = reader.GetString(11),
                                PermiteDescarga = reader.GetBoolean(12),
                                FechaUltimaRevision = reader.GetDateTime(13)
                            };

                            var usuario = new Usuario
                            {
                                ID_Usuario = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                NombrePerfil = reader.GetString(2),
                                DispositivosActivos = reader.GetInt32(3),
                                MontoPagadoAcumulado = reader.GetDecimal(4),
                                PremiumActivo = reader.GetBoolean(5),
                                FechaRegistro = reader.GetDateTime(6),
                                PlanActual = plan // Asigna el objeto PlanSuscripcion
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            }
            return usuarios;
        }

        // --- GET BY ID ASYNC (Obtener por ID) ---
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            // Implementación similar a GetAllAsync, pero con un WHERE ID_Usuario = @id
            // (Esta es una implementación que deberías revisar y completar en tu proyecto)
            return await GetAllAsync().ContinueWith(t => t.Result.FirstOrDefault(u => u.ID_Usuario == id)); 
        }

        // --- ADD ASYNC (Añadir) ---
        public async Task AddAsync(Usuario usuario)
        {
            // 💡 IMPORTANTE: Debes asegurarte de que PlanActualId existe en tu modelo Usuario y base de datos
            string query = @"
                INSERT INTO Usuario (Email, NombrePerfil, DispositivosActivos, MontoPagadoAcumulado, PremiumActivo, FechaRegistro, PlanActualId)
                VALUES (@Email, @NombrePerfil, @DispositivosActivos, @MontoPagadoAcumulado, @PremiumActivo, @FechaRegistro, @PlanActualId);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@NombrePerfil", usuario.NombrePerfil);
                    command.Parameters.AddWithValue("@DispositivosActivos", usuario.DispositivosActivos);
                    command.Parameters.AddWithValue("@MontoPagadoAcumulado", usuario.MontoPagadoAcumulado);
                    command.Parameters.AddWithValue("@PremiumActivo", usuario.PremiumActivo);
                    command.Parameters.AddWithValue("@FechaRegistro", usuario.FechaRegistro);
                    // 💡 Asumo que el ID del Plan es lo que se guarda en la tabla Usuario
                    command.Parameters.AddWithValue("@PlanActualId", usuario.PlanActual.ID_Plan); 

                    usuario.ID_Usuario = (int)await command.ExecuteScalarAsync();
                }
            }
        }

        // --- UPDATE ASYNC (Actualizar) ---
        public async Task UpdateAsync(Usuario usuario)
        {
            string query = @"
                UPDATE Usuario SET 
                    Email = @Email, 
                    NombrePerfil = @NombrePerfil, 
                    DispositivosActivos = @DispositivosActivos, 
                    MontoPagadoAcumulado = @MontoPagadoAcumulado, 
                    PremiumActivo = @PremiumActivo, 
                    FechaRegistro = @FechaRegistro, 
                    PlanActualId = @PlanActualId
                WHERE ID_Usuario = @ID_Usuario;
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Usuario", usuario.ID_Usuario);
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@NombrePerfil", usuario.NombrePerfil);
                    command.Parameters.AddWithValue("@DispositivosActivos", usuario.DispositivosActivos);
                    command.Parameters.AddWithValue("@MontoPagadoAcumulado", usuario.MontoPagadoAcumulado);
                    command.Parameters.AddWithValue("@PremiumActivo", usuario.PremiumActivo);
                    command.Parameters.AddWithValue("@FechaRegistro", usuario.FechaRegistro);
                    command.Parameters.AddWithValue("@PlanActualId", usuario.PlanActual.ID_Plan); 

                    int affectedRows = await command.ExecuteNonQueryAsync();
                    if (affectedRows == 0)
                    {
                        throw new KeyNotFoundException($"Usuario con ID {usuario.ID_Usuario} no encontrado.");
                    }
                }
            }
        }

        // --- DELETE ASYNC (Eliminar) ---
        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM Usuario WHERE ID_Usuario = @ID_Usuario";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID_Usuario", id);
                    int affectedRows = await command.ExecuteNonQueryAsync();
                     if (affectedRows == 0)
                    {
                        throw new KeyNotFoundException($"Usuario con ID {id} no encontrado.");
                    }
                }
            }
        }
        
        // --- INICIALIZAR DATOS ASYNC ---
        // Elimino la inicialización anterior que usaba otras entidades.
        public Task InicializarDatosAsync() {
            // Este método debería estar en PlanSuscripcionRepository primero si quieres 
            // asegurar que los planes base existen antes de que el usuario haga referencias a ellos.
            // Lo dejo vacío por ahora.
            return Task.CompletedTask;
        }
    }
}