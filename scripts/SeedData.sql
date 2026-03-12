USE StudentManagementDB;
GO

INSERT INTO Departments (Name, Location) VALUES 
('Computer Science', 'Main Campus - Block B'),
('Electrical Engineering', 'Main Campus - Block C'),
('Business Information Technology', 'Main Campus - Block A'),
('Mechanical Engineering', 'Main Campus - Block D'),
('Mathematics and Statistics', 'Main Campus - Block E');

INSERT INTO Students (FirstName, LastName, Email, DateOfBirth, DeptID) VALUES 
('Alice', 'Wanjiku', 'alice.w@example.com', '2002-05-15', 1),
('Brian', 'Ochieng', 'brian.o@example.com', '2001-11-20', 2),
('Catherine', 'Muthoni', 'cathy.m@example.com', '2003-02-10', 3),
('David', 'Kiptoo', 'david.k@example.com', '2002-08-30', 4),
('Emily', 'Chepngetich', 'emily.c@example.com', '2001-12-05', 5);

INSERT INTO Courses (Title, Credits, DeptID) VALUES 
('Introduction to C#', 3, 1),
('Circuit Theory', 4, 2),
('Database Management Systems', 3, 3),
('Thermodynamics', 4, 4),
('Linear Algebra', 3, 5);

INSERT INTO Enrollments (StudentID, CourseID, Grade, Semester) VALUES 
(1, 1, 'A', 'Semester 1 2026'),
(2, 2, 'B', 'Semester 1 2026'),
(3, 3, 'A', 'Semester 1 2026'),
(4, 4, 'C', 'Semester 1 2026'),
(5, 5, 'B', 'Semester 1 2026');
GO
