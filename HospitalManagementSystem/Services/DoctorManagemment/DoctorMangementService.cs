using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Enums;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.Auth;
using HospitalManagementSystem.Repositories.Interfaces.DoctorManagemment;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Services.Interfaces.DoctorManagemment;
using Serilog;




namespace HospitalManagementSystem.Services.DoctorManagemment
{
    /// <summary>
    /// Service layer for doctor management operations
    /// </summary>
    public class DoctorMangementService : IDoctorMangementService
    {
        private readonly IAuthRespository _Authrespository;
        private readonly IDoctorManagemmentRespository _DoctorManagemmentRespository;
        private readonly IUserManagementRespository _userManagementRespository;

        /// <summary>
        /// Initializes a new instance of the DoctorMangementService
        /// </summary>
        /// <param name="DoctorManagemmentRespository">Doctor management repository</param>
        /// <param name="Authrespository">Authentication repository</param>
        /// <param name="userManagementRespository">User management repository</param>
        public DoctorMangementService(IDoctorManagemmentRespository DoctorManagemmentRespository,
                                   IAuthRespository Authrespository,
                                   IUserManagementRespository userManagementRespository)
        {
            _DoctorManagemmentRespository = DoctorManagemmentRespository;
            _Authrespository = Authrespository;
            _userManagementRespository = userManagementRespository;
        }

        /// <summary>
        /// Creates a new doctor in the system
        /// </summary>
        /// <param name="doctorRequestDto">Doctor creation request data</param>
        /// <returns>Response containing creation status and details</returns>
        public async Task<CreateDoctorResponseDto?> CreateDoctorAsync(CreateDoctorRequestDto doctorRequestDto)
        {
            Log.Information("Starting doctor creation process for username: {Username}", doctorRequestDto.Username);

            // Check if email exists
            if (await _userManagementRespository.IsEmailExistAsync(doctorRequestDto.Email))
            {
                Log.Warning("Email already exists: {Email}", doctorRequestDto.Email);
                return new CreateDoctorResponseDto { Message = "Email is already registered!", IsSuccess = false };
            }

            // Check if Username exists
            if (await _userManagementRespository.IsUsernameExistAsync(doctorRequestDto.Username))
            {
                Log.Warning("Username already exists: {Username}", doctorRequestDto.Username);
                return new CreateDoctorResponseDto { Message = "Username is already taken!", IsSuccess = false };
            }

            if (await _DoctorManagemmentRespository.IsLicenseNumberExistAsync(doctorRequestDto.LicenseNumber))
            {
                Log.Warning("License number already exists: {LicenseNumber}", doctorRequestDto.LicenseNumber);
                return new CreateDoctorResponseDto { Message = "LicenseNumber is already taken!", IsSuccess = false };
            }

            var user = new User
            {
                Username = doctorRequestDto.Username,
                Email = doctorRequestDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(doctorRequestDto.Password)
            };

            var doctor = new Doctor
            {
                Specialization = doctorRequestDto.Specialization,
                LicenseNumber = doctorRequestDto.LicenseNumber
            };

            var userRole = new UserRole
            {
                RoleId = (int)SystemRoles.Doctor
            };

            
           var doctorid = await _DoctorManagemmentRespository.CreateDoctorAsync(user, doctor, userRole);

            if (doctorid != -1)
            {
                Log.Information("Successfully created doctor with ID: {DoctorId}", doctorid);
                return new CreateDoctorResponseDto
                {
                    Message = "Created Doctor completed successfully",
                    IsSuccess = true,
                    id = doctorid
                };
            }
            else
            {
                Log.Error("Failed to create doctor for username: {Username}", doctorRequestDto.Username);
                return new CreateDoctorResponseDto
                {
                    Message = "An error occurred during Doctor. Please try again",
                    IsSuccess = false
                };
            }
        }

        /// <summary>
        /// Deletes a doctor from the system
        /// </summary>
        /// <param name="id">ID of the doctor to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteDoctorAsync(int id)
        {
            Log.Information("Attempting to delete doctor with ID: {DoctorId}", id);
            var result = await _DoctorManagemmentRespository.DeleteDoctorAsync(id);

            if (result)
            {
                Log.Information("Successfully deleted doctor with ID: {DoctorId}", id);
            }
            else
            {
                Log.Error("Failed to delete doctor with ID: {DoctorId}", id);
            }

            return result;
        }

