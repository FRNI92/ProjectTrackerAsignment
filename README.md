# KonsollApplication - Admin Solution
This solution allows you to manage your company’s data as an admin through a console application. 
You can Create, Read, Update, and Delete (CRUD) all entities using different services.

## Features
- **Project Creation & Selection**:
When creating a new project, you will be asked to choose from a list of options, such as 1, 2, or 3. Even if the database has inconsistent IDs
(e.g., an entity with ID 11 appears as option 3), the system ensures a user-friendly selection process while correctly mapping the choices in the background.

- **Automatic Project Cost Calculation**:
The ProjectService injects ServiceService to calculate the total cost of a project based on its selected service.
This also applies when updating a project, ensuring accurate cost adjustments.

- **Preloaded Data**:
The DataContext contains hard-coded data that is automatically inserted into the database tables at startup.

**Testing**
Integration Tests:
I created integration tests for ProjectRepository and EmployeeRepository since they contain additional logic requiring Include() for related data.

**Mock Tests**:
I wrote mock-based unit tests for services that interact with repositories and other services. 
For example, ProjectService depends on ServiceService, so I mocked ServiceService to keep the tests focused on ProjectService. I also used real factories in these tests.

**Development & Database**
Controllers & API Testing:
Before implementing MenuDialogs, I created Controllers to test the services using Postman. That’s why a Web API project is still included in the solution.

**Database**:
The project was developed using Microsoft SQL Server, and I used SQL Server Management Studio (SSMS) for debugging and troubleshooting.

**Entity Framework Core**:
I used Entity Framework Core (EF Core) to generate database tables based on C# models, automatically translating them into SQL code.

**Base Repository Pattern**:
The BaseRepository handles database communication, while ProjectRepository and EmployeeRepository override certain methods to include related data using Include().
