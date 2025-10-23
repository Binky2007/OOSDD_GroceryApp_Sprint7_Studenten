using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.Generic;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;

        public BoughtProductsService(
            IGroceryListItemsRepository groceryListItemsRepository,
            IGroceryListRepository groceryListRepository,
            IClientRepository clientRepository,
            IProductRepository productRepository)
        {
            _groceryListItemsRepository = groceryListItemsRepository;
            _groceryListRepository = groceryListRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }

        public List<BoughtProducts> Get(int? productId)
        {
            List<BoughtProducts> boughtProductsList = new List<BoughtProducts>();
            // haal alle klanten op
            List<Client> allClients = _clientRepository.GetAll();
            // haal alle boodschappenlijsten op
            List<GroceryList> allLists = _groceryListRepository.GetAll();
            
            foreach (Client client in allClients)
            {
                foreach (GroceryList groceryList in allLists)
                {
                    if (groceryList.ClientId != client.Id)
                    {
                        continue;
                    }

                    List<GroceryListItem> items = _groceryListItemsRepository.GetAllOnGroceryListId(groceryList.Id);

                    foreach (GroceryListItem item in items)
                    {
                        if (item.Product.Id == productId)
                        {
                            BoughtProducts boughtProduct = new BoughtProducts(client, groceryList, item.Product);
                            boughtProductsList.Add(boughtProduct);
                        }
                    }
                }
            }

            return boughtProductsList;
        }


    }
}
