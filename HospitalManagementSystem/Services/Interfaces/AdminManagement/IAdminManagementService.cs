using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Services.Interfaces.AdminManagement
{
    public interface IAdminManagementService
    {
        
        public Task<MessageResponseDto> RegisterAdminAsync(RegisterAdminDto registerDto);
        public Task<UpdateAdminResponseDto?> UpdateAdminAsync(int id, UpdateAdminRequestDto updateAdminRequest);
       public  Task<AdminResponseDto?> GetAdminByIdAsync(int id);
       public Task<bool> DeleteAdminAsync(int id);
        Task<PagedResponseDto<AdminDto>> GetPagedAdminsAsync(PaginationRequestDto dto);
    }
}
