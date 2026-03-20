using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Plugin.ManifestDefinition;
using NINA.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AltAzDeRotator
{
    [Export(typeof(IPluginManifest))]
    public class DeRotationPlugin : IPluginManifest
    {
        private DeRotationService? _deRotationService;
        
        [Import]
        private NINA.Equipment.Interfaces.Mediator.ITelescopeMediator? _telescopeMediator;
        
        [Import]
        private NINA.Equipment.Interfaces.Mediator.IRotatorMediator? _rotatorMediator;
        
        [Import]
        private DeRotationViewModel? _viewModel;

        public DeRotationPlugin()
        {
        }

        // --- IPluginManifest Implementation ---

        public string Identifier => "AltAz.DeRotator";
        public string Name => "Alt-Az De-Rotator";
        public string Author => "ArchitectJuan";
        public string Description => "Actively compensates for field rotation on Alt-Azimuth mounts.";
        
        public IPluginVersion Version => new PluginVersion("1.1.0.1");
        public IPluginVersion MinimumApplicationVersion => new PluginVersion("3.0.0.9001");

        public string LicenseURL => "https://opensource.org/licenses/MPL-2.0";
        public string Homepage => "https://github.com/ArchitectJuan/Nina_Rotator_Plugin";
        public string Repository => "https://github.com/ArchitectJuan/Nina_Rotator_Plugin.git";
        public string ChangelogURL => "https://github.com/ArchitectJuan/Nina_Rotator_Plugin/releases";
        public string License => "MPL-2.0";
        
        public string[] Tags => new string[] { "equipment", "rotator", "alt-az" };

        public IPluginDescription Descriptions => null;
        public IPluginInstallerDetails Installer => null;

        public Task Initialize()
        {
            try
            {
                Logger.Info("Initializing Alt-Az De-Rotator Plugin (V1.1.0.1)...");
                
                if (_viewModel != null && _telescopeMediator != null && _rotatorMediator != null)
                {
                    _deRotationService = new DeRotationService(_telescopeMediator, _rotatorMediator, _viewModel);
                    _deRotationService.Start();
                }
                else 
                {
                    string missing = "";
                    if (_viewModel == null) missing += "ViewModel ";
                    if (_telescopeMediator == null) missing += "TelescopeMediator ";
                    if (_rotatorMediator == null) missing += "RotatorMediator ";
                    Logger.Error($"DeRotationPlugin: Initialization failed. Missing imports: {missing}");
                }
                
                Logger.Info("Alt-Az De-Rotator Plugin initialized successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize Alt-Az De-Rotator Plugin: {ex.Message}");
            }
            
            return Task.CompletedTask;
        }

        public Task Teardown()
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
