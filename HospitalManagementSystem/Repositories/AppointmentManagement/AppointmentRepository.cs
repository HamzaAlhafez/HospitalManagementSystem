using HospitalManagementSystem.Models.Entities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Repositories.Interfaces.AppointmentManagement;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.Enums;
using Serilog;







namespace HospitalManagementSystem.Repositories.AppointmentManagement
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new appointment to the system
        /// </summary>
        /// <param name="appointment">Appointment to add</param>
        /// <returns>ID of the created appointment or -1 if failed</returns>
        public async Task<int> AddAsync(Appointment appointment)
        {
            Log.Information("Adding new appointment for patient ID: {PatientId} with doctor ID: {DoctorId}",
                appointment.PatientId, appointment.DoctorId);

            try
            {
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();

                Log.Debug("Successfully added appointment with ID: {AppointmentId}", appointment.AppointmentId);
                return appointment.AppointmentId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add appointment for patient ID: {PatientId}", appointment.PatientId);
                return -1;
            }
        }

        /// <summary>
        /// Deletes an appointment from the system
        /// </summary>
        /// <param name="id">ID of the appointment to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            Log.Information("Deleting appointment with ID: {AppointmentId}", id);

            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment != null)
                {
                    _context.Appointments.Remove(appointment);
                    await _context.SaveChangesAsync();

                    Log.Debug("Successfully deleted appointment with ID: {AppointmentId}", id);
                    return true;
                }

                Log.Warning("Appointment not found with ID: {AppointmentId}", id);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting appointment with ID: {AppointmentId}", id);
                return false;
            }
        }

        /// <summary>
        /// Checks if an appointment exists
        /// </summary>
        /// <param name="id">ID of the appointment to check</param>
        /// <returns>True if appointment exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            Log.Debug("Checking existence of appointment with ID: {AppointmentId}", id);

            try
            {
                return await _context.Appointments.AnyAsync(x => x.AppointmentId == id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking appointment existence with ID: {AppointmentId}", id);
                return false;
            }
        }

        /// <summary>
        /// Retrieves a paged list of appointments
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing appointments list and total count</returns>
        public async Task<(IEnumerable<Appointment>, int TotalCount)> GetPagedAppointmentAsync(int pageNumber, int pageSize)
        {
            Log.Information("Retrieving paged appointments. Page: {PageNumber}, Size: {PageSize}",
                pageNumber, pageSize);

            
            try
            {
                var baseQuery = _context.Appointments
                    .AsNoTracking()
                    .AsQueryable();

                var totalCount = await baseQuery.CountAsync();

                var appointmentsData = await baseQuery
                    .OrderBy(a => a.DateTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new
                    {
                        Appointment = a,
                        Doctor = a.Doctor,
                        Patient = a.Patient,
                        DoctorUsername = a.Doctor.User.Username,
                        PatientUsername = a.Patient.User.Username
                    })
                    .ToListAsync();

                var appointments = appointmentsData.Select(x =>
                {
                    var appointment = x.Appointment;
                    appointment.Doctor = x.Doctor;
                    appointment.Doctor.User = new User { Username = x.DoctorUsername };
                    appointment.Patient = x.Patient;
                    appointment.Patient.User = new User { Username = x.PatientUsername };
                    return appointment;
                }).ToList();

                Log.Debug("Retrieved {AppointmentCount} appointments out of {TotalCount}",
                    appointments.Count, totalCount);

                return (appointments, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving paged appointments");
                return (new List<Appointment>(), 0);
            }
        }

        /// <summary>
        /// Retrieves an appointment by ID for update purposes
        /// </summary>
        /// <param name="id">ID of the appointment</param>
        /// <returns>Appointment entity or null if not found</returns>
        public async Task<Appointment?> GetAppoitmentByIdForUpdateAsync(int id)
        {
            Log.Debug("Retrieving appointment for update with ID: {AppointmentId}", id);

            try
            {
                var appointment = await _context.Appointments
                    .Include(d => d.Doctor)
                    .Include(p => p.Patient)
                    .FirstOrDefaultAsync(a => a.AppointmentId == id);

                if (appointment == null)
                {
                    Log.Warning("Appointment not found for update with ID: {AppointmentId}", id);
                }

                return appointment;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving appointment for update with ID: {AppointmentId}", id);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all available doctors
        /// </summary>
        /// <returns>List of available doctors</returns>
        public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync()
        {
            Log.Information("Retrieving available doctors");

            try
            {
                return await _context.Doctors
                    .Include(d => d.User)
                    .Where(d => d.User.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving available doctors");
                return new List<Doctor>();
            }
        }

        /// <summary>
        /// Retrieves all available patients
        /// </summary>
        /// <returns>List of available patients</returns>
        public async Task<IEnumerable<Patient>> GetAvailablePatientsAsync()
        {
            Log.Information("Retrieving available patients");

           
           try
            {
                return await _context.Patients
                    .Include(d => d.User)
                    .Where(d => d.User.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving available patients");
                return new List<Patient>();
            }
        }

        /// <summary>
        /// Retrieves appointments by doctor ID
        /// </summary>
        /// <param name="doctorId">ID of the doctor</param>
        /// <returns>List of appointments for the specified doctor</returns>
        public async Task<IEnumerable<Appointment?>> GetAppointmentDoctorByIdAsync(int doctorId)
        {
            Log.Information("Retrieving appointments for doctor ID: {DoctorId}", doctorId);

            try
            {
                var appointments = await _context.Appointments
                    .Include(d => d.Doctor)
                    .ThenInclude(u => u.User)
                    .Include(p => p.Patient)
                    .ThenInclude(u => u.User)
                    .Where(x => x.DoctorId == doctorId)
                    .AsNoTracking()
                    .ToListAsync();

                Log.Debug("Found {AppointmentCount} appointments for doctor ID: {DoctorId}",
                    appointments.Count, doctorId);

                return appointments;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving appointments for doctor ID: {DoctorId}", doctorId);
                return new List<Appointment>();
            }
        }

        /// <summary>
        /// Retrieves an appointment by ID for read-only purposes
        /// </summary>
        /// <param name="id">ID of the appointment</param>
        /// <returns>Appointment entity or null if not found</returns>
        public async Task<Appointment?> GetAppoitmentByIdforReadAsync(int id)
        {
            Log.Debug("Retrieving appointment for read with ID: {AppointmentId}", id);

            try
            {
                var appointment = await _context.Appointments
                    .Include(d => d.Doctor)
                    .ThenInclude(u => u.User)
                    .Include(p => p.Patient)
                    .ThenInclude(u => u.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.AppointmentId == id);

                if (appointment == null)
                {
                    Log.Warning("Appointment not found for read with ID: {AppointmentId}", id);
                }

                return appointment;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving appointment for read with ID: {AppointmentId}", id);
                return null;
            }
        }

        /// <summary>
        /// Retrieves appointments by patient ID
        /// </summary>
        /// <param name="patientId">ID of the patient</param>
        /// <returns>List of appointments for the specified patient</returns>
        public async Task<IEnumerable<Appointment>> GetAppointmentPatientByIdAsync(int patientId)
        {
            Log.Information("Retrieving appointments for patient ID: {PatientId}", patientId);

            try
            {
                var appointments = await _context.Appointments
                    .Include(d => d.Doctor)
                    .ThenInclude(u => u.User)
                    .Include(p => p.Patient)
                    .ThenInclude(u => u.User)
                    .AsNoTracking()
                    .Where(x => x.PatientId == patientId)
                    .ToListAsync();

                Log.Debug("Found {AppointmentCount} appointments for patient ID: {PatientId}",
                    appointments.Count, patientId);

                
return appointments;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving appointments for patient ID: {PatientId}", patientId);
                return new List<Appointment>();
            }
        }

        /// <summary>
        /// Checks if a doctor is available at a specific date/time
        /// </summary>
        /// <param name="doctorId">ID of the doctor</param>
        /// <param name="dateTime">Date and time to check</param>
        /// <returns>True if doctor is available, false otherwise</returns>
        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime dateTime)
        {
            Log.Debug("Checking doctor availability for doctor ID: {DoctorId} at {DateTime}",
                doctorId, dateTime);

            try
            {
                return !await _context.Appointments.AnyAsync(x =>
                    x.DoctorId == doctorId &&
                    x.DateTime == dateTime &&
                    x.Status == AppointmentStatus.confirmed.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking doctor availability for doctor ID: {DoctorId}", doctorId);
                return false;
            }
        }

        /// <summary>
        /// Updates an existing appointment
        /// </summary>
        /// <param name="appointment">Appointment with updated data</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateAsync(Appointment appointment)
        {
            Log.Information("Updating appointment with ID: {AppointmentId}", appointment.AppointmentId);

            try
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();

                Log.Debug("Successfully updated appointment with ID: {AppointmentId}", appointment.AppointmentId);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating appointment with ID: {AppointmentId}", appointment.AppointmentId);
                return false;
            }
        }
    }
}