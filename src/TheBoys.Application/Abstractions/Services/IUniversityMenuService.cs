using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBoys.Application.Dtos;

namespace TheBoys.Application.Abstractions.Services
{
    public interface IUniversityMenuService
    {
        Task<List<MenuDto>> GetFullMenuAsync(int langId, CancellationToken cancellationToken = default);
    }
}