        /// <summary>
        /// Retrieves doctor details by ID
        /// </summary>
        /// <param name="id">ID of the doctor to retrieve</param>
        /// <returns>Doctor response data if found, null otherwise</returns>
        public async Task<DoctorResponseDto?> GetDoctorByIdAsync(int id)
        {
            Log.Debug("Fetching doctor details for ID: {DoctorId}", id);
            var doctor = await _DoctorManagemmentRespository.GetDoctorByIdReadOnlyAsync(id);

            if (doctor?.User == null)
            {
                Log.Warning("Doctor not found for ID: {DoctorId}", id);
                return null;
            }

            Log.Debug("Successfully retrieved doctor with ID: {DoctorId}", id);
            return new DoctorResponseDto
            {
                Username = doctor.User.Username,
                Email = doctor.User.Email,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                IsActive = doctor.User.IsActive
            };
        }

        /// <summary>
        /// Retrieves a paginated list of doctors
        /// </summary>
        /// <param name="dto">Pagination request parameters</param>
        /// <returns>Paged response containing doctor data</returns>
        public async Task<PagedResponseDto<DoctorDto>> GetPagedDoctorsAsync(PaginationRequestDto dto)
        {
            Log.Debug("Fetching paged doctors - Page: {PageNumber}, Size: {PageSize}",
                dto.PageNumber, dto.PageSize);

            var (doctors, totalCount) = await _DoctorManagemmentRespository.GetPagedDoctorsAsync(dto.PageNumber, dto.PageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize);
            var doctorDtos = doctors.Select(d => new DoctorDto
            {
                DoctorId = d.DoctorId,
                Specialization = d.Specialization,
                LicenseNumber = d.LicenseNumber,
                User = new UserDto
                {
                    Username = d.User.Username,
                    Email = d.User.Email,
                    IsActive = d.User.IsActive
                }
            }).ToList();

            
            Log.Debug("Returning {AppointmentCount} doctors out of {TotalCount}", doctorDtos.Count, totalCount);

            return new PagedResponseDto<DoctorDto>
            {
                Items = doctorDtos,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }

        /// <summary>
        /// Updates doctor information
        /// </summary>
        /// <param name="id">ID of the doctor to update</param>
        /// <param name="doctorRequestDto">Updated doctor data</param>
        /// <returns>Response containing update status and details</returns>
        public async Task<UpdateDoctorResponseDto?> UpdateDoctorAsync(int id, UpdateDoctorRequestDto doctorRequestDto)
        {
            Log.Information("Starting update process for doctor ID: {DoctorId}", id);

            // Check if email exists
            if (await _DoctorManagemmentRespository.IsEmailExistsIgnoringCurrentDoctorAsync(doctorRequestDto.Email, id))
            {
                Log.Warning("Email already exists for another doctor: {Email}", doctorRequestDto.Email);
                return new UpdateDoctorResponseDto
                {
                    Message = "Email is already registered!",
                    IsSuccess = false
                };
            }

            // Check if Username exists
            if (await _DoctorManagemmentRespository.IsUsernameExistsIgnoringCurrentDoctorAsync(doctorRequestDto.Username, id))
            {
                Log.Warning("Username already exists for another doctor: {Username}", doctorRequestDto.Username);
                return new UpdateDoctorResponseDto
                {
                    Message = "Username is already taken!",
                    IsSuccess = false
                };
            }

            if (await _DoctorManagemmentRespository.IsLicenseNumberExistsIgnoringCurrentDoctorAsync(doctorRequestDto.LicenseNumber, id))
            {
                Log.Warning("License number already exists for another doctor: {LicenseNumber}", doctorRequestDto.LicenseNumber);
                return new UpdateDoctorResponseDto
                {
                    Message = "LicenseNumber is already taken!",
                    IsSuccess = false
                };
            }

            var doctor = await _DoctorManagemmentRespository.GetDoctorByIdForUpdateAsync(id);
            if (doctor == null)
            {
                Log.Warning("Doctor not found for update - ID: {DoctorId}", id);
                return null;
            }

            Log.Debug("Updating doctor details for ID: {DoctorId}", id);
            doctor.User.Username = doctorRequestDto.Username;
            doctor.User.Email = doctorRequestDto.Email;
            doctor.Specialization = doctorRequestDto.Specialization;
            doctor.LicenseNumber = doctorRequestDto.LicenseNumber;

            var isSuccess = await _DoctorManagemmentRespository.UpdateDoctorAsync(doctor);

            if (isSuccess)
            {
                Log.Information("Successfully updated doctor with ID: {DoctorId}", id);
                return new UpdateDoctorResponseDto
                {
                    IsSuccess = true,
                    Message = "Doctor data has been updated successfully!",
                    Email = doctor.User.Email,
                    Username = doctor.User.Username,
                    LicenseNumber = doctor.LicenseNumber,
                    Specialization = doctor.Specialization
                };
            }
            else
            {
                Log.Error("Failed to update doctor with ID: {DoctorId}", id);
                return new UpdateDoctorResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while updating!"
                };
            }
        }
    }
}