# Prague Parking 2.1 🚗

A parking management system for Prague Castle built with C# and .NET 8.

## Overview

Console application that manages parking for different vehicle types (Cars, Motorcycles, Bikes, Buses). Uses JSON files for data storage and Spectre.Console for the UI.

**Features:**
- 100 parking spots with color-coded map
- Different vehicle sizes: Bike (1 unit), Motorcycle (2), Car (4), Bus (16 units/4 spots)
- Pricing: First 10 minutes free, then hourly rates
- Registration validation (Swedish format ABC123)
- Hot-reload configuration without restart

## Quick Start

**With Visual Studio Code (VS Code):**
1. Open the folder in VS Code
2. Open Terminal (Ctrl+Ö or Ctrl+`)
3. Run: `dotnet run --project src/PragueParking.App`

**With Visual Studio (full IDE):**
1. Open `PragueParking_V2.1.sln`
2. Right-click `PragueParking.App` in Solution Explorer
3. Select "Set as Startup Project"
4. Press F5

**With Command Line:**
```bash
dotnet run --project src/PragueParking.App
```

## Project Structure

```
src/
├── PragueParking.App/          # Console UI and menus
├── PragueParking.Domain/       # Business logic and entities
├── PragueParking.Infrastructure/   # File handling
└── PragueParking.Tests/        # Unit tests
```

## Configuration

Edit `src/PragueParking.App/assets/config.json` to change:
- Number of parking spots
- Vehicle sizes
- Pricing per hour
- Free minutes

Use menu option "Reload config/prices" to apply changes without restarting.

## Usage

**Main menu:**
1. Park vehicle
2. Find vehicle
3. Move vehicle
4. Check-out vehicle (shows price)
5. Show map
6. Show prices
7. Reload config/prices
8. Save & Exit

**Registration format:** 3 letters + 3 digits (example: ABC123)

**Parking rules:**
- Buses need 4 spots in a row and can only use spots 1-50
- Multiple small vehicles can share one spot

## Testing

```bash
dotnet test
```

## Technologies

- .NET 8.0
- Spectre.Console (UI)
- System.Text.Json (persistence)
- MSTest (testing)
- Duplicate prevention
- Back navigation
- Price display menu
- Input auto-cleaning

## 👤 Author

Created as part of the C# programming course assignment.

## 📄 License

Educational project - see course requirements for usage terms.

