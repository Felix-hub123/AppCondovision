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
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string? Password { get; set; }

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }


      

    }
}
