using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Maps;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace XamarinLatinoMaps.UWP
{
    public sealed partial class MapCustomLayout : UserControl
    {
        private Pin _pin;
        public MapCustomLayout(Pin pin)
        {
            this.InitializeComponent();
            _pin = pin;
            SetupData();
        }

        private void SetupData()
        {
            Label.Text = _pin.Label;
            Address.Text = _pin.Address;
        }
    }
}
