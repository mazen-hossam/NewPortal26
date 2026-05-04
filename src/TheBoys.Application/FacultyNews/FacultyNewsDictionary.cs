namespace TheBoys.Application.FacultyNews;

public static class FacultyNewsDictionary
{
    public static readonly IReadOnlyDictionary<int, FacultyNewsConfig> Faculties =
        new Dictionary<int, FacultyNewsConfig>
        {
            [100] = new FacultyNewsConfig
            {
                PublicCode = 100,
                NameAr = "كلية العلوم",
                Abbr = "SCI",
                NewsTable = "prtl_news_sci",
                TranslationTable = "prtl_news_sci_trans"
            },
            [200] = new FacultyNewsConfig
            {
                PublicCode = 200,
                NameAr = "كلية الطب",
                Abbr = "MED",
                NewsTable = "prtl_news_med",
                TranslationTable = "prtl_news_med_trans"
            },
            [300] = new FacultyNewsConfig
            {
                PublicCode = 300,
                NameAr = "كلية الزراعة",
                Abbr = "AGR",
                NewsTable = "prtl_news_agr",
                TranslationTable = "prtl_news_agr_trans"
            },
            [400] = new FacultyNewsConfig
            {
                PublicCode = 400,
                NameAr = "كلية الهندسة",
                Abbr = "ENG",
                NewsTable = "prtl_news_eng",
                TranslationTable = "prtl_news_eng_trans"
            },
            [500] = new FacultyNewsConfig
            {
                PublicCode = 500,
                NameAr = "كلية التجارة",
                Abbr = "COM",
                NewsTable = "prtl_news_com",
                TranslationTable = "prtl_news_com_trans"
            },
            [600] = new FacultyNewsConfig
            {
                PublicCode = 600,
                NameAr = "كلية الحقوق",
                Abbr = "LAW",
                NewsTable = "prtl_news_law",
                TranslationTable = "prtl_news_law_trans"
            },
            [700] = new FacultyNewsConfig
            {
                PublicCode = 700,
                NameAr = "كلية طب الأسنان",
                Abbr = "DENT",
                NewsTable = "prtl_news_dent",
                TranslationTable = "prtl_news_dent_trans"
            },
            [800] = new FacultyNewsConfig
            {
                PublicCode = 800,
                NameAr = "كلية التمريض",
                Abbr = "NUR",
                NewsTable = "prtl_news_nur",
                TranslationTable = "prtl_news_nur_trans"
            },
            [900] = new FacultyNewsConfig
            {
                PublicCode = 900,
                NameAr = "كلية الصيدلة",
                Abbr = "Pharm",
                NewsTable = "prtl_news_Pharm",
                TranslationTable = "prtl_news_Pharm_trans"
            },
            [1000] = new FacultyNewsConfig
            {
                PublicCode = 1000,
                NameAr = "كلية الطب البيطري",
                Abbr = "VMed",
                NewsTable = "prtl_news_VMed",
                TranslationTable = "prtl_news_VMed_trans"
            },
            [1100] = new FacultyNewsConfig
            {
                PublicCode = 1100,
                NameAr = "كلية الذكاء الاصطناعي",
                Abbr = "AI",
                NewsTable = "prtl_news_AI",
                TranslationTable = "prtl_news_AI_trans"
            },
            [1200] = new FacultyNewsConfig
            {
                PublicCode = 1200,
                NameAr = "كلية الآداب",
                Abbr = "ART",
                NewsTable = "prtl_news_art",
                TranslationTable = "prtl_news_art_trans"
            },
            [1300] = new FacultyNewsConfig
            {
                PublicCode = 1300,
                NameAr = "كلية العلوم التطبيقية",
                Abbr = "ASCI",
                NewsTable = "prtl_news_ASCI",
                TranslationTable = "prtl_news_ASCI_trans"
            },
            [1400] = new FacultyNewsConfig
            {
                PublicCode = 1400,
                NameAr = "كلية التربية للطفولة المبكرة",
                Abbr = "ECEDU",
                NewsTable = "prtl_news_ECEDU",
                TranslationTable = "prtl_news_ECEDU_trans"
            },
            [1500] = new FacultyNewsConfig
            {
                PublicCode = 1500,
                NameAr = "كلية التربية",
                Abbr = "EDU",
                NewsTable = "prtl_news_edu",
                TranslationTable = "prtl_news_edu_trans"
            },
            [1600] = new FacultyNewsConfig
            {
                PublicCode = 1600,
                NameAr = "كلية التربية النوعية",
                Abbr = "EDV",
                NewsTable = "prtl_news_edv",
                TranslationTable = "prtl_news_edv_trans"
            },
            [1700] = new FacultyNewsConfig
            {
                PublicCode = 1700,
                NameAr = "كلية الفنون الجميلة",
                Abbr = "FA",
                NewsTable = "prtl_news_fa",
                TranslationTable = "prtl_news_fa_trans"
            },
            [1800] = new FacultyNewsConfig
            {
                PublicCode = 1800,
                NameAr = "كلية الحاسبات والمعلومات",
                Abbr = "FCI",
                NewsTable = "prtl_news_fci",
                TranslationTable = "prtl_news_fci_trans"
            },
            [1900] = new FacultyNewsConfig
            {
                PublicCode = 1900,
                NameAr = "FEE",
                Abbr = "FEE",
                NewsTable = "prtl_news_fee",
                TranslationTable = "prtl_news_fee_trans"
            },
            [2000] = new FacultyNewsConfig
            {
                PublicCode = 2000,
                NameAr = "كلية التربية الرياضية",
                Abbr = "FPE",
                NewsTable = "prtl_news_fpe",
                TranslationTable = "prtl_news_fpe_trans"
            },
            [2100] = new FacultyNewsConfig
            {
                PublicCode = 2100,
                NameAr = "كلية الاقتصاد المنزلي",
                Abbr = "HEC",
                NewsTable = "prtl_news_hec",
                TranslationTable = "prtl_news_hec_trans"
            },
            [2200] = new FacultyNewsConfig
            {
                PublicCode = 2200,
                NameAr = "Ho",
                Abbr = "Ho",
                NewsTable = "prtl_news_ho",
                TranslationTable = "prtl_news_ho_trans"
            },
            [2300] = new FacultyNewsConfig
            {
                PublicCode = 2300,
                NameAr = "LIV",
                Abbr = "LIV",
                NewsTable = "prtl_news_liv",
                TranslationTable = "prtl_news_liv_trans"
            },
            [2400] = new FacultyNewsConfig
            {
                PublicCode = 2400,
                NameAr = "كلية الإعلام",
                Abbr = "Media",
                NewsTable = "prtl_news_media",
                TranslationTable = "prtl_news_media_trans"
            }
        };

    public static bool TryGetFaculty(int publicCode, out FacultyNewsConfig config)
    {
        return Faculties.TryGetValue(publicCode, out config);
    }
}
