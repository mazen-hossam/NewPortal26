using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBoys.Domain.Entities
{
    [Table("prtl_Articles", Schema = "dbo")]
    public class PrtlArticle
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("Abbr")]
        public string? Abbr { get; set; }

        [Column("MenuItemId")]
        public int? MenuItemId { get; set; }

        [Column("Published")]
        public bool Published { get; set; }
    }
}
