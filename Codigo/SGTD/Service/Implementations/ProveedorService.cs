using Data.Contracts;
using Data.Implementations;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Shared.DTOs.ProductoDTOs;
using Shared.DTOs.ProveedorDTOs;
using Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class ProveedorService : IProveedorService
    {
        private IProveedorRepository _proveedorRepository;
        private readonly ProveedorMapper _mapper = new ProveedorMapper();

        public ProveedorService(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        public async Task<List<ProveedorReadDTO>> ObtenerTodosAsync()
        {
            var proveedores = await _proveedorRepository.FindAllAsyncConRubros();

            return _mapper.ToReadDtoList(proveedores);
        }

        public async Task<ProveedorReadDTO> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var proveedor = await _proveedorRepository.ObtenerPorIdConRubros(id);
            if (proveedor == null)
                throw new KeyNotFoundException($"No se encontró ningún proveedor con ID {id}.");

            return _mapper.ToReadDtoWithRubros(proveedor);
        }
        public async Task<ProveedorReadDTO> CrearAsync(ProveedorCreateDTO dto)
        {
            await ValidarProveedorCreateDTO(dto);

            var proveedor = _mapper.ToEntity(dto);
            _mapper.CreateMapRubros(dto, proveedor);
            await _proveedorRepository.Create(proveedor);

            return _mapper.ToReadDtoWithRubros(proveedor);
        }

        public async Task<ProveedorReadDTO> Editar(int id, ProveedorUpdateDTO dto)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            await ValidarProveedorUpdateDTO(id, dto);

            dto.RubroIds = dto.RubroIds?.Distinct().ToList();
            var proveedor = await _proveedorRepository.ObtenerPorIdConRubros(id);
            if (proveedor == null)
                throw new KeyNotFoundException($"No se encontró ningún proveedor con ID {id}.");

            proveedor.UpdatedDate = DateTime.Now;
            _mapper.UpdateEntity(dto, proveedor);
            _mapper.UpdateMapRubros(dto, proveedor);
            await _proveedorRepository.Update(proveedor);

            return _mapper.ToReadDtoWithRubros(proveedor);
        }

        public async Task Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var proveedor = await _proveedorRepository.ObtenerPorId(id);
            if (proveedor == null)
                throw new KeyNotFoundException($"No se encontró ningún proveedor con ID {id}.");

            await _proveedorRepository.Delete(proveedor);
        }

        public async Task<IEnumerable<ProveedorReadDTO>> ObtenerProveedoresActivosAsync()
        {
            var proveedor = await _proveedorRepository.FindActivosAsync();
            return _mapper.ToReadDtoList(proveedor);
        }

        private async Task ValidarProveedorCreateDTO(ProveedorCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");
            if (!EsCuitValido(dto.Cuit))
                throw new ArgumentException("El CUIT no es válido. Formato esperado: XX-XXXXXXXX-X");
            if (string.IsNullOrWhiteSpace(dto.Direccion))
                throw new ArgumentException("La dirección es obligatoria.");
            if (!EsCorreoValido(dto.Correo))
                throw new ArgumentException("El correo no es válido.");
            if (!EsTelefonoValido(dto.Telefono))
                throw new ArgumentException("El número de teléfono debe comenzar con + y tener entre 7 y 15 dígitos.");
            if (string.IsNullOrWhiteSpace(dto.Ciudad))
                throw new ArgumentException("La ciudad es obligatoria.", nameof(dto.Ciudad));
            if (dto.Ciudad.Length > 50)
                throw new ArgumentException("La ciudad debe tener al menos 50 caracteres.", nameof(dto.Ciudad));
            if (string.IsNullOrWhiteSpace(dto.Provincia))
                throw new ArgumentException("La provincia es obligatoria.", nameof(dto.Provincia));
            if (dto.Provincia.Length > 50)
                throw new ArgumentException("La provincia debe tener al menos 50 caracteres.", nameof(dto.Provincia));
            if (dto.EstadoId <= 0)
                throw new ArgumentException("Debe seleccionar un estado válido.");

            var hayUsuario = _proveedorRepository.FindAll();
            if (hayUsuario.Count() != 0)
            {
                if (await _proveedorRepository.ExistePorCuitAsync(dto.Cuit))
                    throw new ArgumentException("Ya existe un proveedor con ese número de CUIT.", nameof(dto.Cuit));
            }
        }

        private async Task ValidarProveedorUpdateDTO(int id, ProveedorUpdateDTO dto)
        {
            if (dto.Id <= 0)
                throw new ArgumentException("ID inválido.");

            if (await _proveedorRepository.ExistePorCuitAsync(dto.Cuit, id))
                throw new ArgumentException("Ya existe un proveedor con ese número de CUIT.", nameof(dto.Cuit));

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");
            if (!EsCuitValido(dto.Cuit))
                throw new ArgumentException("El CUIT no es válido. Formato esperado: XX-XXXXXXXX-X");
            if (string.IsNullOrWhiteSpace(dto.Direccion))
                throw new ArgumentException("La dirección es obligatoria.");
            if (!EsCorreoValido(dto.Correo))
                throw new ArgumentException("El correo no es válido.");
            if (!EsTelefonoValido(dto.Telefono))
                throw new ArgumentException("El número de teléfono debe comenzar con + y tener entre 7 y 15 dígitos.");
            if (string.IsNullOrWhiteSpace(dto.Ciudad))
                throw new ArgumentException("La ciudad es obligatoria.", nameof(dto.Ciudad));
            if (dto.Ciudad.Length > 50)
                throw new ArgumentException("La ciudad debe tener al menos 50 caracteres.", nameof(dto.Ciudad));
            if (string.IsNullOrWhiteSpace(dto.Provincia))
                throw new ArgumentException("La provincia es obligatoria.", nameof(dto.Provincia));
            if (dto.Provincia.Length > 50)
                throw new ArgumentException("La provincia debe tener al menos 50 caracteres.", nameof(dto.Provincia));
            if (dto.EstadoId <= 0)
                throw new ArgumentException("Debe seleccionar un estado válido.");
        }

        private bool EsTelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;

            var regex = new Regex(@"^\+\d{7,15}$");
            return regex.IsMatch(telefono);
        }

        private bool EsCorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            var regex = new Regex(@"^[\w\.-]+@[\w\.-]+\.\w+$");
            return regex.IsMatch(correo);
        }

        private bool EsCuitValido(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit))
                return false;

            var regex = new Regex(@"^\d{2}-\d{8}-\d$");
            return regex.IsMatch(cuit);
        }
       
    }
}
