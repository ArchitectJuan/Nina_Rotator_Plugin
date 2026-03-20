using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace AltAzDeRotator
{
    [Export]
    public partial class DeRotationSettingsView : UserControl
    {
        public DeRotationSettingsView()
        {
            InitializeComponent();
        }
    }
}
