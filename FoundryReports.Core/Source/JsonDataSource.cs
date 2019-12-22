using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports;
using Newtonsoft.Json;

namespace FoundryReports.Core.Source
{
    public class JsonDataSource : JsonSourceBase, IDataSource
    {
        private const string MoldsPath = "molds.json";

        private const string ProductsPath = "products.json";
        
        private const string CustomerPath = "customer.json";
        
        private IList<Mold> _molds = new List<Mold>();

        private IList<Product> _products = new List<Product>();

        private IList<Customer> _customer = new List<Customer>();

        public IEnumerable<IProduct> Products => _products;

        public IEnumerable<IMold> Molds => _molds;

        public IEnumerable<ICustomer> Customer => _customer;

        public async Task Load()
        {
            await LoadMolds();
            await LoadProducts();
        }

        private async Task LoadMolds()
        {
            var molds = await Deserialize<List<Mold>>(MoldsPath);

            _molds = molds;
        }

        private async Task LoadProducts()
        {
            var products = await Deserialize<List<Product>>(ProductsPath);

            foreach (var product in products)
            {
                // as the data is serialized, Mold instances would not be the same as the ones stored within the file. Therefore the connection is set here, through the name
                foreach (var moldRequirement in product.MoldRequirementList)
                {
                    var correspondingMold = _molds.FirstOrDefault(m => m.Name == moldRequirement.MoldName);
                    if(correspondingMold == null)
                        continue; // continuing here would indicate that the existing data is somehow corrupt. Ideally log and present to user, however for the scope of this project omitted
                    
                    moldRequirement.Mold = correspondingMold;
                }
            }

            _products = products;
        }

        public async Task LoadCustomer()
        {
            var customer = await Deserialize<List<Customer>>(CustomerPath);
            
            foreach (var singleCustomer in customer)
            {
                // as the data is serialized, monthly usage instances would not be the same as the ones stored within the file. Therefore the connection is set here, through the name
                foreach (var monthlyProductReport in singleCustomer.MonthlyProductReportsList)
                {
                    var correspondingProduct = _products.FirstOrDefault(m => m.Name == monthlyProductReport.ProductName);
                    if(correspondingProduct == null)
                        continue; // continuing here would indicate that the existing data is somehow corrupt. Ideally log and present to user, however for the scope of this project omitted
                    
                    monthlyProductReport.ForProduct = correspondingProduct;
                }
            }

            _customer = customer;
        }

        public async Task PersistChanges()
        {
            await WriteFile(MoldsPath, JsonConvert.SerializeObject(_molds, SerializerSettings));
            await WriteFile(ProductsPath, JsonConvert.SerializeObject(_products, SerializerSettings));
            await WriteFile(CustomerPath, JsonConvert.SerializeObject(_customer, SerializerSettings));
        }

        public IMold NewMold()
        {
            var newMold = new Mold();
            _molds.Add(newMold);
            return newMold;
        }

        public void RemoveMold(IMold mold)
        {
            var existingMold = _molds.FirstOrDefault(m => ReferenceEquals(m, mold));
            if (existingMold == null)
                return;

            _molds.Remove(existingMold);
        }

        public IProduct NewProduct()
        {
            var newProduct = new Product();
            _products.Add(newProduct);
            return newProduct;
        }

        public void RemoveProduct(IProduct product)
        {
            var existingProduct = _products.FirstOrDefault(p => ReferenceEquals(p, product));
            if (existingProduct == null)
                return;

            _products.Remove(existingProduct);
        }

        public ICustomer AddCustomer()
        {
            var customer = new Customer();
            _customer.Add(customer);
            return customer;
        }

        public void RemoveCustomer(ICustomer item)
        {
            var existingCustomer = _customer.FirstOrDefault(c => ReferenceEquals(c, item));
            if (existingCustomer != null)
            {
                _customer.Remove(existingCustomer);
            }
        }
    }
}