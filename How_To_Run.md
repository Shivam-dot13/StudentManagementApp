🚀 How to Run This Project
Prerequisites
Visual Studio 2022 (Community Edition is fine)

The .NET desktop development workload installed (you can get this from the Visual Studio Installer)

Step-by-Step Instructions
Clone the Repository:

Bash

git clone https://github.com/YourUsername/YourRepositoryName.git
Open the Solution:

Navigate into the cloned folder.

Double-click the StudentManagementApp.sln file to open it in Visual Studio.

Restore NuGet Packages:

Visual Studio should do this automatically. If not, right-click the solution in the Solution Explorer and select Restore NuGet Packages.

Run the Database Migration (Critical Step):

This project uses Entity Framework Core to create a local students.db file. You must create this file before running the app.

Go to Tools -> NuGet Package Manager -> Package Manager Console.

In the console window that opens, type the following command and press Enter:

PowerShell

Update-Database
You will see some "Done" messages. This has now created the students.db file in your project folder.

Run the Application:

Press F5 or click the "Start" button (with the green play icon) at the top of Visual Studio.
