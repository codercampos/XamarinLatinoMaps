using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using MapKit;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using XamarinLatinoMaps.Framework.Renderers;
using XamarinLatinoMaps.iOS.Renderers;

[assembly:ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace XamarinLatinoMaps.iOS.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        private UIView customPinView;
        private List<Pin> pins;
		object _lastTouchedView;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // Unsubscribe from events
                if (Control is MKMapView nativeMap)
                {
                    nativeMap.RemoveAnnotations(nativeMap.Annotations);
                    nativeMap.GetViewForAnnotation = null;
                    nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                    nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                    nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
                }
            }

            if (e.NewElement != null)
            {
				// Subscribe to events and configure the native control to be used
				_lastTouchedView = null;
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                pins = formsMap.Pins.ToList();

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }

            
        }

        MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            if (IsUserLocation(mapView, annotation))
                return null;

            var customPin = GetCustomPin(annotation);
            if (customPin == null)
            {
                return null;
                //throw new Exception("Custom pin not found");
            }

            annotationView = mapView.DequeueReusableAnnotation(customPin.Id.ToString());
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.Id.ToString());
                annotationView.Image = UIImage.FromFile("ic_coffee_bean.png");
                annotationView.CalloutOffset = new CGPoint(0, 0);
                ((CustomMKAnnotationView)annotationView).Id = customPin.Id.ToString();
            }
			annotationView.Annotation = annotation;
			annotationView.CanShowCallout = true;
			AttachGestureToPin(annotationView, annotation);

            return annotationView;
        }


		protected void AttachGestureToPin(MKAnnotationView mapPin, IMKAnnotation annotation)
        {
            var recognizers = mapPin.GestureRecognizers;

            if (recognizers != null)
            {
                foreach (var r in recognizers)
                {
                    mapPin.RemoveGestureRecognizer(r);
                }
            }

#if __MOBILE__
            var recognizer = new UITapGestureRecognizer(g => OnClick(annotation, g))
            {
                ShouldReceiveTouch = (gestureRecognizer, touch) =>
                {
                    _lastTouchedView = touch.View;
                    return true;
                }
            };
#else
            var recognizer = new NSClickGestureRecognizer(g => OnClick(annotation, g));
#endif
            mapPin.AddGestureRecognizer(recognizer);
        }



#if __MOBILE__
            void OnClick(object annotationObject, UITapGestureRecognizer recognizer)
#else
            void OnClick(object annotationObject, NSClickGestureRecognizer recognizer)
#endif
            {
            // https://bugzilla.xamarin.com/show_bug.cgi?id=26416
            NSObject annotation = Runtime.GetNSObject(((IMKAnnotation)annotationObject).Handle);
            if (annotation == null)
                return;

            // lookup pin
            Pin targetPin = null;
            foreach (Pin pin in ((Map)Element).Pins)
            {
                object target = pin.Id;
                if (target != annotation)
                    continue;

                targetPin = pin;
                break;
            }

            // pin not found. Must have been activated outside of forms
            if (targetPin == null)
                return;

            // if the tap happened on the annotation view itself, skip because this is what happens when the callout is showing
            // when the callout is already visible the tap comes in on a different view
            if (_lastTouchedView is MKAnnotationView)
                return;

            targetPin.SendTap();
        }

        void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
        }

        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            customPinView = new UIView();
            // TODO Do something if you want to add extra information on the annotation view. Example: a logo or URL image
        }

        private void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (e.View.Selected) return;
            customPinView.RemoveFromSuperview();
            customPinView.Dispose();
            customPinView = null;
        }

        // TODO make this index-like search
        private Pin GetCustomPin(IMKAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            foreach (var pin in pins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }

        private bool IsUserLocation(MKMapView mapView, IMKAnnotation annotation)
        {
            var userLocationAnnotation = ObjCRuntime.Runtime.GetNSObject(annotation.Handle) as MKUserLocation;
            if (userLocationAnnotation != null)
            {
                return userLocationAnnotation == mapView.UserLocation;
            }

            return false;
        }
    }

    public class CustomMKAnnotationView : MKAnnotationView
    {
        public string Id { get; set; }

        public CustomMKAnnotationView(IMKAnnotation annotation, string id) : base(annotation, id)
        {

        }
    }
}