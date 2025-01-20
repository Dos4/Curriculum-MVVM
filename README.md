# Curriculum-MVVM

# Description
This WPF application is designed to manage data for courses, groups, students, and teachers. The application follows the MVVM pattern and utilizes Dapper ORM for database interactions. It provides a user-friendly interface for handling various entities, exporting/importing data, and generating reports in DOCX/PDF formats.

# Key Features:
Course and Group Management:

View a list of courses and their associated groups.
Use a TreeView for intuitive navigation through courses and groups.
Select a course to display its groups, and select a group to view its students.

Group Editing:
Create new groups or delete existing ones (restrictions apply: groups with students cannot be deleted).
Edit group names and assign/update a teacher as the group’s tutor.

Student Management:
Add, update, or delete students.
Edit student data (name and surname).

Teacher Management:
Manage teacher data by adding, updating, or deleting records.

CSV Import/Export:
Export a group’s student list to a CSV file (comma-separated).
Import a student list into a group (overwrites existing students in the group).
Report Generation:

Generate a DOCX or PDF document for a selected group, including:
The course name (title).
The group name (title).
A numbered list of students (full names).

# Usage
The application simplifies data management for courses, groups, students, and teachers. By supporting operations like adding, updating, exporting/importing, and generating detailed reports, it is a versatile tool for educational or organizational purposes.

# Technology Stack
Framework: WPF (Windows Presentation Foundation)
Design Pattern: MVVM (Model-View-ViewModel)
Database Interaction: Dapper ORM
Reporting Tools: DocX library for DOCX and iTextSharp for PDF generation

# Roadmap
Enhance navigation with additional filtering and search capabilities.
Add user authentication and roles for secure access.
Move the database to a cloud service for multi-user support.

# Support
Email: shynkarnikita@gmail.com
Telegram: @BuenasNo4es

Project Status
The project is currently in progress, with ongoing feature enhancements and optimizations.

Authors and Acknowledgment
Nikita Shynkar – Developer
Thanks to [Foxminded] for the educational support.
