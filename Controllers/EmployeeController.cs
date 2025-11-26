using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Lab1.Models;

namespace Lab1.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // -------------------------
        // SELECT ALL
        // -------------------------
        public IActionResult Index()
        {
            DataTable dt = new DataTable();

            try
            {
                string connStr = GetConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_Employee_SelectAll";

                        SqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading employee data: " + ex.Message;
            }

            return View("Index", dt);
        }

        // -------------------------
        // DELETE
        // -------------------------
        public IActionResult Delete(int EmpID)
        {
            try
            {
                string connStr = GetConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_Employee_Delete";
                        cmd.Parameters.AddWithValue("@EmpID", EmpID);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Employee deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting employee: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // -------------------------
        // ADD / EDIT (GET)
        // -------------------------
        [HttpGet]
        public IActionResult AddEdit(int? EmpID)
        {
            EmployeeModel model = new EmployeeModel
            {
                JoiningDate = DateTime.Today
            };

            try
            {
                ViewBag.DepartmentList = GetDepartmentList();

                if (EmpID.HasValue)
                {
                    string connStr = GetConnectionString();

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();

                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "PR_Emp_SelectByID";
                            cmd.Parameters.Add("@empID", SqlDbType.Int).Value = EmpID.Value;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    model.EmpID = Convert.ToInt32(reader["EmpID"]);
                                    model.EmpName = reader["EmpName"]?.ToString() ?? string.Empty;
                                    model.Salary = reader["Salary"] != DBNull.Value ? Convert.ToDecimal(reader["Salary"]) : 0;
                                    model.JoiningDate = reader["JoiningDate"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["JoiningDate"])
                                        : DateTime.Today;
                                    model.City = reader["City"]?.ToString() ?? string.Empty;
                                    model.DeptID = reader["DeptID"] != DBNull.Value ? Convert.ToInt32(reader["DeptID"]) : 0;
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "Employee not found.";
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading employee: " + ex.Message;
                return RedirectToAction("Index");
            }

            return View("AddEdit", model);
        }

        // -------------------------
        // ADD / EDIT (POST)
        // -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEdit(EmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DepartmentList = GetDepartmentList();
                return View("AddEdit", model);
            }

            try
            {
                string connStr = GetConnectionString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (model.EmpID == 0)
                        {
                            cmd.CommandText = "PR_Emp_Insert";
                        }
                        else
                        {
                            cmd.CommandText = "PR_Emp_Update";
                            cmd.Parameters.Add("@empID", SqlDbType.Int).Value = model.EmpID;
                        }

                        cmd.Parameters.Add("@empName", SqlDbType.VarChar, 100).Value = model.EmpName?.Trim();
                        SqlParameter salaryParam = cmd.Parameters.Add("@Salary", SqlDbType.Decimal);
                        salaryParam.Precision = 10;
                        salaryParam.Scale = 2;
                        salaryParam.Value = model.Salary;
                        cmd.Parameters.Add("@JoiningDate", SqlDbType.DateTime).Value = model.JoiningDate;
                        cmd.Parameters.Add("@city", SqlDbType.VarChar, 100).Value = model.City?.Trim();
                        cmd.Parameters.Add("@deptid", SqlDbType.Int).Value = model.DeptID;

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = model.EmpID == 0
                    ? "Employee added successfully!"
                    : "Employee updated successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving employee: " + ex.Message;
                ViewBag.DepartmentList = GetDepartmentList();
                return View("AddEdit", model);
            }
        }

        private List<SelectListItem> GetDepartmentList()
        {
            List<SelectListItem> departments = new List<SelectListItem>();
            string connStr = GetConnectionString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "PR_Department_SelectAll";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departments.Add(new SelectListItem
                            {
                                Value = reader["DeptID"].ToString(),
                                Text = reader["DepartmentName"].ToString()
                            });
                        }
                    }
                }
            }

            return departments;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("ConnectionString")
                ?? throw new InvalidOperationException("Connection string 'ConnectionString' is missing.");
        }
    }
}