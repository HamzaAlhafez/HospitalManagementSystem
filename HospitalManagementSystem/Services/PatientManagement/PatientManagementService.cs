using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Enums;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.PatientManagement;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Services.Interfaces.PatientManagement;
using Serilog;




namespace HospitalManagementSystem.Services.PatientManagement
{
    /// <summary>
    /// Service for managing patient operations and business logic
    /// </summary>
    public class PatientManagementService : IPatientManagementService
    {
        private readonly IUserManagementRespository _userManagementRespository;
        private readonly IPatientManagementRespository _PatientManagementRespository;

        /// <summary>
        /// Initializes a new instance of the PatientManagementService
        /// </summary>
        /// <param name="userManagementRespository">User management repository</param>
        /// <param name="PatientManagementRespository">Patient management repository</param>
        public PatientManagementService(
            IUserManagementRespository userManagementRespository,
            IPatientManagementRespository PatientManagementRespository)
        {
            _userManagementRespository = userManagementRespository;
            _PatientManagementRespository = PatientManagementRespository;
        }

        /// <summary>
        /// Creates a new patient account
        /// </summary>
        /// <param name="createPatientDto">Patient creation data</param>
        /// <returns>Response indicating success or failure</returns>
        public async Task<MessageResponseDto> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            Log.Information("Starting patient creation for username: {Username}", createPatientDto.Username);

            // Check if email exists
            if (await _userManagementRespository.IsEmailExistAsync(createPatientDto.Email))
            {
                Log.Warning("Email already exists: {Email}", createPatientDto.Email);
                return new MessageResponseDto { Message = "Email is already registered!", IsSuccess = false };
            }

            // Check if Username exists
            if (await _userManagementRespository.IsUsernameExistAsync(createPatientDto.Username))
            {
                Log.Warning("Username already exists: {Username}", createPatientDto.Username);
                return new MessageResponseDto { Message = "Username is already taken!", IsSuccess = false };
            }

            // Check if InsuranceNumber exists
            if (!string.IsNullOrEmpty(createPatientDto.InsuranceNumber) &&
                await _PatientManagementRespository.IsInsuranceNumberExistAsync(createPatientDto.InsuranceNumber))
            {
                Log.Warning("Insurance number already exists: {InsuranceNumber}", createPatientDto.InsuranceNumber);
                return new MessageResponseDto
                {
                    Message = "InsuranceNumber is already taken!",
                    IsSuccess = false
                };
            }

            // Create user and patient entities
            var user = new User()
            {
                Username = createPatientDto.Username,
                Email = createPatientDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createPatientDto.Password)
            };

            var patient = new Patient()
            {
                DateOfBirth = createPatientDto.DateOfBirth,
                InsuranceNumber = string.IsNullOrWhiteSpace(createPatientDto.InsuranceNumber) ?
                    null : createPatientDto.InsuranceNumber,
            };
            var userRole = new UserRole()
            {
                RoleId = (int)SystemRoles.Patient
            };




