﻿namespace WebPizza.ViewModels.Category
{
    public class CategoryUpdateVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public IFormFile Image { get; set; } = null!;
    }
}
