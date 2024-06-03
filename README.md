
# Backend Trip Recruitment Task

This is a proof-of-concept backend application for managing trips, written in Domain-Driven Design (DDD). The application is built using ASP.NET Core and follows the CQRS (Command Query Responsibility Segregation) pattern using MediatR. All this to add variety to the task, in a real production environment everything should be agreed and applied according to the needs of the business.

I encourage anyone reviewing the code for this solution to **save a note (issue, PR) or comment on the code with your insights**! This will help me to be a better and better software engineer, thanks!

Live: [https://backend-trip-recruitment-task.azurewebsites.net/swagger/index.html](https://backend-trip-recruitment-task.azurewebsites.net/swagger/index.html)

## Features

- Create, edit, and delete trips
- List all trips or search trips by country
- Get details of a single trip
- Register users for a trip

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- MediatR
- xUnit
- Moq

## Environment (FYI)

- MacOS Sonoma 14.5
- .NET 8.0.301 SDK

## Getting Started

To get started with the project, follow these steps:

1. Clone the repository to your local machine.
```
git clone git@github.com:oreze/backend-trip-recruitment-task.git
cd backend-trip-recruitment-task
```
2. Build the solution.
```
dotnet restore
dotnet build --no-restore
```
3. Run tests.
```
dotnet test --no-build --verbosity quiet
```
4. Run the application.
```
dotnet run --project src/BackendTripRecruitmentTask.API
```

## Testing

The project includes unit & integration tests using xUnit and Moq. To run the tests, navigate to the `BackendTripRecruitmentTask.UnitTests` or `BackendTripRecruitmentTask.UnitTests` project and run the tests using your preferred test runner.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more information.

In case of any questions, reach me directly on Github or via LinkedIn (preferrable).
