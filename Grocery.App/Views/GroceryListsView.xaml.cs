using Grocery.App.ViewModels;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.App.Views;

public partial class GroceryListsView : ContentPage
{
    public GroceryListsView(IGroceryListService groceryListService, GlobalViewModel global)
    {
        InitializeComponent();
        BindingContext = new GroceryListViewModel(groceryListService, global.Client);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is GroceryListViewModel bindingContext)
        {
            bindingContext.OnAppearing();

        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is GroceryListViewModel bindingContext)
        {
            bindingContext.OnDisappearing();
        }
    }
}