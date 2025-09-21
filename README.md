






# StreamingConsoleApp

A lightweight C# console application for capturing and streaming video data using the RTMP (Real-Time Messaging Protocol) protocol. This is a test project demonstrating live streaming server capabilities.

##  Features

- **RTMP Server**: Built-in RTMP server for receiving and handling streaming data
- **Real-time Video Streaming**: Capture and stream video data in real-time
- **Console Logging**: Integrated logging system with console output for monitoring
- **Cross-platform**: Built on .NET 9.0 for cross-platform compatibility
- **Simple Setup**: Minimal configuration required to get started

## Technology Stack

- **.NET 9.0**: Latest .NET framework
- **C#**: Primary programming language
- **LiveStreamingServerNet**: Core streaming library (v0.31.1)
- **Microsoft.Extensions.Logging**: Logging framework (v9.0.7)

##  Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or JetBrains Rider (optional, for development)

##  Installation

1. Clone the repository:
```bash
git clone https://github.com/Saba-Burduli/StreamingConsoleApp.git
cd StreamingConsoleApp
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

##  Usage

Run the application:
```bash
dotnet run --project StreamingConsoleApp
```

The server will start and listen on:
- **Protocol**: RTMP
- **IP Address**: Any (0.0.0.0)
- **Port**: 1935 (default RTMP port)

##  Project Structure

```
StreamingConsoleApp/
├── StreamingConsoleApp/
│   ├── Program.cs              # Main application entry point
│   └── StreamingConsoleApp.csproj  # Project file with dependencies
├── StreamingConsoleApp.sln     # Visual Studio solution file
└── README.md                   # This file
```

##  Core Components

### Main Application (Program.cs)
- Initializes the LiveStreamingServer
- Configures console logging
- Sets up RTMP endpoint on port 1935
- Handles incoming streaming connections

### Dependencies
- **LiveStreamingServerNet**: Provides RTMP server functionality
- **Microsoft.Extensions.Logging.Console**: Enables console-based logging

##  RTMP Connection

To stream to this server, use any RTMP-compatible streaming software (like OBS Studio) with:
- **Server URL**: `rtmp://localhost:1935/live`
- **Stream Key**: (configure as needed)

##  Testing

This is a test project designed to demonstrate:
- RTMP protocol implementation
- Real-time video data handling
- Server-side streaming capabilities
- Console-based monitoring and logging

##  Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

##  License

This project is available for educational and testing purposes.

##  Contact

**Author**: Saba Burduli  
**Repository**: [StreamingConsoleApp](https://github.com/Saba-Burduli/StreamingConsoleApp)

---

*Note: This is a test project for learning and demonstration purposes. For production use, additional security and error handling features should be implemented.*

2025
