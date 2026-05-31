# DOM Tree Visualizer Project

This project is a web-based DOM Tree Visualizer that parses HTML using a custom C# backend and visualizes the structure as an N-ary tree on the frontend.

## Project Structure
- `backend/DomParserApi`: ASP.NET Core Web API with custom data structures (Stack, Queue, HashTable) and a Regex-based HTML parser.
- `wwwroot`: Modern UI for visualizing the DOM tree.

## How to Run

1. **Prerequisites**: Ensure you have the .NET SDK installed.
2. **Navigate to Backend**:
   ```bash
   cd backend/DomParserApi
   ```
3. **Run the Application**:
   ```bash
   dotnet run --launch-profile http
   ```
4. **Access the App**:
   Open your browser and go to [http://localhost:5175](http://localhost:5175)

## Features
- **N-ary Tree Visualization**: Correctly displays parent-child relationships.
- **Text Node Parsing**: Now captures and displays text content between tags.
- **Attribute Support**: Correctly handles multiple classes and IDs using Regex.
- **Search**: BFS-based search for tags, classes, and IDs.
