using System.Collections.Generic;
using System.Text.Json.Serialization;
using DNTFrameworkCore.Cqrs.Commands;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Commands
{
    public class CreateProductCommand : ICommand
    {
        public string Title { get; }
        public IEnumerable<ProductPriceDTO> Prices { get; }

        [JsonConstructor]
        public CreateProductCommand(string title, IEnumerable<ProductPriceDTO> prices)
        {
            Title = title;
            Prices = prices;
        }
    }

    public class ProductPriceDTO
    {
        public long PriceTypeId { get; }
        public decimal Price { get; }

        [JsonConstructor]
        public ProductPriceDTO(long priceTypeId, decimal price)
        {
            PriceTypeId = priceTypeId;
            Price = price;
        }
    }
}