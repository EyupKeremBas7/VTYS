# Student-Academic Management System

---

## Description

This project is a **Student-Academic Management System** designed to teach the processes of multi-layered web application development. The system allows students and academics to:
- Update their profile information
- Select courses
- Approve or reject course selections

It also incorporates features such as database interaction via an API and the use of a multi-layered architecture.

---

## Features

- **Login and Logout:** Users can log in and log out of the system.
- **Profile Management:** Users can view and update their profile information.
- **Course Selection:** Students can browse available courses and enroll; academics can approve or reject course selections.
- **Database Integration:** Interacts with the database through an API.
- **Multi-Layered Architecture:** The application is divided into data access, business logic, and user interface layers (Models-Controllers-Views, also known as MVC).

---

## Database with SQL Server

### Tables

#### 1. Course
- **CourseID** (Primary Key): Unique identifier for each course.
- **CourseName**: Name of the course.
- **isMandatory**: Boolean field indicating whether the course is mandatory or elective.
- **Credit**: Number of credits the course is worth.
- **Class**: Year or grade level for which the course is designed.
- **InstructorID** (Foreign Key): Links to the `Instructor` table to identify the instructor teaching the course.

---

#### 2. Instructor
- **InstructorID** (Primary Key): Unique identifier for each instructor.
- **FullName**: Full name of the instructor.
- **Title**: Professional title of the instructor.
- **EMail**: Contact email of the instructor.
- **Department**: Department in which the instructor works.
- **Password**: Password for the instructor’s account.

---

#### 3. SelectedCourse
- **SelectionID** (Primary Key): Unique identifier for each course selection.
- **StudentID** (Foreign Key): Links to the `Student` table to identify the student who selected the course.
- **CourseID** (Foreign Key): Links to the `Course` table to identify the selected course.
- **InstructorID** (Foreign Key): Links to the `Instructor` table for course approval.
- **isApproved**: Boolean field indicating whether the course selection has been approved by the instructor.

---

#### 4. Student
- **StudentID** (Primary Key): Unique identifier for each student.
- **Fullname**: Full name of the student.
- **E-mail**: Contact email of the student.
- **InstructorID** (Foreign Key): Links to the `Instructor` table to assign an advisor.
- **Password**: Password for the student’s account.
- **Major**: Student's major or field of study.
- **Class**: Current academic year or level of the student (e.g., 1st Year, 2nd Year).

---

## Controllers

### Instructor
- **GET**: `/Instructor/Login`
- **POST**: `/Instructor/Login`
- **GET**: `/Instructor/Details/{id}`
- **GET**: `/Instructor/getInstructorList`
- **GET**: `/Instructor/UpdateInfo/{id}`
- **POST**: `/Instructor/UpdateInfo/{id}`
- **GET**: `/Instructor/SelectAdjectiveCourse/{id}`
- **POST**: `/Instructor/ApproveCourse/{id}`
- **POST**: `/Instructor/RejectCourse/{id}`

### Student
- **GET**: `/Student/Login`
- **POST**: `/Student/Login`
- **GET**: `/Student/Details/{id}`
- **GET**: `/Student/getStudentList`
- **GET**: `/Student/UpdateInfo/{id}`
- **POST**: `/Student/UpdateInfo/{id}`
- **GET**: `/Student/SelectAdjectiveCourse/{id}`
- **POST**: `/Student/SelectAdjectiveCourse/{id}`

---

## Pages

The application utilizes **Razor Pages** to handle user interactions and display dynamic content. Razor Pages are designed to make page-focused scenarios easier and more productive. Each page is represented by a `.cshtml` file and its associated page model class, which contains the logic for handling requests and responses.

### Key Razor Pages

#### Student
- **Login Page:** Allows students to log into the system.
- **Details Page:** Allows students to view and update their personal information.
- **SelectAdjectiveCourse Page:** Enables students to browse and enroll in available courses.
- **UpdateInfo Page:** Allows students to update their information.

#### Instructor
- **Login Page:** Allows instructors to log into the system.
- **Details Page:** Allows instructors to view and update their personal information.
- **AdjectiveCourse Page:** Enables instructors to manage courses.
- **UpdateInfo Page:** Allows instructors to update their information.

---

## Technologies

- **Backend:** ASP.NET Core
- **Frontend:** HTML, CSS, JavaScript
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Testing:** Postman

---

## Contributor

Eyüp Kerem Baş
