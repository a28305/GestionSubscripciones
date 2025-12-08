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
    public class FacturaRepository : IFacturaRepository
    {
        private readonly string _connectionString;

        public FacturaRepository(IConfiguration configuration)
        {
             // Usar la cadena de conexión definida en appsettings/docker-compose
             _connectionString = configuration.GetConnectionString("GestionServiceDB") ?? 
                                 throw new ArgumentNullException(nameof(configuration), "La cadena de conexión 'GestionServiceDB' no se encontró.");
        }

        // --- Método Auxiliar para mapear un SqlDataReader a un objeto Factura ---
        private Factura MapFacturaFromReader(SqlDataReader reader)
        {
            return new Factura
            {
                ID_Factura = reader.GetInt32(reader.GetOrdinal("ID_Factura")),
                MontoTotal = reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                FechaEmision = reader.GetDateTime(reader.GetOrdinal("FechaEmision")),
                EsPagoTardio = reader.GetBoolean(reader.GetOrdinal("EsPagoTardio")),
                DetallesConcepto = reader.GetString(reader.GetOrdinal("DetallesConcepto")),
                DescuentoAplicado = reader.GetInt32(reader.GetOrdinal("DescuentoAplicado")),
                UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")), 
                PlanId = reader.GetInt32(reader.GetOrdinal("PlanId"))
            };
        }

        // 1. GET ALL ASYNC (Listar todas las Facturas)
        public async Task<List<Factura>> GetAllAsync()
        {
            var facturas = new List<Factura>();
            string query = "SELECT * FROM Factura"; 

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            facturas.Add(MapFacturaFromReader(reader));
                        }
                    }
                }
            }
            return facturas;
        }

        // 2. GET BY ID ASYNC (Consultar Factura por ID)
        public async Task<Factura?> GetByIdAsync(int id)
        {
            Factura? factura = null;
            string query = "SELECT * FROM Factura WHERE ID_Factura = @Id"; 

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
                            factura = MapFacturaFromReader(reader);
                        }
                    }
                }
            }
            return factura;
        }

        // 3. GET BY USUARIO ID ASYNC (Recurso Asociado - Requisito AA1)
        public async Task<List<Factura>> GetByUsuarioIdAsync(int usuarioId)
        {
            var facturas = new List<Factura>();
            // 💡 Nota: Asumo que la tabla Factura tiene una columna UsuarioId.
            string query = "SELECT * FROM Factura WHERE UsuarioId = @UsuarioId ORDER BY FechaEmision DESC"; 

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
                            facturas.Add(MapFacturaFromReader(reader));
                        }
                    }
                }
            }
            return facturas;
        }

        // 4. ADD ASYNC (Crear Factura)
        public async Task AddAsync(Factura factura)
        {
            string query = @"
                INSERT INTO Factura (MontoTotal, Estado, FechaEmision, EsPagoTardio, DetallesConcepto, DescuentoAplicado, UsuarioId, PlanId) 
                VALUES (@MontoTotal, @Estado, @FechaEmision, @EsPagoTardio, @DetallesConcepto, @DescuentoAplicado, @UsuarioId, @PlanId);
                SELECT CAST(SCOPE_IDENTITY() as int);";
                    
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MontoTotal", factura.MontoTotal);
                    command.Parameters.AddWithValue("@Estado", factura.Estado);
                    command.Parameters.AddWithValue("@FechaEmision", factura.FechaEmision);
                    command.Parameters.AddWithValue("@EsPagoTardio", factura.EsPagoTardio);
                    command.Parameters.AddWithValue("@DetallesConcepto", factura.DetallesConcepto);
                    command.Parameters.AddWithValue("@DescuentoAplicado", factura.DescuentoAplicado);
                    command.Parameters.AddWithValue("@UsuarioId", factura.UsuarioId);
                    command.Parameters.AddWithValue("@PlanId", factura.PlanId);

                    // Recuperar el ID generado por la base de datos
                    factura.ID_Factura = (int)await command.ExecuteScalarAsync();
                }
            }
        }

        // 5. UPDATE ASYNC (Actualizar Factura)
        public async Task UpdateAsync(Factura factura) 
        {
            string query = @"
                UPDATE Factura 
                SET MontoTotal = @MontoTotal, 
                    Estado = @Estado, 
                    FechaEmision = @FechaEmision,
                    EsPagoTardio = @EsPagoTardio,
                    DetallesConcepto = @DetallesConcepto,
                    DescuentoAplicado = @DescuentoAplicado,
                    UsuarioId = @UsuarioId,
                    PlanId = @PlanId
                WHERE ID_Factura = @Id";
                    
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", factura.ID_Factura);
                    command.Parameters.AddWithValue("@MontoTotal", factura.MontoTotal);
                    command.Parameters.AddWithValue("@Estado", factura.Estado);
                    command.Parameters.AddWithValue("@FechaEmision", factura.FechaEmision);
                    command.Parameters.AddWithValue("@EsPagoTardio", factura.EsPagoTardio);
                    command.Parameters.AddWithValue("@DetallesConcepto", factura.DetallesConcepto);
                    command.Parameters.AddWithValue("@DescuentoAplicado", factura.DescuentoAplicado);
                    command.Parameters.AddWithValue("@UsuarioId", factura.UsuarioId);
                    command.Parameters.AddWithValue("@PlanId", factura.PlanId);

                    int affectedRows = await command.ExecuteNonQueryAsync();
                    if (affectedRows == 0)
                    {
                        throw new KeyNotFoundException($"Factura con ID {factura.ID_Factura} no encontrada.");
                    }
                }
            }
        }

        // 6. DELETE ASYNC (Eliminar Factura)
        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM Factura WHERE ID_Factura = @Id";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    if (affectedRows == 0)
                    {
                        throw new KeyNotFoundException($"Factura con ID {id} no encontrada.");
                    }
                }
            }
        }
    }
}