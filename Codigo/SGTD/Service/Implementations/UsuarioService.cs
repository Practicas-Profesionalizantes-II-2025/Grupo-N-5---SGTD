using Data.Contracts;
using Data.Repositorios.Contracts;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Service.Mappers;
using Shared.DTOs.ClienteDTOs;
using Shared.DTOs.UsuarioDTOs;
using Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private IUsuarioRepository _usuarioRepository;
        private readonly UsuarioMapper _mapper = new UsuarioMapper();

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioReadDTO>> ObtenerTodosAsync()
        {
            var usuario = await _usuarioRepository.FindAllAsync();
            return _mapper.ToReadDtoList(usuario);
        }
        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("El correo no puede ser vacío.");

            // Llamamos al repositorio para buscar el usuario por correo
            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(correo);

            // Si no existe, devolvemos null
            if (usuario == null)
                return null;

            return usuario;
        }


        public async Task<UsuarioReadDTO> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if (usuario == null)
                throw new KeyNotFoundException($"No se encontró ningún usuario con ID {id}.");

            return _mapper.ToReadDto(usuario);
        }

        public async Task<UsuarioReadDTO> CrearAsync(UsuarioCreateDTO dto)
        {
            await ValidarUsuarioCreateDTO(dto);


            var usuario = _mapper.ToEntity(dto);

            usuario.Contrasenia = BCrypt.Net.BCrypt.HashPassword(dto.Contrasenia);

            await _usuarioRepository.Create(usuario);

            return _mapper.ToReadDto(usuario);
        }

        public async Task<UsuarioReadDTO> Editar(int id, UsuarioUpdateDTO dto)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            await ValidarUsuarioUpdateDTO(id, dto);

            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if (usuario == null)
                throw new KeyNotFoundException($"No se encontró ningún usuario con ID {id}.");

            usuario.UpdatedDate = DateTime.Now;

            _mapper.UpdateEntity(dto, usuario);

            await _usuarioRepository.Update(usuario);

            return _mapper.ToReadDto(usuario);
        }

        public async Task Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if (usuario == null)
                throw new KeyNotFoundException($"No se encontró ningún usuario con ID {id}.");

            await _usuarioRepository.Delete(usuario);
        }
        public async Task<Usuario?> LoginAsync(string correo, string contraseña)
        {
            var user = await _usuarioRepository.ObtenerPorCorreoAsync(correo);

            if (user == null)
                return null;

            // Verificar la contraseña (ej: usando BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(contraseña, user.Contrasenia))
                return null;

            if (user.EstadoId != 1) // 1 = Activo
                return null;

            return user; // devolver usuario válido
        }
        private async Task ValidarUsuarioCreateDTO(UsuarioCreateDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "El objeto UsuarioCreateDTO no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(dto.TipoDocumento))
                throw new ArgumentException("El tipo de documento debe ser un valor válido.", nameof(dto.TipoDocumento));

            if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
                throw new ArgumentException("El número de documento es obligatorio.", nameof(dto.NumeroDocumento));
            if (dto.NumeroDocumento.Length < 6 || dto.NumeroDocumento.Length > 15)
                throw new ArgumentException("El número de documento debe tener entre 6 y 15 caracteres.", nameof(dto.NumeroDocumento));

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(dto.Nombre));

            if (string.IsNullOrWhiteSpace(dto.Apellido))
                throw new ArgumentException("El apellido es obligatorio.", nameof(dto.Apellido));

            if (string.IsNullOrWhiteSpace(dto.CorreoElectronico))
                throw new ArgumentException("El correo electrónico es obligatorio.", nameof(dto.CorreoElectronico));
            if (!Regex.IsMatch(dto.CorreoElectronico, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El formato del correo electrónico no es válido.", nameof(dto.CorreoElectronico));

            if (string.IsNullOrWhiteSpace(dto.Contrasenia))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Contrasenia));
            if (dto.Contrasenia.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.", nameof(dto.Contrasenia));

            if (string.IsNullOrWhiteSpace(dto.Ciudad))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Ciudad));
            if (dto.Ciudad.Length > 50)
                throw new ArgumentException("La ciudad debe tener menos de 50 caracteres.", nameof(dto.Ciudad));

            if (string.IsNullOrWhiteSpace(dto.Provincia))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Provincia));
            if (dto.Provincia.Length > 50)
                throw new ArgumentException("La provincia debe tener menos de 50 caracteres.", nameof(dto.Provincia));

            if (dto.RolId <= 0)
                throw new ArgumentException("El rol debe ser seleccionado.", nameof(dto.RolId));

            if (dto.EstadoId != null && dto.EstadoId <= 0)
                throw new ArgumentException("El estado debe ser un valor válido si se especifica.", nameof(dto.EstadoId));
            var hayUsuario = _usuarioRepository.FindAll();
            if (hayUsuario.Count() != 0)
            {
                if (await _usuarioRepository.ExistePorDniAsync(dto.NumeroDocumento))
                    throw new ArgumentException("Ya existe un usuario con ese número de documento.", nameof(dto.NumeroDocumento));

                if (await _usuarioRepository.ExistePorCorreoAsync(dto.CorreoElectronico))
                    throw new ArgumentException("Ya existe un usuario con ese correo electrónico.", nameof(dto.CorreoElectronico));
            }
        }
        

        private async Task ValidarUsuarioUpdateDTO(int id, UsuarioUpdateDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "El objeto UsuarioUpdateDTO no puede ser nulo.");

            if (dto.Id != id)
                throw new ArgumentException("El ID del cuerpo no coincide con el ID de la ruta.");

            if (string.IsNullOrWhiteSpace(dto.TipoDocumento))
                throw new ArgumentException("El tipo de documento debe ser un valor válido.", nameof(dto.TipoDocumento));

            if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
                throw new ArgumentException("El número de documento es obligatorio.", nameof(dto.NumeroDocumento));
            if (dto.NumeroDocumento.Length < 6 || dto.NumeroDocumento.Length > 15)
                throw new ArgumentException("El número de documento debe tener entre 6 y 15 caracteres.", nameof(dto.NumeroDocumento));

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(dto.Nombre));

            if (string.IsNullOrWhiteSpace(dto.Apellido))
                throw new ArgumentException("El apellido es obligatorio.", nameof(dto.Apellido));

            if (string.IsNullOrWhiteSpace(dto.CorreoElectronico))
                throw new ArgumentException("El correo electrónico es obligatorio.", nameof(dto.CorreoElectronico));
            if (!Regex.IsMatch(dto.CorreoElectronico, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El formato del correo electrónico no es válido.", nameof(dto.CorreoElectronico));

            if (string.IsNullOrWhiteSpace(dto.Contrasenia))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Contrasenia));
            if (dto.Contrasenia.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.", nameof(dto.Contrasenia));

            if(string.IsNullOrWhiteSpace(dto.Ciudad))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Ciudad));
            if (dto.Ciudad.Length < 50)
                throw new ArgumentException("La ciudad debe tener al menos 50 caracteres.", nameof(dto.Ciudad));

            if (string.IsNullOrWhiteSpace(dto.Provincia))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Provincia));
            if (dto.Provincia.Length < 50)
                throw new ArgumentException("La provincia debe tener al menos 50 caracteres.", nameof(dto.Provincia));

            if (dto.RolId <= 0)
                throw new ArgumentException("El rol debe ser seleccionado.", nameof(dto.RolId));

            if (dto.EstadoId != null && dto.EstadoId <= 0)
                throw new ArgumentException("El estado debe ser un valor válido si se especifica.", nameof(dto.EstadoId));
            var hayUsuario = _usuarioRepository.FindAll();
            if (hayUsuario != null)
            {
                if (await _usuarioRepository.ExistePorDniAsync(dto.NumeroDocumento))
                    throw new ArgumentException("Ya existe un usuario con ese número de documento.", nameof(dto.NumeroDocumento));

                if (await _usuarioRepository.ExistePorCorreoAsync(dto.CorreoElectronico))
                    throw new ArgumentException("Ya existe un usuario con ese correo electrónico.", nameof(dto.CorreoElectronico));
            }

        }

    }
}
