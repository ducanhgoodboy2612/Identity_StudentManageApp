using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement_Domain.Interface
{
    public partial interface ITuitionFeeRepository
    {
        Task<IEnumerable<TuitionFee>> GetTuitionFeesByStudentIdAsync(int studentId);
        Task<TuitionFee> CreateTuitionFeeAsync(TuitionFeeDTO tuitionFeeDTO);
        Task<TuitionFee> UpdateTuitionFeeAsync(TuitionFeeDTO tuitionFeeDTO);
        Task<bool> DeleteTuitionFeeAsync(int tuitionFeeId);
        Task<IEnumerable<StudentTuitionFeeStatusDTO>> GetStudentsWithOverdueFeesByClassIdAsync(int classId);
    }
}
