using Data.Contracts;
using Data.Implementations;
using Service.Contracts;
using Shared.DTOs.ClienteDTOs;
using System.Text.RegularExpressions;

namespace Service.Implementations
{
    public class ClienteService : IClienteService
    {
        private IClienteRepository _clienteRepository;
        private readonly ClienteMapper _mapper = new ClienteMapper();

        public ClienteService(IClienteRepository productoRepository)
        {
            _clienteRepository = productoRepository;
        }

        public async Task<List<ClienteReadDTO>> ObtenerTodosAsync()
        {
            var clientes = await _clienteRepository.FindAllAsync();
            return _mapper.ToReadDtoList(clientes);
        }

        public async Task<ClienteReadDTO> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var clientes = await _clienteRepository.ObtenerPorId(id);
            if (clientes == null)
                throw new KeyNotFoundException($"No se encontró ningún estado con ID {id}.");

            return _mapper.ToReadDto(clientes);
        }

        public async Task<ClienteReadDTO> CrearAsync(ClienteCreateDTO dto)
        {
            await ValidarClienteCreateDTO(dto);

            var cliente = _mapper.ToEntity(dto);
            await _clienteRepository.Create(cliente);

            return _mapper.ToReadDto(cliente);
        }

        public async Task<ClienteReadDTO> Editar(int id, ClienteUpdateDTO dto)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            await ValidarClienteUpdateDTO(dto);

            var cliente = await _clienteRepository.ObtenerPorId(id);
            if (cliente == null)
                throw new KeyNotFoundException($"No se encontró ningún cliente con ID {id}.");

            cliente.UpdatedDate = DateTime.Now;
            _mapper.UpdateEntity(dto, cliente);

            await _clienteRepository.Update(cliente);

            return _mapper.ToReadDto(cliente);
        }

        public async Task Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var cliente = await _clienteRepository.ObtenerPorId(id);
            if (cliente == null)
                throw new KeyNotFoundException($"No se encontró ningún cliente con ID {id}.");

            await _clienteRepository.Delete(cliente);
        }


        private async Task ValidarClienteCreateDTO(ClienteCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Nombre))
                throw new ArgumentException("El nombre del cliente es obligatorio.");
            if (dto.Dni > 99999999)
                throw new ArgumentException("El DNI debe contener 7 números como máximo.");
            if (!EsTelefonoValido(dto.Telefono))
                throw new ArgumentException("El numero de telefono deve comenzar con + y contener entre 7 y 15 caracteres.");
            if (string.IsNullOrEmpty(dto.Direccion))
                throw new ArgumentException("La dirección del cliente es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Ciudad))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Ciudad));
            if (dto.Ciudad.Length > 50)
                throw new ArgumentException("La ciudad debe tener al menos 50 caracteres.", nameof(dto.Ciudad));
            if (string.IsNullOrWhiteSpace(dto.Provincia))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Provincia));
            if (dto.Provincia.Length > 50)
                throw new ArgumentException("La provincia debe tener al menos 50 caracteres.", nameof(dto.Provincia));
            var hayCliente = _clienteRepository.FindAll();
            if (hayCliente.Count() != 0)
            {
                if (await _clienteRepository.ExistePorDniAsync(dto.Dni))
                    throw new ArgumentException("Ya existe un Cliente con ese numero de documento.", nameof(dto.Dni));
            }
        }

        private async Task ValidarClienteUpdateDTO(ClienteUpdateDTO dto)
        {
            if (await _clienteRepository.ExistePorDniAsync(dto.Dni))
                throw new ArgumentException("Ya existe un Cliente con ese numero de documento.", nameof(dto.Dni));
            if (string.IsNullOrEmpty(dto.Nombre))
                throw new ArgumentException("El nombre del cliente es obligatorio.");
            if (dto.Dni > 99999999)
                throw new ArgumentException("El DNI debe contener 7 números como máximo.");
            if (!EsTelefonoValido(dto.Telefono))
                throw new ArgumentException("El numero de telefono deve comenzar con + y contener entre 7 y 15 caracteres.");
            if (string.IsNullOrEmpty(dto.Direccion))
                throw new ArgumentException("La dirección del cliente es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Ciudad))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Ciudad));
            if (dto.Ciudad.Length > 50)
                throw new ArgumentException("La ciudad debe tener al menos 50 caracteres.", nameof(dto.Ciudad));
            if (string.IsNullOrWhiteSpace(dto.Provincia))
                throw new ArgumentException("La contraseña es obligatoria.", nameof(dto.Provincia));
            if (dto.Provincia.Length > 50)
                throw new ArgumentException("La provincia debe tener al menos 50 caracteres.", nameof(dto.Provincia));
        }

        public bool EsTelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;

            var regex = new Regex(@"^\+\d{7,15}$");
            return regex.IsMatch(telefono);
        }
    }
}
