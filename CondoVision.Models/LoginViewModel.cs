using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CondoVision.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar os dados do formulário de login do utilizador,
    /// contendo as credenciais necessárias para autenticação.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Endereço de email do utilizador (usado como nome de utilizador para login).
        /// Deve estar num formato válido de email.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")] 
        [Display(Name = "E-mail")] 
        [EmailAddress(ErrorMessage = "O formato do endereço de e-mail é inválido.")] 
        public required string Username { get; set; } 

        /// <summary>
        /// Password do utilizador para autenticação.
        /// Deve ter no mínimo 6 caracteres.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [MinLength(6, ErrorMessage = "O campo {0} deve ter pelo menos {1} caracteres.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        /// <summary>
        /// Indica se o utilizador deseja manter a sessão iniciada ("lembrar-me").
        /// </summary>
        [Required] 
        [DisplayName("Lembrar-me?")] 
        public required bool RememberMe { get; set; }

    }
}
