using Microsoft.Data.SqlClient;
using GESTIONSUBSCRIPCIONES.models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GESTIONSUBSCRIPCIONES.Repository
{
    public class UsuarioRepository : IUsuarioRepository 
    {
        private readonly string _connectionString;

        // Inyecciones de repositorios de ítems
        private readonly IPlatoPrincipalRepository _platoprincipalrepository;
        private readonly IBebidaRepository _bebidarepository;
        private readonly IPostreRepository _postrerepository;

        public MenuRepository(IConfiguration configuration, IPlatoPrincipalRepository platoprincipalrepository, IBebidaRepository bebidarepository, IPostreRepository postrerepository)
        {
             _connectionString = configuration.GetConnectionString("RestauranteDB") ?? "Not found";
             _platoprincipalrepository = platoprincipalrepository;
             _bebidarepository = bebidarepository;
             _postrerepository = postrerepository;
        }
        
        // --- GET ALL ASYNC (Listar) ---
        public async Task<List<Menu>> GetAllAsync()
        {
            var menus = new List<Menu>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // NOTA: Asumo que el nombre de la tabla en BD sigue siendo 'Combo' o lo has renombrado a 'Menu'.
                // Usaré 'Combo' en la query por si la BD no se ha actualizado. Si se llama 'Menu', cámbialo aquí.
                string query = "SELECT Id, PlatoPrincipal, Bebida, Postre, Fecha, Precio FROM Combo"; 
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int platoId = reader.GetInt32(1);
                            int bebidaId = reader.GetInt32(2);
                            int postreId = reader.GetInt32(3);
                            
                            var menu = new Menu // Usando la clase Menu
                            {
                                Id = reader.GetInt32(0),
                                PlatoPrincipalId = platoId, 
                                BebidaId = bebidaId, 
                                PostreId = postreId, 
                                Fecha = reader.GetDateTime(4), 
                                Precio = Convert.ToDouble(reader.GetDecimal(5)), 
                                
                                // Carga de objetos de navegación
                                PlatoPrincipal = await _platoprincipalrepository.GetByIdAsync(platoId),
                                Bebida = await _bebidarepository.GetByIdAsync(bebidaId),
                                Postre = await _postrerepository.GetByIdAsync(postreId)
                            }; 
                            menus.Add(menu);
                        }
                    }
                }
            }
            return menus;
        }

        // --- GET BY ID ASYNC (Consultar) ---
        public async Task<Menu?> GetByIdAsync(int id)
        {
            Menu? menu = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // NOTA: Revisar el SELECT, faltaba una coma después de Postre
                string query = "SELECT Id, PlatoPrincipal, Bebida, Postre, Fecha, Precio FROM Combo WHERE Id = @Id"; 
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int platoId = reader.GetInt32(1);
                            int bebidaId = reader.GetInt32(2);
                            int postreId = reader.GetInt32(3);

                            menu = new Menu // Usando la clase Menu
                            {
                                Id = reader.GetInt32(0),
                                PlatoPrincipalId = platoId, 
                                BebidaId = bebidaId, 
                                PostreId = postreId,        
                                Fecha = reader.GetDateTime(4), 
                                Precio = Convert.ToDouble(reader.GetDecimal(5)), 
                                
                                PlatoPrincipal = await _platoprincipalrepository.GetByIdAsync(platoId),
                                Bebida = await _bebidarepository.GetByIdAsync(bebidaId),
                                Postre = await _postrerepository.GetByIdAsync(postreId)
                            };
                        }
                    }
                }
            }
            return menu;
        }

        // ADD ASYNC (Crear) 
        public async Task AddAsync(Menu menu) 
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    INSERT INTO Combo (PlatoPrincipal, Bebida, Postre, Fecha, Precio) 
                    VALUES (@PlatoPrincipalId, @BebidaId, @PostreId, @Fecha, @Precio)";
                    
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlatoPrincipalId", menu.PlatoPrincipalId);
                    command.Parameters.AddWithValue("@BebidaId", menu.BebidaId);
                    command.Parameters.AddWithValue("@PostreId", menu.PostreId);
                    command.Parameters.AddWithValue("@Fecha", menu.Fecha); 
                    command.Parameters.AddWithValue("@Precio", menu.Precio); 
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // UPDATE ASYNC (Actualizar) 
        public async Task UpdateAsync(Menu menu) // Usando la clase Menu
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    UPDATE Combo 
                    SET PlatoPrincipal = @PlatoPrincipalId, 
                        Bebida = @BebidaId, 
                        Postre = @PostreId,
                        Fecha = @Fecha,          
                        Precio = @Precio          
                    WHERE Id = @Id";
                    
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", menu.Id);
                    command.Parameters.AddWithValue("@PlatoPrincipalId", menu.PlatoPrincipalId);
                    command.Parameters.AddWithValue("@BebidaId", menu.BebidaId);
                    command.Parameters.AddWithValue("@PostreId", menu.PostreId);
                    command.Parameters.AddWithValue("@Fecha", menu.Fecha); 
                    command.Parameters.AddWithValue("@Precio", menu.Precio); 

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        // DELETE ASYNC (Eliminar) 
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM Combo WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        
        //  INICIALIZAR DATOS ASYNC 
        public async Task InicializarDatosAsync() {
            var plato = await _platoprincipalrepository.GetByIdAsync(1);
            var bebida = await _bebidarepository.GetByIdAsync(1);
            var postre = await _postrerepository.GetByIdAsync(1);

            if (plato != null && bebida != null && postre != null)
            {
                var menu = new Menu() // Usando la clase Menu
                {
                    PlatoPrincipalId = plato.Id, 
                    BebidaId = bebida.Id, 
                    PostreId = postre.Id,
                    
                    Fecha = DateTime.Today, 
                    
                    Precio = plato.Precio + bebida.Precio + postre.Precio  
                };
                menu.PlatoPrincipal = plato;
                menu.Bebida = bebida;
                menu.Postre = postre;

                await AddAsync(menu); 
            }
        }
    }
}