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
            //var allowedRootIds = new List<int> { 412, 415, 416, 417,442 , 539 }; 

            var rawData = await (from menu in _context.PrtlMenuUnivs
                                 join trans in _context.PrtlMenuUnivTranslations on menu.TranslationId equals trans.TranslationId
                                 where trans.LangId == langId
                                 && menu.Published == true
                                 select new
                                 {
                                     menu.MenuId,
                                     menu.ParentId,
                                     menu.Url,
                                     menu.Order,
                                     trans.TranslationData
                                 }).ToListAsync(cancellationToken);

            if (rawData == null || !rawData.Any())
                return new List<MenuDto>();

            var allowedIds = new List<int> {  415, 416, 417, 442 , 443 };

            var filteredList = rawData.Where(m => allowedIds.Contains(m.MenuId)).ToList();

            var articles = await _context.PrtlArticles
                                 .Where(a => a.Published == true)
                                 .ToListAsync(cancellationToken);

            var allItems = filteredList.Select(m => new MenuDto
            {
                Id = m.MenuId,
                ParentId = null, 
                Title = m.TranslationData,
                Order = m.Order,
                Url = BuildFinalUrl(m.Url, m.MenuId, articles),
                SubMenus = new List<MenuDto>() 
            }).OrderBy(m => m.Order).ToList();

            var item418 = allItems.FirstOrDefault(i => i.Id == 418);
            if (item418 != null) item418.Url = "http://mu.menofia.edu.eg/View/60828/ar";

            var rootMenus = allItems.Where(m => m.ParentId == null || !allItems.Any(x => x.Id == m.ParentId))
                           .OrderBy(m => m.Order)
                           .ToList();

            foreach (var root in rootMenus)
            {
                root.SubMenus = allItems.Where(x => x.ParentId == root.Id)
                                        .OrderBy(x => x.Order)
                                        .ToList();
            }

            return rootMenus;
        }


        public async Task<List<MenuDto>> GetCollegesMenuAsync(int langId, CancellationToken cancellationToken = default)
        {
            var rawData = await (from menu in _context.PrtlMenuUnivs
                                 join trans in _context.PrtlMenuUnivTranslations on menu.TranslationId equals trans.TranslationId
                                 where trans.LangId == langId
                                 && menu.Published == true
                                 select new
                                 {
                                     menu.MenuId,
                                     menu.ParentId,
                                     menu.Url,
                                     menu.Order,
                                     trans.TranslationData
                                 }).ToListAsync(cancellationToken);

            var articles = await _context.PrtlArticles
                                         .Where(a => a.Published == true)
                                         .ToListAsync(cancellationToken);
          

            int collegesParentId = 172799;

            var collegeItems = rawData
                .Where(m => m.ParentId == collegesParentId)
                .Select(m => new MenuDto
                {
                    Id = m.MenuId,
                    ParentId = m.ParentId,
                    Title = m.TranslationData,
                    Order = m.Order,
                    
                    Url = BuildFinalUrl(m.Url, m.MenuId, articles),
                   
                    SubMenus = new List<MenuDto>()
                })
                .OrderBy(m => m.Order)
                .ToList();

            return collegeItems;
        }

        private string BuildFinalUrl(string? dbUrl, int menuId, List<PrtlArticle> articles)
        {
            if (string.IsNullOrWhiteSpace(dbUrl) || dbUrl.Trim().ToLower() == "view" || dbUrl.Contains("mu.menofia.edu.eg/View/"))
            {
                var article = articles.FirstOrDefault(a => a.MenuItemId == menuId);

                if (article != null && !string.IsNullOrEmpty(article.Abbr))
                {
                    return $"http://mu.menofia.edu.eg/View/{article.Abbr}/ar";
                }
                return "#";
            }
            return dbUrl;
        }
    }

}