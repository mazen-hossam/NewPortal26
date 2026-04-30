using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBoys.Domain.Entities
{
    [Table("prtl_menu_univ_trans", Schema = "dbo")]
    public class PrtlMenuUnivTranslation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("Translation_ID")]
        public string TranslationId { get; set; } = string.Empty;

        [Column("Translation_Data")]
        public string TranslationData { get; set; } = string.Empty;

        [Column("Lang_Id")]
        public int LangId { get; set; }

        [ForeignKey(nameof(LangId))]
        public Language Language { get; set; } = null!;
    }
}
