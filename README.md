# Job Tracker
Simple application to keep track of job applications

## Features
- Each added posting can hold multiple details
	- Position, Company, Posting URL, Arrangement (Onsite etc.), Type (Fulltime etc), Location, Distance, and Status (Applied etc.)
- Can store geocoded locations, and calculate distance from your current location to potential jobs.
- Create/Load save files for different sets of job postings.

## Technologies
- WPF, C#
- MVVM Pattern (Using Community Tookit)
- SQLite for caching and storage
- Geocoding via Nominatim (Calculate distances with Haversine)

## Learned Subjects
- C# (Events, Properties, DependencyProperties, etc.)
- SQLite Language/Syntax, Parameter Isolation
- WPF XAML (UI, Resource Dictionaries, Namespaces, Binding, Styles, etc.)
- MVVM (View/ViewModel/Model Seperation, Binding, Commands, Validators, etc.)
- Caching - Geocoding requests are stored in a cache database to increase performance and lessen requests to Nominatim.