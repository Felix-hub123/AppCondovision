using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CondoVision.Models
{
    public class EditProfileViewModel
    {
        public string? Id { get; set; }


        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string? FullName { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo {0} não é um endereço de e-mail válido.")]
        public string? Email { get; set; }

        [Display(Name = "Número de Telefone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Foto de Perfil")]
        public IFormFile? ImageFile { get; set; }
        public Guid? ImageId { get; set; }
    }
}
