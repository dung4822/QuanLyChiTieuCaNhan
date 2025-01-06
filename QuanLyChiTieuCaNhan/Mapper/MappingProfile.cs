using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChiTieuCaNhan.DTO.Auth;
using QuanLyChiTieuCaNhan.DTO.Budget;
using QuanLyChiTieuCaNhan.DTO.Category;
using QuanLyChiTieuCaNhan.DTO.ExpenseTransaction;
using QuanLyChiTieuCaNhan.Models;

namespace QuanLyChiTieuCaNhan.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, RegisterUserDto>().ReverseMap();

            CreateMap<User, UserResponseDto>()
      .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
      .ReverseMap();
            //Category Maper
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id, otp => otp.MapFrom(src => src.CategoryId))
                .ReverseMap();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<ExpenseTransaction, ExpenseTransactionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExpenseTransactionId)) // Map Id
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)); // Map tên Category

            // Mapping từ CreateExpenseTransactionDto sang ExpenseTransaction
            CreateMap<CreateExpenseTransactionDto, ExpenseTransaction>();

            // Mapping từ UpdateExpenseTransactionDto sang ExpenseTransaction
            CreateMap<UpdateExpenseTransactionDto, ExpenseTransaction>();




            //Budget
            CreateMap<Budget, BudgetDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BudgetId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

            CreateMap<CreateBudgetDto, Budget>();
            CreateMap<UpdateBudgetDto, Budget>();
        }
    }
}
