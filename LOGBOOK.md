# Logbook – Prague Parking 2.1

## Monday 13 October – Domain model & solution setup
- Initialized the solution and created the projects: `App`, `Domain`, `Infrastructure`, `Tests`.
- Designed core domain entities: `Vehicle`, `ParkingSpot`, `ParkingSession`.
- Defined interfaces for pricing, allocation, and storage services.

## Tuesday 14 October – Persistence & configuration
- Implemented `ConfigProvider` for loading configuration from file.
- Built `FileStorage` for JSON-based persistence of data and settings.
- Created `PriceListProvider` to handle dynamic pricing configuration.

## Wednesday 15 October – Data models & serialization
- Defined serializable models and DTOs for domain and storage layers.
- Added JSON serialization/deserialization logic using System.Text.Json.
- Validated configuration inputs and ensured fallbacks for defaults.

## Thursday 16 October – UI with Spectre.Console
- Integrated `Spectre.Console` to build a text-based menu interface.
- Developed the main menu with interactive navigation.
- Implemented a map renderer for visualizing parking slot status.

## Friday 17 October – Pricing logic & parking rules
- Developed full pricing logic based on vehicle type, duration, and zones.
- Enforced parking constraints for buses (e.g., disallowed in regular spots).
- Implemented config validation with clear error handling.

## Saturday 18 October – Unit tests & seed data
- Added MSTest unit tests for pricing calculation and parking allocation.
- Implemented automatic seed data generation when `data.json` is missing.
- Performed manual testing of main user flows.

## Sunday 19 October – Final polish & GitHub release
- Performed full manual run-through to check edge cases and stability.
- Cleaned up code, added inline documentation and comments.
- Pushed final version to GitHub and marked as v2.1 release.
