﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_PetStore.API.Models.DataModels
{
    public class ShoppingCart
    {

        public ShoppingCart()
        {

        }
        public ShoppingCart(int id, int count, double subtotal)
        {
            Id = id;
            Count = count;
            Subtotal = subtotal;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int Count { get; set; }

        [ForeignKey("ProductId")]
        public Products Product { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; }

        //[NotMapped] 
        //[Required]
        public double Subtotal { get; set; }
        public double Total { get; set; }


    }
}
