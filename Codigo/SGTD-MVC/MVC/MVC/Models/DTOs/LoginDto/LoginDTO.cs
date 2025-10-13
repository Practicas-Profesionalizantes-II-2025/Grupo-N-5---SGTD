using System.ComponentModel.DataAnnotations;

namespace MVC.Models.DTOs.LoginDto
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }
    }
}
