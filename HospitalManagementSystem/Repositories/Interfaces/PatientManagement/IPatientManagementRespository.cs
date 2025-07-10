using HospitalManagementSystem.Models.Entities;

namespace HospitalManagementSystem.Repositories.Interfaces.PatientManagement
{
    public interface IPatientManagementRespository
    {
        Task<bool> CreatePatientAsync(User user, Patient patient, UserRole userRole);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<Patient?> GetPatientByIdForUpdateAsync(int id);
        Task<Patient?> GetPatientByIdReadOnlyAsync(int id);
        Task<bool> IsUsernameExistsIgnoringCurrentPatientAsync(string username, int currentUserId);
        Task<bool> IsEmailExistsIgnoringCurrentPatientAsync(string Email, int currentUserId);
        Task<bool> IsInsuranceNumberExistsIgnoringCurrentPatientAsync(string InsuranceNumber, int currentUserId);
        Task<bool> IsInsuranceNumberExistAsync(string InsuranceNumber);
        Task<(List<Patient> patients, int TotalCount)> GetPagedpatientsAsync(int pageNumber, int pageSize);

        Task<bool> DeletePatientAsync(int id);

    }
}
