using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            var result = new List<BoughtProducts>();

            if (productId == null)
                return result;

            // Haal alle items op die dit product bevatten
            var items = _groceryListItemsRepository.GetAll()
                .Where(item => item.ProductId == productId)
                .ToList();

            foreach (var item in items)
            {
                // Haal de boodschappenlijst op
                var groceryList = _groceryListRepository.Get(item.GroceryListId);
                if (groceryList == null)
                    continue;

                // Haal de klant op die deze boodschappenlijst heeft
                var client = _clientRepository.Get(groceryList.ClientId);
                if (client == null)
                    continue;

                // Haal het product op
                var product = _productRepository.Get(item.ProductId);
                if (product == null)
                    continue;

                // Voeg toe aan resultaat
                result.Add(new BoughtProducts(client, groceryList, product));
            }

            return result;
        }
    }
}
