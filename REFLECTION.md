# Reflection – Prague Parking 2.1

## 1. Summary

This project is a console-based parking management system for Prague Castle. It handles different vehicle types (Cars, Motorcycles, Bikes, and Buses) with different sizes and prices. The system saves data in JSON files and has an interactive menu built with Spectre.Console.

The solution is divided into four projects:
- **PragueParking.App**: The console application with menus
- **PragueParking.Domain**: Business logic and vehicle classes
- **PragueParking.Infrastructure**: File handling
- **PragueParking.Tests**: Unit tests

Main features include saving parking data to JSON, reloading configuration without restarting, registration number validation, and a visual map showing which spots are occupied.

**Note**: I have used AI (GitHub Copilot) as a tool during development to help with code suggestions, debugging, and understanding concepts.

## 2. How I solved the task

I separated the code into different layers - Domain for business logic, Infrastructure for file handling, and App for the UI. This makes the code easier to understand.

I used a unit-based system where each parking spot has 4 units:
- Bike: 1 unit
- Motorcycle: 2 units  
- Car: 4 units
- Bus: 16 units (needs 4 spots)

Buses were tricky because they need 4 parking spots in a row and can only park in spots 1-50. I solved this by reserving all 4 spots and storing the bus in the first one.

The pricing system gives 10 minutes free, then charges for each started hour. I used Spectre.Console to make the UI look professional with colors and tables.

## 3. Challenges

**Bus Parking Validation**: The system would reserve the bus but then fail saying the spot was too small. I fixed this by checking if the spot is already reserved for that specific vehicle before validating size.

**Showing Bus on All Spots**: Only the first spot showed the bus registration. I modified MapRenderer to display the bus registration on all 4 reserved spots.

**Registration Validation**: I implemented Swedish format (ABC123) with auto-cleaning that removes special characters. New vehicles must follow the format, but you can search for old vehicles without strict validation.

**Duplicate Prevention**: Added a check to prevent parking multiple vehicles with the same registration number.

## 4. Methods and technologies

- **OOP**: Inheritance with Vehicle base class, interfaces (IVehicle, IParkingSpot), dependency injection
- **Data**: System.Text.Json for JSON files, Dictionary and List collections
- **Technologies**: Spectre.Console (UI), MSTest (testing), LINQ (queries), Regex (validation)

## 5. What I would improve

- Move validation logic to a separate class
- Use better variable names (e.g., "registrationNumber" instead of "reg")
- Write more unit tests (currently only have the 2 required ones)
- Add parking history and statistics
- Better error messages that explain WHY something failed

## 6. Assignment conclusion

I managed to implement all the required features for Prague Parking 2.1:

✅ Different vehicle types with correct sizes (Bike=1, MC=2, Car=4, Bus=16)  
✅ Buses limited to first 50 spots and require 4 spots  
✅ Automatic test data creation when data.json is missing  
✅ Menu option to reload configuration  
✅ Using interfaces (IVehicle and IParkingSpot)  
✅ Correct pricing with 10 free minutes and hourly charging  
✅ Spectre.Console for the UI  
✅ JSON files for saving data  
✅ Unit tests with MSTest  

I also added some extra features like registration validation, preventing duplicates, back buttons in menus, and a price display option.

Overall, the project works as intended. There are definitely things I could improve (see section 5), but it meets all the requirements and handles the main scenarios correctly.

## 7. Course conclusion

This course taught me a lot about C# and how to structure code properly. I learned about organizing code into layers, using inheritance and interfaces in practice, working with JSON, and building better UIs.

**What was difficult:**
- Understanding when to create new classes vs. adding to existing ones
- Debugging issues across multiple files
- Getting the bus parking logic right took several attempts

**What went well:**
- The overall structure turned out clean
- Most features worked on first or second attempt
- Using AI as a tool helped me understand concepts faster and find solutions to problems

**Using AI:**
I used GitHub Copilot throughout the project for code suggestions and debugging help. It was useful for learning new syntax and getting unstuck, but I made sure to understand the code it suggested rather than just copying it. This helped me learn faster while still doing the work myself.

I feel comfortable building console applications now and have a better understanding of project structure. The parking project was challenging but manageable, and I can see how these concepts apply to larger applications.

