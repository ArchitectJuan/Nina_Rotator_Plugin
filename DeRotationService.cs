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

        public DeRotationService(NINA.Equipment.Interfaces.Mediator.ITelescopeMediator? telescopeMediator, NINA.Equipment.Interfaces.Mediator.IRotatorMediator? rotatorMediator)
        {
            _telescopeMediator = telescopeMediator;
            _rotatorMediator = rotatorMediator;
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            if (_cancellationTokenSource != null)
            {
                _backgroundTask = Task.Run(() => BackgroundLoop(_cancellationTokenSource.Token));
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
                Logger.Info("DeRotationService background loop stopped.");
            }
        }

        private async Task BackgroundLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Utilize mediators
                    if (_telescopeMediator != null && _rotatorMediator != null)
                    {
                        dynamic? telescope = _telescopeMediator?.GetDevice();

                        if (telescope != null && telescope.Connected == true)
                        {
                            // 1. Get current altitude and azimuth from the mount telemetry
                            double alt = telescope.Altitude;
                            double az = telescope.Azimuth;
                            
                            // Let's assume a default latitude for now if we can't fetch it
                            double lat = 40.0; 

                            // 2. Calculate required rotation rate using our MathEngine
                            double requiredRateDegreesPerHour = MathEngine.CalculateRotationRate(alt, az, lat);
                            
                            // Calculate how many degrees we should move in this 1-second polling interval
                            double degreesPerSecond = requiredRateDegreesPerHour / 3600.0;
                            
                            // We can query position from the rotator mediator directly or its info
                            // Let's assume rotatorMediator has GetTargetPosition or similar, 
                            // but we can also use dynamic device object:
                            dynamic? rotatorDevice = _rotatorMediator?.GetDevice();
                            if (rotatorDevice != null && rotatorDevice.Connected == true)
                            {
                                double currentRotatorPosition = rotatorDevice.Position;
                                double newTargetPosition = currentRotatorPosition + degreesPerSecond;
                                
                                newTargetPosition = newTargetPosition % 360.0;
                                if (newTargetPosition < 0) newTargetPosition += 360.0;

                                if (Math.Abs(newTargetPosition - currentRotatorPosition) > 0.01) 
                                {
                                    // Use the mediator's strongly typed Move method
                                    if (_rotatorMediator != null)
                                    {
                                        _ = Task.Run(() => _rotatorMediator.Move((float)newTargetPosition, token), token);
                                    }
                                }
                            }
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
                }
            }
        }
    }
}