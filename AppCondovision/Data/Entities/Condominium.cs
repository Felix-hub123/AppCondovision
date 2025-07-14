using System.ComponentModel.DataAnnotations;

namespace AppCondovision.Data.Entities
{
    public class Condominium : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        public int? CityId { get; set; }
        public int? CompanyId { get; set; }

        [MaxLength(50)]
        public string? Manager { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? TaxNumber { get; set; }

        public bool WasDeleted { get; set; } = false;

      

        // Navigation properties
        public virtual City? City { get; set; } // Objeto City associado
        public virtual Company? Company { get; set; } // Objeto Company associado
        public virtual ICollection<Fraction> Fractions { get; set; } = new List<Fraction>(); // Frações que compõem o condomínio
        public virtual ICollection<User> Users { get; set; } = new List<User>(); // Utilizadores (condóminos, gestores) associados ao condomínio
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>(); // Orçamentos do condomínio
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>(); // Despesas do condomínio
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>(); // Documentos do condomínio
        public virtual ICollection<Occurrence> Occurrences { get; set; } = new List<Occurrence>(); // Ocorrências reportadas no condomínio
        public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>(); // Reuniões (assembleias) do condomínio
        public virtual ICollection<Intervention> Interventions { get; set; } = new List<Intervention>(); // Intervenções/manutenções agendadas para o condomínio
        public virtual ICollection<Quota> Quotas { get; set; } = new List<Quota>(); // Quotas geradas para o condomínio
    }
}
