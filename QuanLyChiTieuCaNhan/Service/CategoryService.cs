using AutoMapper;
using QuanLyChiTieuCaNhan.DTO.Category;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository;

namespace QuanLyChiTieuCaNhan.Service
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto,string userId);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesByUserAsync(string userId);
    }
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, string userId)
        {
            var category = _mapper.Map<Category>(dto);
            category.UserId = userId;
            category = await _categoryRepository.AddAsync(category);
            
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            _mapper.Map(dto, category);
            return await _categoryRepository.UpdateAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesByUserAsync(string userId)
        {
            // Lấy danh sách các category thuộc về user
            var categories = await _categoryRepository.GetCategoriesByUserIdAsync(userId);

            // Map danh sách Category sang CategoryDto
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }
}
