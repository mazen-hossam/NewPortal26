using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Dtos;
using TheBoys.Domain.Entities;
using TheBoys.Infrastructure.Persistence;

namespace TheBoys.Infrastructure.Services
{
    public class UniversityMenuService : IUniversityMenuService
    {
        private readonly ApplicationDbContext _context;

        public UniversityMenuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuDto>> GetFullMenuAsync(int langId, CancellationToken cancellationToken = default)
        {
            var menuList = await (from menu in _context.PrtlMenuUnivs
                               join trans in _context.PrtlMenuUnivTranslations on menu.TranslationId equals trans.TranslationId
                               where trans.LangId == langId
                               &&  menu.Published == true
                               && trans.TranslationData != "اختبار"
                               && trans.TranslationData != "اختبار1"
                               && menu.MenuId != 806  
                               && menu.MenuId != 495
                                  select new 
                               {
                                   menu.MenuId,
                                   menu.ParentId,
                                   menu.Url,
                                   menu.Order,
                                   trans.TranslationData

                               }).ToListAsync(cancellationToken);

            if (menuList == null || !menuList.Any())
                return new List<MenuDto>();

            var articles = await _context.PrtlArticles
                                     .Where(a => a.Published == true)
                                     .ToListAsync(cancellationToken);


            var allItems = menuList.Select(m => new MenuDto
            {
                Id = m.MenuId,
                ParentId = m.ParentId,
                Title = m.TranslationData,
                Order = m.Order,
                Url = BuildFinalUrl(m.Url, m.MenuId, articles)
            }).ToList();


            foreach (var item in allItems)
            {
                if (item.Id == 418)
                {
                    item.Url = "http://mu.menofia.edu.eg/View/60828/ar";
                }
            }


            var rootMenus = allItems.Where(m => m.ParentId == null).OrderBy(m => m.Order).ToList();

            foreach (var root in rootMenus)
            {
                root.SubMenus = allItems.Where(x => x.ParentId == root.Id)
                                        .OrderBy(x => x.Order)
                                        .ToList();
            }

            return rootMenus;
        }

        private string BuildFinalUrl(string? dbUrl, int menuId, List<PrtlArticle> articles)
        {

            //string baseUrl = "http://mu.menofia.edu.eg";

            if (string.IsNullOrWhiteSpace(dbUrl) || dbUrl.Trim().ToLower() == "view" || dbUrl.Contains("mu.menofia.edu.eg/View/"))
            {
                var article = articles.FirstOrDefault(a => a.MenuItemId == menuId);

                if (article != null&& !string.IsNullOrEmpty(article.Abbr))
                {
                    //return $"/details/{article.Abbr}";
                    return $"http://mu.menofia.edu.eg/View/{article.Abbr}/ar";
                }
                return "#";
            }
            return dbUrl;
        }

    }
}