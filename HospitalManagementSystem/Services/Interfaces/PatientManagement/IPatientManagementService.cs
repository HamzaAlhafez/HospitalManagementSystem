using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;

namespace HospitalManagementSystem.Services.Interfaces.PatientManagement
{
    public interface IPatientManagementService 
    {
        public Task<MessageResponseDto> CreatePatientAsync(CreatePatientDto createPatientDto);
        public Task<UpdatePatientResponseDto?> UpdatePatientAsync(int id, UpdatePatientRequestDto updatePatientRequest);
        public Task<PatientResponseDto?> GetPatientByIdAsync(int id);
        public Task<bool> DeletePatientAsync(int id);
        Task<PagedResponseDto<PatientDto>> GetPagedPatientsAsync(PaginationRequestDto dto);

    }
}
