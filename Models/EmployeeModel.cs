namespace Lab1.Models
{
    public class EmployeeModel
        {
            public int EmpID { get; set; }
            public string EmpName { get; set; } = string.Empty;
            public decimal Salary { get; set; }
            public DateTime JoiningDate { get; set; }
            public string City { get; set; } = string.Empty;
            public int DeptID { get; set; }

            // Optional navigation property for join data
            public string DepartmentName { get; set; } = string.Empty;
        }
 

}