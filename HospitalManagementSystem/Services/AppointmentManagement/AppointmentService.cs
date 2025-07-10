
using HospitalManagementSystem.DTOs.Requests;
using HospitalManagementSystem.DTOs.Responses;
using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Repositories.Interfaces.AppointmentManagement;
using HospitalManagementSystem.Repositories.Interfaces.DoctorManagemment;
using HospitalManagementSystem.Services.Interfaces.AppointmentManagement;
using HospitalManagementSystem.Enums;
using HospitalManagementSystem.Repositories.Interfaces.PatientManagement;
using HospitalManagementSystem.Repositories.PatientManagement;
using HospitalManagementSystem.Repositories.DoctorManagemment;
using Microsoft.Identity.Client;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using Serilog;

namespace HospitalManagementSystem.Services.AppointmentManagement
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorManagemmentRespository _doctorManagemmentRespository;
        private readonly IPatientManagementRespository _patientManagementRespository;
        private readonly IUserManagementRespository _userManagementRespository;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorManagemmentRespository doctorManagemmentRespository,
            IPatientManagementRespository patientManagementRespository,
            IUserManagementRespository userManagementRespository)
        {
            _appointmentRepository = appointmentRepository;
            _doctorManagemmentRespository = doctorManagemmentRespository;
            _patientManagementRespository = patientManagementRespository;
            _userManagementRespository = userManagementRespository;
        }

        /// <summary>
        /// Cancels an existing appointment
        /// </summary>
        /// <param name="appointmentId">ID of the appointment to cancel</param>
        /// <param name="cancelRequestDto">Cancellation reason</param>
        /// <returns>Response indicating success or failure</returns>
        public async Task<MessageResponseDto> CancelAppointment(int appointmentId, string cancelRequestDto)
        {
            Log.Debug("Attempting to cancel appointment with ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetAppoitmentByIdForUpdateAsync(appointmentId);

            if (appointment == null)
            {
                Log.Warning("Appointment not found with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Appointment not found"
                };
            }

            if (appointment.Status == AppointmentStatus.completed.ToString())
            {
                Log.Warning("Attempt to cancel already completed appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Cannot cancel already completed appointment"
                };
            }

            if (appointment.Status == AppointmentStatus.cancelled.ToString())
            {
                Log.Warning("Attempt to cancel already cancelled appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Appointment is already cancelled"
                };
            }

            if (string.IsNullOrWhiteSpace(cancelRequestDto))
            {
                Log.Warning("Cancellation reason not provided for appointment ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Cancellation reason is required"
                };
            }

           
            appointment.Status = AppointmentStatus.cancelled.ToString();
            appointment.Notes = $"Cancellation Reason: {cancelRequestDto}";

            var isUpdateStatus = await _appointmentRepository.UpdateAsync(appointment);

            if (!isUpdateStatus)
            {
                Log.Error("Failed to update status for appointment ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to cancel appointment"
                };
            }

            Log.Information("Successfully cancelled appointment with ID: {AppointmentId}", appointmentId);
            return new MessageResponseDto
            {
                IsSuccess = true,
                Message = "Appointment cancelled successfully"
            };
        }

        /// <summary>
        /// Marks an appointment as completed
        /// </summary>
        /// <param name="appointmentId">ID of the appointment to complete</param>
        /// <returns>Response indicating success or failure</returns>
        public async Task<MessageResponseDto> CompleteAppointment(int appointmentId)
        {
            Log.Debug("Attempting to complete appointment with ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetAppoitmentByIdForUpdateAsync(appointmentId);

            if (appointment == null)
            {
                Log.Warning("Appointment not found with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Appointment not found"
                };
            }

            if (appointment.Status == AppointmentStatus.pending.ToString())
            {
                Log.Warning("Attempt to complete pending appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Cannot complete pending appointment"
                };
            }

            if (appointment.Status == AppointmentStatus.cancelled.ToString())
            {
                Log.Warning("Attempt to complete cancelled appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Cannot complete cancelled appointment"
                };
            }

            if (appointment.Status == AppointmentStatus.completed.ToString())
            {
                Log.Warning("Attempt to complete already completed appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Appointment is already completed"
                };
            }

            appointment.Status = AppointmentStatus.completed.ToString();
            var isUpdateStatus = await _appointmentRepository.UpdateAsync(appointment);

            if (!isUpdateStatus)
            {
                Log.Error("Failed to update status for appointment ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to complete appointment"
                };
            }

            Log.Information("Successfully completed appointment with ID: {AppointmentId}", appointmentId);
            return new MessageResponseDto
            {
                IsSuccess = true,
                Message = "Appointment completed successfully"
            };
        }

        /// <summary>

       
        /// Confirms a pending appointment
        /// </summary>
        /// <param name="appointmentId">ID of the appointment to confirm</param>
        /// <returns>Response indicating success or failure</returns>
        public async Task<MessageResponseDto> ConfirmAppointment(int appointmentId)
        {
            Log.Debug("Attempting to confirm appointment with ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetAppoitmentByIdForUpdateAsync(appointmentId);
            if (appointment == null)
            {
                Log.Warning("Appointment not found with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Appointment not found"
                };
            }

            if (appointment.Status == AppointmentStatus.completed.ToString())
            {
                Log.Warning("Attempt to confirm already completed appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Cannot confirm already completed appointment"
                };
            }

            if (appointment.Status == AppointmentStatus.cancelled.ToString())
            {
                Log.Warning("Attempt to confirm cancelled appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Cannot confirm cancelled appointment"
                };
            }

            if (appointment.Status == AppointmentStatus.confirmed.ToString())
            {
                Log.Warning("Attempt to confirm already confirmed appointment with ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Appointment is already confirmed"
                };
            }

            appointment.Status = AppointmentStatus.confirmed.ToString();
            var isUpdateStatus = await _appointmentRepository.UpdateAsync(appointment);

            if (!isUpdateStatus)
            {
                Log.Error("Failed to update status for appointment ID: {AppointmentId}", appointmentId);
                return new MessageResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to update appointment status"
                };
            }

            Log.Information("Successfully confirmed appointment with ID: {AppointmentId}", appointmentId);
            return new MessageResponseDto
            {
                IsSuccess = true,
                Message = "Appointment confirmed successfully"
            };
        }

        /// <summary>
        /// Creates a new appointment by admin
        /// </summary>
        /// <param name="dto">Appointment creation data</param>
        /// <returns>Response with appointment details or error</returns>
        public async Task<AppointmentResponseDto> CreateAppointmentByAdminAsync(AppointmentAdminRequestDto dto)
        {
            Log.Debug("Attempting to create new appointment by admin for doctor ID: {DoctorId} and patient ID: {PatientId}",
                dto.DoctorId, dto.PatientId);

            // Check doctor availability (corrected logic)
            if (!await _appointmentRepository.IsDoctorAvailableAsync(dto.DoctorId, dto.DateTime))
            {
                Log.Warning("Doctor with ID: {DoctorId} is not available at requested time: {DateTime}",
                    dto.DoctorId, dto.DateTime);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Doctor is not available at the requested time."
                };
            }
            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                DateTime = dto.DateTime,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes,
            };




            int appointmentId = await _appointmentRepository.AddAsync(appointment);

            if (appointmentId == -1)
            {
                Log.Error("Failed to create appointment in database for doctor ID: {DoctorId} and patient ID: {PatientId}",
                    dto.DoctorId, dto.PatientId);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to create appointment due to a database error."
                };
            }

            var doctor = await _doctorManagemmentRespository.GetDoctorByIdReadOnlyAsync(dto.DoctorId);
            var patient = await _patientManagementRespository.GetPatientByIdReadOnlyAsync(dto.PatientId);

            Log.Information("Successfully created appointment with ID: {AppointmentId} by admin", appointmentId);
            return new AppointmentResponseDto
            {
                IsSuccess = true,
                AppointmentId = appointmentId,
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                DateTime = dto.DateTime,
                Status = AppointmentStatus.pending.ToString(),
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? "no Notes" : dto.Notes,
                DoctorName = doctor.User.Username,
                PatientName = patient.User.Username,
                Message = "Appointment created successfully."
            };
        }

        /// <summary>
        /// Creates a new appointment by doctor
        /// </summary>
        /// <param name="dto">Appointment creation data</param>
        /// <param name="Userid">ID of the doctor user</param>
        /// <param name="doctorusername">Username of the doctor</param>
        /// <returns>Response with appointment details or error</returns>
        public async Task<AppointmentResponseDto> CreateAppointmentByDoctorAsync(AppointmentDoctorRequestDto dto, int Userid, string doctorusername)
        {
            Log.Debug("Attempting to create new appointment by doctor with user ID: {UserId}", Userid);

            var doctorid = await _userManagementRespository.GetDoctorIdByUseridAsync(Userid);
            if (doctorid == -1)
            {
                Log.Warning("Doctor not found for user ID: {UserId}", Userid);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to create appointment due to a database error."
                };
            }

            if (!await _appointmentRepository.IsDoctorAvailableAsync(doctorid, dto.DateTime))
            {
                Log.Warning("Doctor with ID: {DoctorId} is not available at requested time: {DateTime}",
                    doctorid, dto.DateTime);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "You are not available at the requested time."
                };
            }

            var appointment = new Appointment
            {
                DoctorId = doctorid,
                PatientId = dto.PatientId,
                DateTime = dto.DateTime,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes,
                Status = AppointmentStatus.confirmed.ToString(),
            };

            int appointmentId = await _appointmentRepository.AddAsync(appointment);

            if (appointmentId == -1)
            {
                Log.Error("Failed to create appointment in database for doctor ID: {DoctorId}", doctorid);

               
            return new AppointmentResponseDto
            {
                IsSuccess = false,
                Message = "Failed to create appointment due to a database error."

            };
            }

            var patient = await _patientManagementRespository.GetPatientByIdReadOnlyAsync(dto.PatientId);

            Log.Information("Successfully created appointment with ID: {AppointmentId} by doctor", appointmentId);
            return new AppointmentResponseDto
            {
                IsSuccess = true,
                AppointmentId = appointmentId,
                DoctorId = doctorid,
                PatientId = dto.PatientId,
                DateTime = dto.DateTime,
                Status = AppointmentStatus.confirmed.ToString(),
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? "no Notes" : dto.Notes,
                DoctorName = doctorusername,
                PatientName = patient.User.Username,
                Message = "Appointment created successfully."
            };
        }

        /// <summary>
        /// Creates a new appointment by patient
        /// </summary>
        /// <param name="dto">Appointment creation data</param>
        /// <param name="userid">ID of the patient user</param>
        /// <param name="Patientusername">Username of the patient</param>
        /// <returns>Response with appointment details or error</returns>
        public async Task<AppointmentResponseDto> CreateAppointmentByPatientAsync(AppointmentPatientRequestDto dto, int userid, string Patientusername)
        {
            Log.Debug("Attempting to create new appointment by patient with user ID: {UserId}", userid);

            var patientid = await _userManagementRespository.GetPatientIdByUseridAsync(userid);
            if (patientid == -1)
            {
                Log.Warning("Patient not found for user ID: {UserId}", userid);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to create appointment due to a database error."
                };
            }

            if (!await _appointmentRepository.IsDoctorAvailableAsync(dto.Doctorid, dto.DateTime))
            {
                Log.Warning("Doctor with ID: {DoctorId} is not available at requested time: {DateTime}",
                    dto.Doctorid, dto.DateTime);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "You are not available at the requested time."
                };
            }

            var appointment = new Appointment
            {
                DoctorId = dto.Doctorid,
                PatientId = patientid,
                DateTime = dto.DateTime,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes,
                Status = AppointmentStatus.pending.ToString(),
            };

            int appointmentId = await _appointmentRepository.AddAsync(appointment);

            if (appointmentId == -1)
            {
                Log.Error("Failed to create appointment in database for patient ID: {PatientId}", patientid);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to create appointment due to a database error."
                };
            }

            var doctor = await _doctorManagemmentRespository.GetDoctorByIdReadOnlyAsync(dto.Doctorid);

            Log.Information("Successfully created appointment with ID: {AppointmentId} by patient", appointmentId);
            return new AppointmentResponseDto
            {
                IsSuccess = true,
                AppointmentId = appointmentId,

               
                DoctorId = dto.Doctorid,
                PatientId = patientid,
                DateTime = dto.DateTime,
                Status = AppointmentStatus.pending.ToString(),
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? "no Notes" : dto.Notes,
                DoctorName = doctor.User.Username,
                PatientName = Patientusername,
                Message = "Appointment created successfully."
            };
        }

        /// <summary>
        /// Deletes an appointment
        /// </summary>
        /// <param name="id">ID of the appointment to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            Log.Debug("Attempting to delete appointment with ID: {AppointmentId}", id);

            var result = await _appointmentRepository.DeleteAsync(id);
            if (result)
            {
                Log.Information("Successfully deleted appointment with ID: {AppointmentId}", id);
            }
            else
            {
                Log.Warning("Failed to delete appointment with ID: {AppointmentId}", id);
            }
            return result;
        }

        /// <summary>
        /// Gets an appointment by its ID
        /// </summary>
        /// <param name="id">ID of the appointment to retrieve</param>
        /// <returns>Appointment details or error response</returns>
        public async Task<AppointmentResponseDto> GetAppointmentById(int id)
        {
            Log.Debug("Attempting to get appointment with ID: {AppointmentId}", id);

            var appointment = await _appointmentRepository.GetAppoitmentByIdforReadAsync(id);
            if (appointment == null)
            {
                Log.Warning("Appointment not found with ID: {AppointmentId}", id);
                return new AppointmentResponseDto { IsSuccess = false, Message = "Appointment Not Found" };
            }

            Log.Debug("Successfully retrieved appointment with ID: {AppointmentId}", id);
            return new AppointmentResponseDto
            {
                IsSuccess = true,
                AppointmentId = appointment.AppointmentId,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                DateTime = appointment.DateTime,
                Notes = string.IsNullOrEmpty(appointment.Notes) ? "No Notes" : appointment.Notes,
                DoctorName = appointment.Doctor.User.Username,
                PatientName = appointment.Patient.User.Username,
                Message = "Get Appointment successfully."
            };
        }

        /// <summary>
        /// Gets all appointments for a specific doctor
        /// </summary>
        /// <param name="userid">ID of the doctor user</param>
        /// <returns>List of appointments or null if not found</returns>
        public async Task<IEnumerable<AppointmentResponseDto?>> GetAppointmentDoctorByIdAsync(int userid)
        {
            Log.Debug("Attempting to get appointments for doctor with user ID: {UserId}", userid);

            var doctorid = await _userManagementRespository.GetDoctorIdByUseridAsync(userid);
            if (doctorid == -1)
            {
                Log.Warning("Doctor not found for user ID: {UserId}", userid);
                return null;
            }

            var doctorAppointment = await _appointmentRepository.GetAppointmentDoctorByIdAsync(doctorid);
            if (doctorAppointment == null)
            {
                Log.Warning("No appointments found for doctor ID: {DoctorId}", doctorid);
                return null;
            }

            Log.Debug("Successfully retrieved {AppointmentCount} appointments for doctor ID: {DoctorId}",
                doctorAppointment.Count(), doctorid);

           
            return doctorAppointment.Select(d => new AppointmentResponseDto
           {
                AppointmentId = d.AppointmentId,
                DoctorId = d.DoctorId,
                PatientId = d.PatientId,
                DateTime = d.DateTime,
                Status = d.Status,
                Notes = string.IsNullOrEmpty(d.Notes) ? "no Notes" : d.Notes,
                DoctorName = d.Doctor.User.Username,
                PatientName = d.Patient.User.Username,

            });
        }

        /// <summary>
        /// Gets all appointments for a specific patient
        /// </summary>
        /// <param name="userid">ID of the patient user</param>
        /// <returns>List of appointments or null if not found</returns>
        public async Task<IEnumerable<AppointmentResponseDto?>> GetAppointmentPatientByIdAsync(int userid)
        {
            Log.Debug("Attempting to get appointments for patient with user ID: {UserId}", userid);

            var patientid = await _userManagementRespository.GetPatientIdByUseridAsync(userid);
            if (patientid == -1)
            {
                Log.Warning("Patient not found for user ID: {UserId}", userid);
                return null;
            }

            var PatientAppointment = await _appointmentRepository.GetAppointmentPatientByIdAsync(patientid);
            if (PatientAppointment == null)
            {
                Log.Warning("No appointments found for patient ID: {PatientId}", patientid);
                return null;
            }

            Log.Debug("Successfully retrieved {AppointmentCount} appointments for patient ID: {PatientId}",
                PatientAppointment.Count(), patientid);
            return PatientAppointment.Select(d => new AppointmentResponseDto
            {
                AppointmentId = d.AppointmentId,
                DoctorId = d.DoctorId,
                PatientId = d.PatientId,
                DateTime = d.DateTime,
                Status = d.Status,
                Notes = string.IsNullOrEmpty(d.Notes) ? "no Notes" : d.Notes,
                DoctorName = d.Doctor.User.Username,
                PatientName = d.Patient.User.Username,
            });
        }

        /// <summary>
        /// Gets all available doctors
        /// </summary>
        /// <returns>List of available doctors</returns>
        public async Task<IEnumerable<DoctorAvailableResponseDto>> GetAvailableDoctorsAsync()
        {
            Log.Debug("Attempting to get all available doctors");

            var doctors = await _appointmentRepository.GetAvailableDoctorsAsync();
            Log.Debug("Successfully retrieved {AppointmentCount} available doctors", doctors.Count());
            return doctors.Select(d => new DoctorAvailableResponseDto
            {
                DoctorId = d.DoctorId,
                username = d.User.Username,
                Specialization = d.Specialization
            });
        }

        /// <summary>
        /// Gets all available patients
        /// </summary>
        /// <returns>List of available patients</returns>
        public async Task<IEnumerable<patientAvailableResponseDto>> GetAvailablePatientAsync()
        {
            Log.Debug("Attempting to get all available patients");

            var patient = await _appointmentRepository.GetAvailablePatientsAsync();
            Log.Debug("Successfully retrieved {AppointmentCount} available patients", patient.Count());
            return patient.Select(p => new patientAvailableResponseDto
            {
                PatientId = p.PatientId,
                username = p.User.Username,
                InsuranceNumber = p.InsuranceNumber,
            });
        }

        /// <summary>
        /// Gets appointments with pagination
        /// </summary>
        /// <param name="dto">Pagination parameters</param>

       
        /// <returns>Paged response with appointments</returns>
        public async Task<PagedResponseDto<AppointmentResponseDto>> GetPagedAppointmentsAsync(PaginationRequestDto dto)
        {
            Log.Debug("Attempting to get paged appointments (Page: {PageNumber}, Size: {PageSize})",
                dto.PageNumber, dto.PageSize);

            var (Appointment, totalCount) = await _appointmentRepository.GetPagedAppointmentAsync(dto.PageNumber, dto.PageSize);

            var totalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize);

            var AppointemtnDtos = Appointment.Select(d => new AppointmentResponseDto
            {
                AppointmentId = d.AppointmentId,
                DoctorId = d.DoctorId,
                PatientId = d.PatientId,
                Status = d.Status,
                Notes = string.IsNullOrEmpty(d.Notes) ? "No notes" : d.Notes,
                DoctorName = d.Doctor.User.Username,
                PatientName = d.Patient.User.Username
            }).ToList();

            Log.Debug("Successfully retrieved {AppointmentCount} appointments (Page: {PageNumber}, Total: {TotalCount})",
                AppointemtnDtos.Count, dto.PageNumber, totalCount);

            return new PagedResponseDto<AppointmentResponseDto>
            {
                Items = AppointemtnDtos,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }

        /// <summary>
        /// Updates an appointment by admin
        /// </summary>
        /// <param name="id">ID of the appointment to update</param>
        /// <param name="dto">Updated appointment data</param>
        /// <returns>Updated appointment details or error</returns>
        public async Task<AppointmentResponseDto?> UpdateAppointmentByAdminAsync(int id, AppointmentAdminRequestDto dto)
        {
            Log.Debug("Attempting to update appointment with ID: {AppointmentId} by admin", id);

            if (!await _appointmentRepository.IsDoctorAvailableAsync(dto.DoctorId, dto.DateTime))
            {
                Log.Warning("Doctor with ID: {DoctorId} is not available at requested time: {DateTime}",
                    dto.DoctorId, dto.DateTime);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Doctor is not available at the requested time."
                };
            }

            var appointment = await _appointmentRepository.GetAppoitmentByIdForUpdateAsync(id);
            if (appointment == null)
            {
                Log.Warning("Appointment not found with ID: {AppointmentId}", id);
                return null;
            }

            appointment.DoctorId = dto.DoctorId;
            appointment.PatientId = dto.PatientId;
            appointment.DateTime = dto.DateTime;
            appointment.Notes = string.IsNullOrEmpty(dto.Notes) ? null : dto.Notes;

            var isupdate = await _appointmentRepository.UpdateAsync(appointment);
            if (!isupdate)
            {
                Log.Error("Failed to update appointment with ID: {AppointmentId}", id);
                return new AppointmentResponseDto { IsSuccess = false, Message = "Failed Update." };
            }

            Log.Information("Successfully updated appointment with ID: {AppointmentId} by admin", id);
            return new AppointmentResponseDto
            {
                IsSuccess = true,
                Message = "Appointment Update successfully.",
                AppointmentId = appointment.AppointmentId,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                DateTime = appointment.DateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                DoctorName = appointment.Doctor.User.Username,
                PatientName = appointment.Patient.User.Username
            };
        }

      
        /// <summary>
        /// Updates an appointment by doctor
        /// </summary>
        /// <param name="Appointemntid">ID of the appointment to update</param>
        /// <param name="dto">Updated appointment data</param>
        /// <param name="userid">ID of the doctor user</param>
        /// <returns>Updated appointment details or error</returns>
        public async Task<AppointmentResponseDto?> UpdateAppointmentByDoctorAsync(int Appointemntid, AppointmentDoctorRequestDto dto, int userid)
        {
            Log.Debug("Attempting to update appointment with ID: {AppointmentId} by doctor with user ID: {UserId}",
                Appointemntid, userid);

            var doctorid = await _userManagementRespository.GetDoctorIdByUseridAsync(userid);
            if (doctorid == -1)
            {
                Log.Warning("Doctor not found for user ID: {UserId}", userid);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to create appointment due to a database error."
                };
            }

            if (!await _appointmentRepository.IsDoctorAvailableAsync(doctorid, dto.DateTime))
            {
                Log.Warning("Doctor with ID: {DoctorId} is not available at requested time: {DateTime}",
                    doctorid, dto.DateTime);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "You are not available at the requested time."
                };
            }

            var appointment = await _appointmentRepository.GetAppoitmentByIdForUpdateAsync(Appointemntid);
            if (appointment == null)
            {
                Log.Warning("Appointment not found with ID: {AppointmentId}", Appointemntid);
                return null;
            }

            appointment.PatientId = dto.PatientId;
            appointment.DateTime = dto.DateTime;
            appointment.Notes = string.IsNullOrEmpty(dto.Notes) ? null : dto.Notes;

            var isupdate = await _appointmentRepository.UpdateAsync(appointment);
            if (!isupdate)
            {
                Log.Error("Failed to update appointment with ID: {AppointmentId}", Appointemntid);
                return new AppointmentResponseDto { IsSuccess = false, Message = "Failed Update." };
            }

            Log.Information("Successfully updated appointment with ID: {AppointmentId} by doctor", Appointemntid);
            return new AppointmentResponseDto
            {
                IsSuccess = true,
                Message = "Appointment Update successfully.",
                AppointmentId = appointment.AppointmentId,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                DateTime = appointment.DateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                DoctorName = appointment.Doctor.User.Username,
                PatientName = appointment.Patient.User.Username
            };
        }

        /// <summary>
        /// Updates an appointment by patient
        /// </summary>
        /// <param name="Appointemntid">ID of the appointment to update</param>
        /// <param name="dto">Updated appointment data</param>
        /// <param name="userid">ID of the patient user</param>
        /// <returns>Updated appointment details or error</returns>
        public async Task<AppointmentResponseDto?> UpdateAppointmentByPatientAsync(int Appointemntid, AppointmentPatientRequestDto dto, int userid)
        {
            Log.Debug("Attempting to update appointment with ID: {AppointmentId} by patient with user ID: {UserId}",
                Appointemntid, userid);

           
              var patientid = await _userManagementRespository.GetPatientIdByUseridAsync(userid);
            if (patientid == -1)
            {
                Log.Warning("Patient not found for user ID: {UserId}", userid);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to create appointment due to a database error."
                };
            }

            if (!await _appointmentRepository.IsDoctorAvailableAsync(dto.Doctorid, dto.DateTime))
            {
                Log.Warning("Doctor with ID: {DoctorId} is not available at requested time: {DateTime}",
                    dto.Doctorid, dto.DateTime);
                return new AppointmentResponseDto
                {
                    IsSuccess = false,
                    Message = "You are not available at the requested time."
                };
            }

            var appointment = await _appointmentRepository.GetAppoitmentByIdForUpdateAsync(Appointemntid);
            if (appointment == null)
            {
                Log.Warning("Appointment not found with ID: {AppointmentId}", Appointemntid);
                return null;
            }

            appointment.DoctorId = dto.Doctorid;
            appointment.DateTime = dto.DateTime;
            appointment.Notes = string.IsNullOrEmpty(dto.Notes) ? null : dto.Notes;

            var isupdate = await _appointmentRepository.UpdateAsync(appointment);
            if (!isupdate)
            {
                Log.Error("Failed to update appointment with ID: {AppointmentId}", Appointemntid);
                return new AppointmentResponseDto { IsSuccess = false, Message = "Failed Update." };
            }

            Log.Information("Successfully updated appointment with ID: {AppointmentId} by patient", Appointemntid);
            return new AppointmentResponseDto
            {
                IsSuccess = true,
                Message = "Appointment Update successfully.",
                AppointmentId = appointment.AppointmentId,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                DateTime = appointment.DateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                DoctorName = appointment.Doctor.User.Username,
                PatientName = appointment.Patient.User.Username
            };
        }
    }
}