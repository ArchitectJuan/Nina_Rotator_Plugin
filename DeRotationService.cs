using System;
using System.Threading;
using System.Threading.Tasks;
using NINA.Core.Utility;

namespace AltAzDeRotator
{
    public class DeRotationService
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _backgroundTask;
        private NINA.Equipment.Interfaces.Mediator.ITelescopeMediator? _telescopeMediator;
        private NINA.Equipment.Interfaces.Mediator.IRotatorMediator? _rotatorMediator;
        private readonly DeRotationViewModel _viewModel;

        public DeRotationService(NINA.Equipment.Interfaces.Mediator.ITelescopeMediator? telescopeMediator, NINA.Equipment.Interfaces.Mediator.IRotatorMediator? rotatorMediator, DeRotationViewModel viewModel)
        {
            _telescopeMediator = telescopeMediator;
            _rotatorMediator = rotatorMediator;
            _viewModel = viewModel;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            if (_cancellationTokenSource != null)
            {
                _backgroundTask = Task.Run(() => BackgroundLoop(_cancellationTokenSource.Token));
                _viewModel.IsActive = true;
                _viewModel.Status = "Running";
            }
            Logger.Info("DeRotationService background loop started.");
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                // Wait briefly for the task to complete
                _backgroundTask?.Wait(TimeSpan.FromSeconds(2));
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _viewModel.IsActive = false;
                _viewModel.Status = "Stopped";
                Logger.Info("DeRotationService background loop stopped.");
            }
        }

        private async Task BackgroundLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!_viewModel.IsDeRotationEnabled)
                    {
                        _viewModel.Status = "Disabled";
                        _viewModel.IsActive = false;
                        await Task.Delay(1000, token);
                        continue;
                    }

                    // Utilize mediators
                    if (_telescopeMediator != null && _rotatorMediator != null)
                    {
                        var telescope = _telescopeMediator?.GetDevice() as NINA.Equipment.Interfaces.ITelescope;
                        var rotatorDevice = _rotatorMediator?.GetDevice() as NINA.Equipment.Interfaces.IRotator;

                        if (telescope != null && telescope.Connected && rotatorDevice != null && rotatorDevice.Connected)
                        {
                            _viewModel.Status = "Running";
                            _viewModel.IsActive = true;
                            
                            // 1. Get current altitude and azimuth from the mount telemetry
                            double alt = 0.0;
                            double az = 0.0;
                            double lat = 40.0;

                            try
                            {
                                // The mount tracking altitude
                                alt = telescope.Altitude;
                                az = telescope.Azimuth;
                                // The user's geological location
                                lat = telescope.SiteLatitude;
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"Failed to fetch mount telemetry. Mount drivers might not expose Altitude: {ex.Message}");
                            }
                            
                            _viewModel.Altitude = alt;
                            _viewModel.Azimuth = az;

                            // 2. Calculate required rotation rate using our MathEngine
                            double requiredRateDegreesPerHour = MathEngine.CalculateRotationRate(alt, az, lat);
                            _viewModel.RotationRate = requiredRateDegreesPerHour;

                            // Calculate how many degrees we should move in this 1-second polling interval
                            double degreesPerSecond = requiredRateDegreesPerHour / 3600.0;
                            
                            // We can query position from the rotator mediator directly or its info
                            // Let's assume rotatorMediator has GetTargetPosition or similar, 
                            // but we can also use dynamic device object:
                            // We already verified the rotator is connected above
                            {
                                double currentRotatorPosition = 0;
                                try
                                {
                                    currentRotatorPosition = rotatorDevice.Position;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"Failed to fetch rotator position: {ex.Message}");
                                    await Task.Delay(1000, token);
                                    continue;
                                }
                                
                                double newTargetPosition = currentRotatorPosition + degreesPerSecond;
                                
                                newTargetPosition = newTargetPosition % 360.0;
                                if (newTargetPosition < 0) newTargetPosition += 360.0;
                                
                                _viewModel.TargetPosition = newTargetPosition;

                                if (Math.Abs(newTargetPosition - currentRotatorPosition) > 0.01) 
                                {
                                    // Use the mediator's strongly typed Move method
                                    if (_rotatorMediator != null)
                                    {
                                        _viewModel.TotalRotationApplied += degreesPerSecond;
                                        _ = Task.Run(() => _rotatorMediator.Move((float)newTargetPosition, token), token);
                                    }
                                }
                            }
                        }
                        else
                        {
                             _viewModel.Status = "Mount and rotator are not connected";
                             _viewModel.IsActive = false;
                        }
                    }
                    
                    await Task.Delay(1000, token); // Poll and update every second
                }
                catch (TaskCanceledException)
                {
                    // Expected when the loop is stopped
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error in DeRotationService background loop: {ex.Message}");
                    _viewModel.Status = $"Error: {ex.Message}";
                    await Task.Delay(1000, token); // Prevent runaway loop
                }
            }
        }
    }
}