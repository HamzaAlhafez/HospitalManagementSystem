using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Enums;

namespace HospitalManagementSystem.Repositories.Interfaces.AppointmentManagement
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetAppoitmentByIdforReadAsync(int id);
        Task<(IEnumerable<Appointment>, int TotalCount)> GetPagedAppointmentAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Appointment>> GetAppointmentDoctorByIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentPatientByIdAsync(int patientId);
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync();
        Task<IEnumerable<Patient>> GetAvailablePatientsAsync();
       
        Task<Appointment?> GetAppoitmentByIdForUpdateAsync(int id);


        Task<int> AddAsync(Appointment appointment);
        Task<bool> UpdateAsync(Appointment appointment);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
       
        
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime dateTime);

    }
}
