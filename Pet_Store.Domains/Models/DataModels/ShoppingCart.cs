﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pet_Store.Domains.Models.DataModels
{
    public class ShoppingCart
    {


        [Key]
        public int Id { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public Products Product { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public Users User { get; set; }

        public double Subtotal { get; set; }

    }
}
