using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroORM
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal StockPrice { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductRepository
    {
        string connectionString = "server=.;database=ZeroOrmExample;persist security info=True;Integrated Security=SSPI";
        public void Create(Product entity)
        {
            var ProductInsertSql = "INSERT INTO Products (Name, Description, UnitPrice, Quantity, StockPrice, IsActive) VALUES (@Name, @Description, @UnitPrice, @Quantity, @StockPrice, @IsActive)";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(ProductInsertSql, connection))
                {
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@UnitPrice", entity.UnitPrice);
                    command.Parameters.AddWithValue("@Quantity", entity.Quantity);
                    command.Parameters.AddWithValue("@StockPrice", entity.StockPrice);
                    command.Parameters.AddWithValue("@IsActive", entity.IsActive);
                    int erc = command.ExecuteNonQuery();
                    Debug.Assert(erc > 0);
                }
            }
        }

        public async Task CreateAsync(Product entity, CancellationToken cancellationToken)
        {
            var ProductInsertSql = "INSERT INTO Products (Name, Description, UnitPrice, Quantity, StockPrice, IsActive) VALUES (@Name, @Description, @UnitPrice, @Quantity, @StockPrice, @IsActive)";
            int erc = await SqlHelper.ExecuteNonQueryAsync(connectionString, ProductInsertSql, entity, cancellationToken);
            Debug.Assert(erc > 0);
        }

        public async Task<List<Product>> FindByQuantityAsync(decimal quantity, CancellationToken cancellationToken)
        {
            var ProductSelectByQuantitySql = "SELECT * FROM Products WHERE (Quantity = @Quantity)";
            using var reader = await SqlHelper.ExecuteReaderAsync(connectionString, ProductSelectByQuantitySql, quantity, cancellationToken);
            return await reader.ToListAsync<Product>(cancellationToken);
        }

        public List<Product> FindByQuantity(decimal quantity)
        {
            var ProductSelectByQuantitySql = "SELECT * FROM Products WHERE (Quantity = @Quantity)";
            List<Product> products = new List<Product>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(ProductSelectByQuantitySql, connection))
                {
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                                StockPrice = reader.GetDecimal(reader.GetOrdinal("StockPrice")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                            };

                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }
    }
}
