using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AltAzDeRotator
{
    /* 
     * NINA 3.x discovery works best by exporting IPluginManifest. 
     * Inheriting from PluginBase allows NINA to automatically fill 
     * most properties from the AssemblyInfo attributes we defined.
     */
    [Export(typeof(IPluginManifest))]
    public class DeRotationPlugin : PluginBase
    {
        private DeRotationService? _deRotationService;
        private readonly NINA.Equipment.Interfaces.Mediator.ITelescopeMediator _telescopeMediator;
        private readonly NINA.Equipment.Interfaces.Mediator.IRotatorMediator _rotatorMediator;
        
        [Import]
        private DeRotationViewModel? _viewModel;

        [ImportingConstructor]
        public DeRotationPlugin(
            NINA.Equipment.Interfaces.Mediator.ITelescopeMediator telescopeMediator, 
            NINA.Equipment.Interfaces.Mediator.IRotatorMediator rotatorMediator)
        {
            _telescopeMediator = telescopeMediator;
            _rotatorMediator = rotatorMediator;
        }

        public override Task Initialize()
        {
            try
            {
                Logger.Info("Initializing Alt-Az De-Rotator Plugin...");
                
                // Instantiate and start the background polling service
                if (_viewModel != null)
                {
                    _deRotationService = new DeRotationService(_telescopeMediator, _rotatorMediator, _viewModel);
                    _deRotationService.Start();
                }
                
                Logger.Info("Alt-Az De-Rotator Plugin initialized successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize Alt-Az De-Rotator Plugin: {ex.Message}");
            }
            
            return Task.CompletedTask;
        }

        public override Task Teardown()
        {
            try
            {
                Logger.Info("Tearing down Alt-Az De-Rotator Plugin...");
                
                if (_deRotationService != null)
                {
                    _deRotationService.Stop();
                    _deRotationService = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during teardown of Alt-Az De-Rotator Plugin: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
