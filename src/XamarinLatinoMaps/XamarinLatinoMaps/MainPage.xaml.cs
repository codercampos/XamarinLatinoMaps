using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace XamarinLatinoMaps
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		    MapView.MoveToRegion(
		        MapSpan.FromCenterAndRadius(
		            new Position(37, -122), Distance.FromMiles(1)));
        }

	    private void Street_OnClicked(object sender, EventArgs e)
	    {
	        MapView.MapType = MapType.Street;
	    }


	    private void Hybrid_OnClicked(object sender, EventArgs e)
	    {
	        MapView.MapType = MapType.Hybrid;
	    }

	    private void Satellite_OnClicked(object sender, EventArgs e)
	    {
	        MapView.MapType = MapType.Satellite;
	    }
	}
}
