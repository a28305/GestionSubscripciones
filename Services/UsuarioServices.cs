using GESTIONSUBSCRIPCIONES.Repository;
using GESTIONSUBSCRIPCIONES.models;
namespace GESTIONSUBSCRIPCIONES.Services;
{
    public class UsuarioService : IUsuarioservices
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _usuarioRepository.GetAllAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");

            return await _usuarioRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new ArgumentException("El nombre del plato no puede estar vacío.");

            if (usuario.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor que cero.");

            await _usuarioRepository.AddAsync(usuario);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            if (usuario.Id <= 0)
                throw new ArgumentException("El ID no es válido para actualización.");

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new ArgumentException("El nombre del plato no puede estar vacío.");

            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.");

            await _usuarioRepository.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync() {
            await _usuarioRepository.InicializarDatosAsync();
        }
    }
}
