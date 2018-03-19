using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace XamarinLatinoMaps.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Plugin initialization here
            Xamarin.FormsMaps.Init("fugTOjzqZ4SnhpyODgwS~EicHDmhjc-1-qFFRcnRLag~AubgjSbPJ-WHHW2Q2sFpREnx0TE0hcqOkwJUX01WFdxDEFeUDD2TAF0CQqtEb0jf");

            LoadApplication(new XamarinLatinoMaps.App());
        }
    }
}
