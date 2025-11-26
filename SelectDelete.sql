create database ADN

use ADN

CREATE TABLE Department (
    DeptID INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName VARCHAR(100) NOT NULL
);

CREATE TABLE Employee (
    EmpID INT PRIMARY KEY IDENTITY(1,1),
    EmpName VARCHAR(100) NOT NULL,
    Salary DECIMAL(10,2) NOT NULL,
    JoiningDate DATETIME NOT NULL,
    City VARCHAR(100) NOT NULL,
    DeptID INT,
    FOREIGN KEY (DeptID) REFERENCES Department(DeptID)
);

-- Insert Data into Department Table
INSERT INTO Department (DepartmentName) VALUES 
('Human Resources'),
('Finance'),
('IT'),
('Marketing'),
('Sales');

-- Insert Data into Employee Table
INSERT INTO Employee (EmpName, Salary, JoiningDate, City, DeptID) VALUES
('Rahul Mehta', 55000.00, '2023-01-15', 'Ahmedabad', 1),
('Priya Shah', 65000.00, '2022-11-10', 'Surat', 2),
('Karan Patel', 80000.00, '2021-06-25', 'Rajkot', 3),
('Sneha Joshi', 60000.00, '2023-03-12', 'Vadodara', 4),
('Vikas Rao', 75000.00, '2022-08-05', 'Mumbai', 5),
('Aisha Khan', 70000.00, '2023-09-01', 'Pune', 3),
('Jay Desai', 58000.00, '2024-02-20', 'Ahmedabad', 1),
('Neha Verma', 72000.00, '2022-10-17', 'Surat', 5);

-- View the Data
SELECT * FROM Department;
SELECT * FROM Employee;

---stored-proc
---department

CREATE PROCEDURE PR_Department_SelectAll
AS
BEGIN
    SELECT DeptID, DepartmentName
    FROM Department;
END;
GO

CREATE PROCEDURE PR_Department_Delete
    @DeptID INT
AS
BEGIN
    DELETE FROM Department
    WHERE DeptID = @DeptID;
END;
GO

----employee
CREATE PROCEDURE PR_Employee_SelectAll
AS
BEGIN
    SELECT e.EmpID, e.EmpName, e.Salary, e.JoiningDate, e.City, e.DeptID, d.DepartmentName
    FROM Employee e join department d
	on e.deptid=d.deptid
END;
GO

ALTER PROCEDURE PR_Employee_SelectAll
AS
BEGIN
    SELECT 
        e.EmpID,
        e.EmpName,
        e.Salary,
        e.JoiningDate,
        e.City,
        e.DeptID,
        d.DepartmentName   -- <<< this column is needed by your view
    FROM Employee e
    LEFT JOIN Department d ON e.DeptID = d.DeptID;
END;
GO




CREATE PROCEDURE PR_Employee_Delete
    @EmpID INT
AS
BEGIN
    DELETE FROM Employee
    WHERE EmpID = @EmpID;
END;
GO


