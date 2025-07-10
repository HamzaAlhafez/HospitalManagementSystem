using System.Threading.Tasks;
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.Models.Entities;
using static HospitalManagementSystem.Repositories.DoctorManagemment.DoctorManagemmentRespository;

namespace HospitalManagementSystem.Repositories.Interfaces.DoctorManagemment
{
    public interface IDoctorManagemmentRespository
    {
        Task<bool> IsLicenseNumberExistAsync(string LicenseNumber);
       
        Task<int> CreateDoctorAsync(User user, Doctor doctor, UserRole userRole);
        Task<bool> UpdateDoctorAsync(Doctor doctor);
       

        Task<bool> IsLicenseNumberExistsIgnoringCurrentDoctorAsync(string LicenseNumber, int currentUserId);

        Task<bool> IsUsernameExistsIgnoringCurrentDoctorAsync(string username, int currentUserId);
        Task<bool> IsEmailExistsIgnoringCurrentDoctorAsync(string username, int currentUserId);

        Task<bool> DeleteDoctorAsync(int id);
        Task<(List<Doctor> Doctors, int TotalCount)> GetPagedDoctorsAsync(int pageNumber, int pageSize);
         Task<Doctor?> GetDoctorByIdForUpdateAsync(int id);
         Task<Doctor?> GetDoctorByIdReadOnlyAsync(int id);
       





    }
}
