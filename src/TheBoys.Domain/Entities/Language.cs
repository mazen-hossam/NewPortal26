using System.ComponentModel.DataAnnotations.Schema;

namespace TheBoys.Domain.Entities;

[Table("prtl_Languages", Schema = "dbo")]
public class Language
{
    [Column("Lang_Id")]
    public int Id { get; set; }

    [Column("LCID")]
    public string LCID { get; set; }
}

public class LanguageModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Flag { get; set; }
}
