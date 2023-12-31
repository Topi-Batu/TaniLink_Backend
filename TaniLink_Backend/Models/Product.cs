﻿using System.ComponentModel.DataAnnotations.Schema;

namespace TaniLink_Backend.Models
{
    public class Product : Auditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long Price { get; set; }
        public string? UnitName { get; set; }
        public int AvailableStock { get; set; }
        public int Sold { get; set; }
        public Commodity? Commodity { get; set; }
        public Area? Area { get; set; }
        public Seller? Seller { get; set; }
        public ICollection<ProductImage>? Images { get; set; }
        public ICollection<ShoppingCart>? ShoppingCarts { get; set; }

    }
}
