---
description: Build the N.I.N.A. Rotator plugin and clean up unused files
---

This workflow ensures the Rotator Plugin is cleanly built and that all temporary compilation folders and logs are deleted afterward, keeping the workspace neat. The compiled artifacts are automatically sent to the `Builds` folder by the `.csproj` configuration.

// turbo-all
1. Navigate to the `c:\Dockerhost\Nina_Rotator_Plugin` directory.
2. Run `dotnet clean`.
3. Run `dotnet build AltAzDeRotator.csproj`.
4. Run `Remove-Item -Path bin, obj, *.log -Recurse -Force` to clean up the unused files.
