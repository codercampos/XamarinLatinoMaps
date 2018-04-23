using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;
using XamarinLatinoMaps.Framework.Renderers;
using XamarinLatinoMaps.UWP.Renderers;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace XamarinLatinoMaps.UWP.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        private MapControl _nativeMap;
        private List<Pin> _pins;
        private MapCustomLayout _mapOverlay;
        private bool _showOverlay;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                _nativeMap.MapElementClick -= OnMapElementClick;
                _nativeMap.Children.Clear();
                _nativeMap = null;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                _nativeMap = Control;
                _pins = formsMap.Pins.ToList();

                _nativeMap.Children.Clear();
                _nativeMap.MapElementClick += OnMapElementClick;

                foreach (var pin in _pins)
                {
                    var snPosition = new BasicGeoposition { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    var mapIcon = new MapIcon
                    {
                        Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///ic_coffee_bean.png")),
                        CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                        Location = snPoint,
                        NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0),
                    };

                    _nativeMap.MapElements.Add(mapIcon);
                }
            }
        }

        private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            if (mapIcon != null)
            {
                if (!_showOverlay)
                {
                    var customPin = GetCustomPin(mapIcon.Location.Position);
                    if (customPin == null)
                    {
                        throw new Exception("Custom pin not found");
                    }

                    if (_mapOverlay == null)
                    {
                        _mapOverlay = new MapCustomLayout(customPin);
                    }

                    var snPosition = new BasicGeoposition { Latitude = customPin.Position.Latitude, Longitude = customPin.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    _nativeMap.Children.Add(_mapOverlay);
                    MapControl.SetLocation(_mapOverlay, snPoint);
                    MapControl.SetNormalizedAnchorPoint(_mapOverlay, new Windows.Foundation.Point(0.5, 1.0));
                    _showOverlay = true;
                }
                else
                {
                    _nativeMap.Children.Remove(_mapOverlay);
                    _showOverlay = false;
                }
            }
        }

        private Pin GetCustomPin(BasicGeoposition position)
        {
            var pos = new Position(position.Latitude, position.Longitude);
            foreach (var pin in _pins)
            {
                if (pin.Position == pos)
                {
                    return pin;
                }
            }
            return null;
        }
    }
}
