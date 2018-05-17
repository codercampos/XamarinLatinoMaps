using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinLatinoMaps.Models;

namespace XamarinLatinoMaps.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private List<CoffeeShop> _items;

        public ICommand PinTappedCommand { get; private set; }

        public List<CoffeeShop> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public MainPageViewModel()
        {
			PinTappedCommand = new Command(() => Application.Current.MainPage.Navigation.PushAsync(new Page()));
            Init();         
        }

        private void Init()
        {
            Items = new List<CoffeeShop>
            {
                new CoffeeShop
                {
                    Id = 1,
                    Name = "Viva Espresso San Benito",
                    Description = "Centro Comercial El Hipodromo 503, San Benito",
                    Latitude = 13.6946923,
                    Longitude = -89.2414103,
                    Rate = 5
                },
                new CoffeeShop
                {
                    Id = 2,
                    Name = "The Coffee Cup Masferrer",
                    Description = "Rendondel Masferrer, Colonia Escalon",
                    Latitude = 13.703869,
                    Longitude = -89.248569,
                    Rate = 5
                },
                new CoffeeShop
                {
                    Id = 3,
                    Name = "El Cafecito Bistro",
                    Description = "Fundacion Emprendedores Por El Mundo, Paseo El Carmen",
                    Latitude = 13.67534,
                    Longitude = -89.2868771,
                    Rate = 5
                }
            };
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
