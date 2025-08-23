using System.ComponentModel.DataAnnotations;

namespace CondoVision.Models
{
    public class UnitViewModel
    {
        [Required(ErrorMessage = "O nome da unidade é obrigatório.")]
        [Display(Name = "Nome da Unidade")]
        public string? UnitName { get; set; }

        [Required(ErrorMessage = "A permilagem é obrigatória.")]
        [Range(0.01, 100.00, ErrorMessage = "A permilagem deve estar entre 0.01 e 100.00.")]
        [Display(Name = "Permilagem")]
        public decimal Permillage { get; set; }

        [Display(Name = "Proprietário")]
        public string? OwnerId { get; set; }

        [Display(Name = "Condomínio")]
        public int CondominiumId { get; set; }


        [Display(Name = "Proprietário")]
        public string? OwnerFullName { get; set; }

        [Display(Name = "Condomínio")]
        public string? CondominiumName { get; set; }


        [Display(Name = "ID da Unidade")]
        public int? Id { get; set; }

    }
}
