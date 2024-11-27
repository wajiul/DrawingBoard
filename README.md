# Collaborative Drawing Board

A real-time, multi-user collaborative drawing board built using **ASP.NET Core MVC**, **Fabric.js**, **SignalR**, and **PostgreSQL**. This project allows multiple users to interact on a shared canvas in real-time, providing a seamless collaborative drawing experience.

Check out the live demo: [Collaborative Drawing Board Live](http://www.owajeul.somee.com/)
## Features

- **Real-Time Collaboration**: Users can draw, edit, and interact with the shared canvas simultaneously.
- **Drawing Tools**: Includes various drawing tools like pencil, shapes, and eraser.
- **Object Manipulation**: Allows resizing, rotating, and moving of objects on the canvas.
- **Seamless Updates**: Instant updates for all users via SignalR.
- **Persistence**: Saves board data to a PostgreSQL database for retrieval and synchronization.

## Technologies Used

- **ASP.NET Core MVC**: Backend framework for handling server-side logic and API endpoints.
- **Fabric.js**: Client-side library for managing canvas-based drawings.
- **SignalR**: Real-time communication between clients and server.
- **PostgreSQL**: Database for storing user and drawing data.

## Installation

### Setup Instructions

1. **Clone the Repository**

   ```bash
   git clone https://github.com/wajiul/drawingboard.git
   cd collaborative-drawing-board
2. **Set Up Database**

  - Create a PostgreSQL database.
  - Update the connection string in appsettings.json
    
4. **Run Database Migrations**

     ```bash
     dotnet ef database update

5. **Run the Application**
     ```bash
     dotnet run
