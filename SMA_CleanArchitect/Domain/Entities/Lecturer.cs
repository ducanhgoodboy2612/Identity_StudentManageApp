using System.Security.Claims;

namespace Domain.Entities
{
    public class Lecturer
    {
        public int LecturerID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? YoB { get; set; }

        // Quan hệ 1-n với Class
        public ICollection<Class> Classes { get; set; }
    }

}
