using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.App.Views;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        private readonly IGroceryListService _groceryListService;
        private readonly GlobalViewModel _global;

        public ObservableCollection<GroceryList> GroceryLists { get; set; } = new();

        public Client Client => _global.Client;

        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel global)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _global = global;

            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> parameter = new()
            {
                { nameof(GroceryList), groceryList }
            };

            await Shell.Current.GoToAsync($"{nameof(GroceryListItemsView)}?Titel={groceryList.Name}", true, parameter);
        }

        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            if (Client?.Role == Role.Admin)
            {
                await Shell.Current.GoToAsync(nameof(BoughtProductsView));
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Geen toegang", "Alleen admins mogen deze pagina zien.", "OK");
            }
        }
    }
}