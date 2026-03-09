# Alt-Az De-Rotator User Guide

Welcome to the Alt-Az De-Rotator Plugin for N.I.N.A. This guide will walk you through the installation and operational steps to get your field rotator compensating for Alt-Azimuth tracking.

## 1. Installation

1. Navigate to the `Builds\AltAz.DeRotator` directory in this repository.
2. Ensure you have the following files:
   - `AltAzDeRotator.dll`
   - `manifest.json`
3. Locate your N.I.N.A. 3.0 Plugins version folder. By default, this is located at:
   `%localappdata%\NINA\Plugins\3.0.0`
4. Create a new folder inside the `3.0.0` directory named exactly **`AltAz.DeRotator`**.
5. Copy the contents of `Builds\AltAz.DeRotator` into this new folder.
6. **Important**: The folder name must match the plugin ID `AltAz.DeRotator`.
7. Restart N.I.N.A.

## 2. Requirements

- N.I.N.A. version 3.0 or higher.
- A connected and communicating Telescope mount (ASCOM, EQMOD, GreenSwamp, etc.) with valid Altitude and Azimuth telemetry.
- A connected Rotator (ASCOM-compatible) capable of Absolute Positioning movements.

## 3. Operation

The plugin operates silently in the background once initialized by N.I.N.A. 

1. **Connect your Equipment:** Open N.I.N.A., go to the **Equipment** tab, and connect both your Telescope and your Rotator.
2. **Start Tracking:** Once your telescope is unparked and tracking, the plugin will automatically begin calculating the field rotation rate.
3. **Automatic Adjustments:** Every second, the plugin calculates the required position offset. If the required change exceeds `0.01 degrees`, a command is issued to the Rotator to adjust its angle. 

*Note: You may see your rotator periodically make tiny adjustments throughout the night. This is expected behavior to compensate for geometric field rotation!*
