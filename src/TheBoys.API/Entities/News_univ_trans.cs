using System.ComponentModel.DataAnnotations.Schema;

namespace TheBoys.API.Entities;

[Table("prtl_news_univ_trans", Schema = "dbo")]
public class News_univ_trans
{
    [Column("id")]
    public int Id { get; set; }

    [Column("News_Head")]
    public string NewsHead { get; set; }

    [Column("News_Abbr")]
    public string NewsAbbr { get; set; }

    [Column("News_Body")]
    public string NewsBody { get; set; }

    [Column("News_Source")]
    public string NewsSource { get; set; }

    [ForeignKey(name: nameof(Language))]
    [Column("Lang_Id")]
    public int LangId { get; set; }

    [Column("Img_alt")]
    public string ImgAlt { get; set; }

    [Column("News_Id")]
    public int NewsId { get; set; }



    [ForeignKey(nameof(NewsId))]
    public News_univ News_univ { get; set; }


    [ForeignKey(nameof(LangId))]
    public Language Language { get; set; }
}
