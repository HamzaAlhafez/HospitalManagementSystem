namespace HospitalManagementSystem.DTOs.Responses
{
    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }



        public bool IsSuccess { get; set; }
       public string Message { get; set; }


    }
}
