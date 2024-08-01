﻿using MicroMarket.Services.Catalog.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicroMarket.Services.Catalog.Dtos
{
    public class CategoryUpdateRequestDto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }
    }
}
