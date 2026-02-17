using System.ComponentModel.DataAnnotations.Schema;

namespace TheBoys.API.Entities;

[Table("prtl_news", Schema = "dbo")]
public class News
{
    [Column("News_Id")]
    public int NewsId { get; set; }

    [Column("News_date")]
    public DateTime? NewsDate { get; set; }

    [Column("News_img")]
    public string NewsImg { get; set; }

    [Column("Owner_ID")]
    public Guid OwnerId { get; set; }

    [Column("currentNews_date")]
    public DateTime? CurrentNewDate { get; set; }

    [Column("Published")]
    public bool Published { get; set; }

    [Column("IsFeatured")]
    public bool IsFeatured { get; set; }

    public ICollection<NewsTranslation> NewsTranslations { get; set; } =
        new List<NewsTranslation>();
}
