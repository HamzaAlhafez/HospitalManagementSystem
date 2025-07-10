using System.Threading.Tasks;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Services.Interfaces.AppointmentManagement
{
    public interface IAppointmentService
    {
        Task<IEnumerable<DoctorAvailableResponseDto>> GetAvailableDoctorsAsync();
        Task<IEnumerable<patientAvailableResponseDto>> GetAvailablePatientAsync();
        Task<AppointmentResponseDto> CreateAppointmentByAdminAsync(AppointmentAdminRequestDto dto);
        Task<AppointmentResponseDto> CreateAppointmentByDoctorAsync(AppointmentDoctorRequestDto dto, int userid, string doctorusername);
        Task<AppointmentResponseDto> CreateAppointmentByPatientAsync(AppointmentPatientRequestDto dto, int userid, string patientusername);
        Task<AppointmentResponseDto> GetAppointmentById(int id);
        Task<AppointmentResponseDto?> UpdateAppointmentByAdminAsync(int id, AppointmentAdminRequestDto dto);
        Task<AppointmentResponseDto?> UpdateAppointmentByDoctorAsync(int Appointemntid, AppointmentDoctorRequestDto dto, int userid);
        Task<AppointmentResponseDto?> UpdateAppointmentByPatientAsync(int Appointemntid, AppointmentPatientRequestDto dto, int userid);
        Task<PagedResponseDto<AppointmentResponseDto>> GetPagedAppointmentsAsync(PaginationRequestDto dto);
        Task<IEnumerable<AppointmentResponseDto?>> GetAppointmentDoctorByIdAsync(int userid);
        Task<IEnumerable<AppointmentResponseDto?>> GetAppointmentPatientByIdAsync(int userid);
        Task<MessageResponseDto> ConfirmAppointment(int appointmentId);
        Task<MessageResponseDto> CancelAppointment(int appointmentId, string cancelRequestDto);
        Task<MessageResponseDto> CompleteAppointment(int appointmentId);  

        Task<bool> DeleteAsync(int id);
    }
}
