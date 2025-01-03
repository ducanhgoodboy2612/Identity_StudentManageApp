using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.DTOs
{
    public class LecturerDTO
    {
        public int LecturerID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? YoB { get; set; }
    }
}
