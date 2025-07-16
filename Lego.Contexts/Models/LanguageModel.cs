using System.ComponentModel.DataAnnotations;

namespace Lego.Contexts.Models
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(10)]
         public string LanguageLocaleCode { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public bool IsRtl { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;
       
    }
}
