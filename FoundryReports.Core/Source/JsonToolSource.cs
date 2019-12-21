using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FoundryReports.Core.Products;
using Newtonsoft.Json;

namespace FoundryReports.Core.Source
{
    public class JsonToolSource : IToolSource
    {
        private const string MoldsPath = "molds.json";

        private const string ProductsPath = "products.json";

        private IList<Mold> _molds = new List<Mold>();

        private IList<Product> _products = new List<Product>();

        private static JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
            {TypeNameHandling = TypeNameHandling.Objects, Formatting = Formatting.Indented};

        public async IAsyncEnumerable<IProduct> LoadProducts()
        {
            var products = await Deserialize<List<Product>>(ProductsPath);

            _products = products;

            foreach (var product in products)
            {
                yield return product;
            }
        }

        public async IAsyncEnumerable<IMold> LoadMolds()
        {
            var molds = await Deserialize<List<Mold>>(MoldsPath);

            _molds = molds;

            foreach (var mold in molds)
            {
                yield return mold;
            }
        }

        private async Task<T> Deserialize<T>(string filePath) where T : class, new()
        {
            var content = await GetOrCreateFile(filePath);
            T? result = null;
            try
            {
                result = JsonConvert.DeserializeObject<T>(content, SerializerSettings);
            }
            catch (Exception)
            {
                // log exception here. (which is omitted within the scope of this project)
            }

            if (result == null)
            {
                result = new T();
                await WriteFile(filePath, JsonConvert.SerializeObject(result, SerializerSettings));
            }

            return result;
        }

        public async Task PersistChanges()
        {
            await Task.Delay(500);
            await WriteFile(MoldsPath, JsonConvert.SerializeObject(_molds, SerializerSettings));
            await WriteFile(ProductsPath, JsonConvert.SerializeObject(_molds, SerializerSettings));
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

        private async Task WriteFile(string path, string content)
        {
            try
            {
                await Task.Delay(2500);
                await File.WriteAllTextAsync(path, content);
            }
            catch (Exception)
            {
                // log exception here. (which is omitted within the scope of this project)
            }
        }

        private async Task<string> GetOrCreateFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    return await File.ReadAllTextAsync(path);

                File.Create(path);

                return string.Empty;
            }
            catch (Exception)
            {
                // log exception here. (which is omitted within the scope of this project)
                return string.Empty;
            }
        }
    }
}