            Log.Debug("Attempting to create patient in repository");
            if (await _PatientManagementRespository.CreatePatientAsync(user, patient, userRole))
            {
                Log.Information("Patient created successfully: {Username}", createPatientDto.Username);
                return new MessageResponseDto
                {
                    Message = "Registration completed successfully",
                    IsSuccess = true
                };
            }
            else
            {
                Log.Error("Failed to create patient: {Username}", createPatientDto.Username);
                return new MessageResponseDto
                {
                    Message = "An error occurred during registration. Please try again",
                    IsSuccess = false
                };
            }
        }

        /// <summary>
        /// Deletes a patient by ID
        /// </summary>
        /// <param name="id">ID of the patient to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeletePatientAsync(int id)
        {
            Log.Information("Attempting to delete patient with ID: {PatientId}", id);
            var result = await _PatientManagementRespository.DeletePatientAsync(id);
            Log.Information("Delete operation for patient {PatientId} completed with result: {Result}", id, result);
            return result;
        }

        /// <summary>
        /// Retrieves a paginated list of patients
        /// </summary>
        /// <param name="dto">Pagination parameters</param>
        /// <returns>Paged response containing patient data</returns>
        public async Task<PagedResponseDto<PatientDto>> GetPagedPatientsAsync(PaginationRequestDto dto)
        {
            Log.Debug("Fetching paged patients - Page {PageNumber}, Size {PageSize}",
                dto.PageNumber, dto.PageSize);

            var (Patients, totalCount) = await _PatientManagementRespository.GetPagedpatientsAsync(
                dto.PageNumber,
                dto.PageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize);

            var pateintsDtos = Patients.Select(p => new PatientDto
            {
                PatientId = p.PatientId,
                DateOfBirth = p.DateOfBirth,
                InsuranceNumber = p.InsuranceNumber,
                User = new UserDto
                {
                    Username = p.User.Username,
                    Email = p.User.Email,
                    IsActive = p.User.IsActive
                }
            }).ToList();

            Log.Information("Retrieved {AppointmentCount} patients out of {TotalCount} for page {PageNumber}",
                pateintsDtos.Count, totalCount, dto.PageNumber);

            return new PagedResponseDto<PatientDto>
            {
                Items = pateintsDtos,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }

        /// <summary>
        /// Retrieves patient details by ID
        /// </summary>
        /// <param name="id">ID of the patient</param>
        /// <returns>Patient details or null if not found</returns>
        public async Task<PatientResponseDto?> GetPatientByIdAsync(int id)
        {
            Log.Debug("Fetching patient details for ID: {PatientId}", id);

            var patient = await _PatientManagementRespository.GetPatientByIdReadOnlyAsync(id);

            if (patient?.User == null)
            {
                Log.Warning("Patient not found for ID: {PatientId}", id);
                return null;
            }

           
Log.Debug("Successfully retrieved patient details for ID: {PatientId}", id);
            return new PatientResponseDto
            {
                Username = patient.User.Username,
                Email = patient.User.Email,
                DateOfBirth = patient.DateOfBirth,
                InsuranceNumber = patient.InsuranceNumber,
                IsActive = patient.User.IsActive
            };
        }

        /// <summary>
        /// Updates patient information
        /// </summary>
        /// <param name="id">ID of the patient to update</param>
        /// <param name="updatePatientRequest">Updated patient data</param>
        /// <returns>Response indicating success or failure</returns>
        public async Task<UpdatePatientResponseDto?> UpdatePatientAsync(int id, UpdatePatientRequestDto updatePatientRequest)
        {
            Log.Information("Starting update for patient ID: {PatientId}", id);

            // Check if email exists for other patients
            if (await _PatientManagementRespository.IsEmailExistsIgnoringCurrentPatientAsync(updatePatientRequest.Email, id))
            {
                Log.Warning("Email already exists for another patient: {Email}", updatePatientRequest.Email);
                return new UpdatePatientResponseDto
                {
                    Message = "Email is already registered!",
                    IsSuccess = false
                };
            }

            // Check if username exists for other patients
            if (await _PatientManagementRespository.IsUsernameExistsIgnoringCurrentPatientAsync(updatePatientRequest.Username, id))
            {
                Log.Warning("Username already exists for another patient: {Username}", updatePatientRequest.Username);
                return new UpdatePatientResponseDto
                {
                    Message = "Username is already taken!",
                    IsSuccess = false
                };
            }

            // Check if insurance number exists for other patients
            if (!string.IsNullOrWhiteSpace(updatePatientRequest.InsuranceNumber) &&
                await _PatientManagementRespository.IsInsuranceNumberExistsIgnoringCurrentPatientAsync(
                    updatePatientRequest.InsuranceNumber, id))
            {
                Log.Warning("Insurance number already exists for another patient: {InsuranceNumber}",
                    updatePatientRequest.InsuranceNumber);
                return new UpdatePatientResponseDto
                {
                    Message = "InsuranceNumber is already taken!",
                    IsSuccess = false
                };
            }

            // Retrieve patient for update
            var patient = await _PatientManagementRespository.GetPatientByIdForUpdateAsync(id);
            if (patient == null)
            {
                Log.Warning("Patient not found for update: {PatientId}", id);
                return null;
            }

            // Update patient properties
            patient.User.Username = updatePatientRequest.Username;
            patient.User.Email = updatePatientRequest.Email;
            patient.DateOfBirth = updatePatientRequest.DateOfBirth;
            patient.InsuranceNumber = string.IsNullOrEmpty(updatePatientRequest.InsuranceNumber) ?
                null : updatePatientRequest.InsuranceNumber;

            Log.Debug("Attempting to update patient in repository");
            var isSuccess = await _PatientManagementRespository.UpdatePatientAsync(patient);

            if (isSuccess)
            {
                Log.Information("Patient updated successfully: {PatientId}", id);
                return new UpdatePatientResponseDto
                {
                    IsSuccess = true,
                    Message = "patient data has been updated successfully!",
                    Email = patient.User.Email,
                    Username = patient.User.Username,
                    DateOfBirth = patient.DateOfBirth,
                    InsuranceNumber = patient.InsuranceNumber



                };
            }
            else
            {
                Log.Error("Failed to update patient: {PatientId}", id);
                return new UpdatePatientResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while updating!"
                };
            }
        }
    }
}