using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBoys.Domain.Entities
{
    [Table("prtl_menu_univ", Schema = "dbo")]
    public class PrtlMenuUniv
    {
        [Key]
        [Column("Menu_id")]
        public int MenuId { get; set; }

        [Column("Parent_id")]
        public int? ParentId { get; set; }

        [Column("Order")]
        public int Order { get; set; }

        [Column("Url")]
        public string? Url { get; set; }

        [Column("Translation_ID")]
        public string? TranslationId { get; set; }

        [Column("Owner_ID")]
        public Guid? OwnerId { get; set; }

        [Column("Published")]
        public bool Published { get; set; }

        public ICollection<PrtlMenuUnivTranslation> Translations { get; set; } = new List<PrtlMenuUnivTranslation>();
    }
}
