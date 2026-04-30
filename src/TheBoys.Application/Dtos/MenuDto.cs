using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBoys.Application.Dtos
{
    public class MenuDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<MenuDto> SubMenus { get; set; } = new();
    }
}
