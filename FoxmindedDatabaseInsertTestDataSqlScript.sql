USE Shynkar_Task8_Foxminded;
GO

INSERT INTO COURSES (NAME, DESCRIPTION)
VALUES 
    ('Mathematics', 'An introductory course to Mathematics.'),
    ('Physics', 'Fundamentals of Physics.'),
    ('Chemistry', 'Basic concepts of Chemistry.'),
    ('Biology', 'Study of living organisms.'),
    ('Computer Science', 'Introduction to computer science and programming.');

INSERT INTO TEACHERS (FIRST_NAME, LAST_NAME)
VALUES
	('Alan', 'Dubrovsky'),
	('Jane', 'Green'),
	('Harry', 'Potter'),
	('Lucas', 'White'),
	('Philip', 'Jarg'),
	('Alice', 'Parker');

INSERT INTO GROUPS (COURSE_ID, TEACHER_ID, NAME)
VALUES 
    (1, 1, 'SR-01'),
    (1, 2, 'SR-02'),
    (2, 3, 'PH-01'),
    (3, 4, 'CH-01'),
    (4, 5, 'BI-01'),
    (5, 6, 'CS-01');

INSERT INTO STUDENTS (GROUP_ID, FIRST_NAME, LAST_NAME)
VALUES 
    (1, 'John', 'Doe'),
    (1, 'Jane', 'Smith'),
    (1, 'Alice', 'Johnson'),
    (2, 'Bob', 'Brown'),
    (2, 'Charlie', 'Davis'),
    (3, 'Eve', 'White'),
    (3, 'Frank', 'Black'),
    (4, 'Grace', 'Green'),
    (4, 'Heidi', 'Wong'),
    (5, 'Ivan', 'Garcia'),
    (5, 'Judy', 'Martinez'),
    (6, 'Kevin', 'Roberts'),
    (6, 'Liam', 'Taylor'),
    (6, 'Mia', 'Wilson'),
    (6, 'Olivia', 'Anderson'),
    (6, 'Noah', 'Harris'),
    (6, 'Sophia', 'Clark'),
    (6, 'James', 'Lewis'),
    (6, 'Isabella', 'Young'),
    (6, 'Lucas', 'King'),
    (6, 'Amelia', 'Scott'),
	(6, 'Jhon', 'Simpson');
