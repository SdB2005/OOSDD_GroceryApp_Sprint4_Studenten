using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            // Haal alle boodschappenlijst-items op uit de repository
            var items = _groceriesRepository.GetAll();

            // Groepeer items op ProductId en bereken het totaal verkochte aantal per product
            var topProducts = items
                .GroupBy(i => i.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalSold = group.Sum(item => item.Amount)
                })
                .OrderByDescending(x => x.TotalSold) // Sorteer aflopend op aantal verkopen
                .Take(topX); // Neem de top X producten

            var bestSellers = new List<BestSellingProducts>();
            int rank = 1;

            // Voor elk product in de top, haal het product op en voeg toe aan de lijst met bestsellers
            foreach (var productInfo in topProducts)
            {
                var product = _productRepository.Get(productInfo.ProductId);
                if (product != null)
                {
                    bestSellers.Add(new BestSellingProducts(
                        productInfo.ProductId,
                        product.Name,
                        product.Stock,
                        productInfo.TotalSold,
                        rank // Rangnummer
                    ));
                }
                rank++; // Rangnummer verhogen, ongeacht of product bestaat
            }
            // Retourneer de lijst met best verkochte producten
            return bestSellers;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
