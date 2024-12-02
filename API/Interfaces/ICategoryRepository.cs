using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id);
        Task<ICollection<Category>> GetCategoriesAsync();
        void Add(Category category);
        void UpdateCategory(Category category);
    }
}