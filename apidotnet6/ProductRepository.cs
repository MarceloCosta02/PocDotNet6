namespace apidotnet6
{
    public static class ProductRepository
    {
        public static List<Product> Products { get; set; } = new List<Product>();

        //public static void Add(ProductRequest product)
        //{           
        //    Products.Add(product);
        //}

        public static List<Product> GetAll()
        {
            return Products.ToList();
        }

        public static Product GetBy(string code)
        {
            return Products.FirstOrDefault(p => p.Code == code);
        }

        public static void Remove(Product product)
        {
            Products.Remove(product);
        }
    }

}
