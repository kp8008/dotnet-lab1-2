--insert and update
--department

CREATE PROCEDURE PR_Dept_Insert
    @deptName VARCHAR(100)
AS
BEGIN
    INSERT INTO Department(DepartmentName)
    VALUES(@deptName);
END

CREATE PROCEDURE PR_Dept_Update
    @deptID INT,
    @departmentName VARCHAR(100)
AS
BEGIN
    UPDATE Department
    SET DepartmentName = @departmentName
    WHERE deptID = @deptID;
END

CREATE PROCEDURE PR_Dept_SelectByID
    @deptID INT
AS
BEGIN
    SELECT deptID, DepartmentName
    FROM Department
    WHERE deptID = @deptID;
END


--employee
CREATE PROCEDURE PR_Emp_Insert
    @empName VARCHAR(100),
    @Salary DECIMAL(8,2),
    @JoiningDate DATETIME,
    @city VARCHAR(100),
    @deptid INT
AS
BEGIN
    INSERT INTO Employee(empName, Salary, JoiningDate, city, DeptID)
    VALUES(@empName, @Salary, @JoiningDate, @city, @deptid);
END


CREATE PROCEDURE PR_Emp_Update
    @empID INT,
    @empName VARCHAR(100),
    @Salary DECIMAL(8,2),
    @JoiningDate DATETIME,
    @city VARCHAR(100),
    @deptid INT
AS
BEGIN
    UPDATE Employee
    SET empName = @empName,
        Salary = @Salary,
        JoiningDate = @JoiningDate,
        city = @city,
        DeptID = @deptid
    WHERE empID = @empID;
END

CREATE PROCEDURE PR_Emp_SelectByID
    @empID INT
AS
BEGIN
    SELECT empID, empName, Salary, JoiningDate, city, DeptID
    FROM Employee
    WHERE empID = @empID;
END
