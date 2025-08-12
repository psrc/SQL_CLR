# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a SQL CLR (Common Language Runtime) repository containing multiple C# assemblies that extend SQL Server functionality. Each subdirectory represents a separate SQL CLR project that compiles to a .dll and can be deployed to SQL Server as assemblies providing custom functions, aggregates, or user-defined types.

## Core Components

### Major CLR Assemblies
- **ApiGeocode**: Geocoding functions using Bing Maps and Google APIs, returns SQL Server geometry types
- **ProjNet**: Coordinate system transformations and projections for spatial data
- **SqlRegEx**: Regular expression functions for SQL Server (IsMatch, Replace, Matches, Split)
- **StringDistance**: Damerau-Levenshtein distance calculations for string matching
- **ElmerStatfunctions**: Statistical aggregate functions including median calculations
- **LocRecognize**: Location recognition and parsing functionality
- **XY_LatLng**: Coordinate conversion between XY and Lat/Lng formats

### Assembly Architecture
Each CLR project follows a standard pattern:
- **[ProjectName].cs**: Main implementation file with SqlFunction/SqlAggregate attributes
- **[ProjectName].sqlproj**: SQL Server Data Tools project file with UNSAFE permission set
- **[ProjectName].sln**: Visual Studio solution file
- **CLR_assembly_steps.sql**: SQL deployment script for creating assemblies, logins, and functions
- **[ProjectName]_key.pfx**: Strong name key file for assembly signing

## Build Commands

### Building Individual Projects
```bash
# Build a specific project (from project directory)
dotnet build

# Or using MSBuild for SQL projects
msbuild ApiGeocode.sqlproj /p:Configuration=Release
```

### Building All Solutions
```bash
# Build all .NET projects
find . -name "*.sln" -exec dotnet build {} \;

# Build specific project types
dotnet build ProjNet/ProjNet.sln
msbuild ApiGeocode/ApiGeocode.sln
```

## Development Workflow

### Adding New CLR Functions
1. Implement C# method with `[SqlFunction]` attribute
2. Update .sqlproj file to include new source files
3. Build the assembly to generate .dll
4. Update CLR_assembly_steps.sql with new function definitions
5. Deploy to SQL Server using the assembly deployment script

### Assembly Deployment Process
1. Copy built .dll to target server location (typically D:\clr\)
2. Run CLR_assembly_steps.sql script which:
   - Creates asymmetric key from assembly executable
   - Creates login with UNSAFE ASSEMBLY permission
   - Creates assembly with UNSAFE permission set
   - Creates SQL functions mapped to CLR methods

### Key Development Notes
- All assemblies require UNSAFE permission set due to web API calls and file system access
- Strong name signing is required using .pfx key files
- Target framework is .NET 4.8 for compatibility with SQL Server CLR
- External dependencies like ProjNet.dll must be referenced and deployed alongside main assemblies

### SQL Server Integration
- Functions are created as `EXTERNAL NAME [AssemblyName].[ClassName].[MethodName]`
- Geometry/Geography types use Microsoft.SqlServer.Types
- DataAccess = DataAccessKind.Read for functions that may query data
- SystemDataAccess = SystemDataAccessKind.None for deterministic functions

## Testing
- **ConsoleTestApp**: Test harness for testing CLR functions outside SQL Server
- Use SQL scripts in each project to test deployed functions
- Verify assembly loading and function creation through SQL Server Management Studio

## Dependencies
- Microsoft.SqlServer.Types for spatial data types
- ProjNet library for coordinate transformations (custom build in ProjNet/ directory)
- .NET Framework 4.8 runtime
- SQL Server CLR integration enabled

## Security Considerations
- All assemblies run with UNSAFE permission set
- API keys for geocoding services should be managed securely
- Strong name signing prevents assembly tampering
- Asymmetric key authentication provides secure assembly deployment