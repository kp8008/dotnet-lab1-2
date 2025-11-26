using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Lab1.Models;

namespace Lab1.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
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
                        cmd.CommandText = "PR_Department_SelectAll";

                        SqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading department data: " + ex.Message;
            }

            return View("Index", dt);
        }

        // -------------------------
        // DELETE
        // -------------------------
        public IActionResult Delete(int DeptID)
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
                        cmd.CommandText = "PR_Department_Delete";
                        cmd.Parameters.AddWithValue("@DeptID", DeptID);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Department deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting department: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // -------------------------
        // ADD / EDIT (GET)
        // -------------------------
        [HttpGet]
        public IActionResult AddEdit(int? DeptID)
        {
            DepartmentModel model = new DepartmentModel();

            if (DeptID.HasValue)
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
                            cmd.CommandText = "PR_Dept_SelectByID";
                            cmd.Parameters.Add("@deptID", SqlDbType.Int).Value = DeptID.Value;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    model.DeptID = reader["DeptID"] != DBNull.Value ? Convert.ToInt32(reader["DeptID"]) : 0;
                                    model.DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty;
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "Department not found.";
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error loading department: " + ex.Message;
                    return RedirectToAction("Index");
                }
            }

            return View("AddEdit", model);
        }

        // -------------------------
        // ADD / EDIT (POST)
        // -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEdit(DepartmentModel model)
        {
            if (!ModelState.IsValid)
            {
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

                        if (model.DeptID == 0)
                        {
                            cmd.CommandText = "PR_Dept_Insert";
                            cmd.Parameters.Add("@deptName", SqlDbType.VarChar, 100).Value = model.DepartmentName?.Trim();
                        }
                        else
                        {
                            cmd.CommandText = "PR_Dept_Update";
                            cmd.Parameters.Add("@deptID", SqlDbType.Int).Value = model.DeptID;
                            cmd.Parameters.Add("@departmentName", SqlDbType.VarChar, 100).Value = model.DepartmentName?.Trim();
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = model.DeptID == 0
                    ? "Department added successfully!"
                    : "Department updated successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving department: " + ex.Message;
                return View("AddEdit", model);
            }
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("ConnectionString")
                ?? throw new InvalidOperationException("Connection string 'ConnectionString' is missing.");
        }
    }
}