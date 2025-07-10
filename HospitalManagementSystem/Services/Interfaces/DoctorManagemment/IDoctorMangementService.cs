using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Services.Interfaces.DoctorManagemment
{
    public interface IDoctorMangementService
    {
        Task<CreateDoctorResponseDto?> CreateDoctorAsync(CreateDoctorRequestDto doctorRequestDto);
        Task<DoctorResponseDto?> GetDoctorByIdAsync(int id);
        Task<UpdateDoctorResponseDto?> UpdateDoctorAsync(int id, UpdateDoctorRequestDto doctorRequestDto);
        Task<bool> DeleteDoctorAsync(int id);
        Task<PagedResponseDto<DoctorDto>> GetPagedDoctorsAsync(PaginationRequestDto dto);



    }
}
