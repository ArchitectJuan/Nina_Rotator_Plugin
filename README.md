# N.I.N.A. Alt-Az De-Rotator Plugin

## Overview
This N.I.N.A. (Nighttime Imaging 'N' Astronomy) plugin actively compensates for field rotation on Alt-Azimuth telescope mounts. It seamlessly interfaces with your connected Telescope and Rotator hardware via N.I.N.A. 3.0's equipment management API, continuously calculating and applying the required rotational adjustments during your imaging sessions.

## Features
- **Real-time Telemetry:** Reads live Altitude, Azimuth, and site Latitude directly from your N.I.N.A. equipment profile.
- **User Interface:** Dedicated dockable window to monitor current altitude, azimuth, rotation rate, and target position.
- **On/Off Control:** Simple toggle switch to enable or disable active de-rotation as needed.
- **Dynamic Calculation:** Uses a built-in math engine to compute the exact degrees of rotation required per second to offset Earth's rotation for Alt-Az setups.
- **Micro-step Prevention:** Intelligent 0.01-degree thresholding ensures your Rotator is only commanded when necessary, preventing unnecessary hardware wear.
- **N.I.N.A. 3.x Compatibility:** Built against the modern .NET 8.0 framework and N.I.N.A. 3.0 Managed Extensibility Framework (MEF).

## Version History
- **V1.1.0:** Added User Interface and On/Off switch control. Migrated to DockableVM pattern.
- **V1.0.0:** Initial Release. Features background polling service, dynamic rate calculation, and MEF dependency injection.

## Installation
Refer to the `UserGuide.md` for complete installation and usage instructions.

## Building from Source
This project requires Visual Studio or the .NET 8.0 SDK.
Run `dotnet build AltAzDeRotator.csproj` to compile the library. Output files are automatically copied to the `Builds\Nina_Rotator_Plugin_V1.0` folder.

<img width="1919" height="1045" alt="image" src="https://github.com/user-attachments/assets/01ff4a47-6c4b-45c0-91f5-5c990f143ca4" />
<img width="1919" height="1044" alt="image" src="https://github.com/user-attachments/assets/fb85dfe0-59e2-4403-958e-295583d68a73" />
<img width="455" height="245" alt="image" src="https://github.com/user-attachments/assets/394c7b31-d0e6-4c4b-8207-d081759b905a" />
