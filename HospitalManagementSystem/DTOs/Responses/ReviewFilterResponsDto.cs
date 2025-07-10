namespace HospitalManagementSystem.DTOs.Responses
{
    public class ReviewFilterResponsDto
    {
        public int Id { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
       

        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }

    }
}
