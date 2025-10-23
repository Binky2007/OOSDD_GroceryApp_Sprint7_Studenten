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
            // pak alle producten uit alle boodschappenlijsten
            List<GroceryListItem> allGroceryItems = _groceriesRepository.GetAll();

            List<IGrouping<int, GroceryListItem>> groupedByProduct = allGroceryItems.GroupBy(item => item.ProductId).ToList();

            List<(int ProductId, int TotalSold)> productCountsList = new List<(int ProductId, int TotalSold)>();
            foreach (IGrouping<int, GroceryListItem> group in groupedByProduct)
            {
                int totalSold = 0;
                foreach (GroceryListItem item in group)
                {
                    totalSold += item.Amount; 
                }
                productCountsList.Add((group.Key, totalSold));
            }

            productCountsList.Sort((a, b) => b.TotalSold.CompareTo(a.TotalSold));
            List<(int ProductId, int TotalSold)> topProducts = new List<(int ProductId, int TotalSold)>();
            int counter = 0;
            foreach ((int ProductId, int TotalSold) entry in productCountsList)
            {
                if (counter >= topX)
                    break;

                topProducts.Add(entry);
                counter++;
            }

            // Haal alle producten op
            List<Product> allProducts = _productRepository.GetAll();

            // Maak lijst van BestSellingProducts
            List<BestSellingProducts> bestSellingProducts = new List<BestSellingProducts>();
            int ranking = 1;
            foreach ((int ProductId, int TotalSold) entry in topProducts)
            {
                Product? product = allProducts.FirstOrDefault(p => p.Id == entry.ProductId);
                if (product != null)
                {
                    BestSellingProducts bestProduct = new BestSellingProducts(
                        product.Id,
                        product.name,
                        product.Stock,
                        entry.TotalSold,
                        ranking++
                    );
                    bestSellingProducts.Add(bestProduct);
                }
            }

            return bestSellingProducts;
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
