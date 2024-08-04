using MicroMarket.Services.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Catalog.DbContexts
{
    public static class CatalogDataSeeder
    {

        public static async Task SeedData(CatalogDbContext context)
        {
            var isPresented = await context.Categories.Where(c => c.Name == "Action").AnyAsync();
            if (!isPresented)
            {
                await context.Categories.AddRangeAsync(
                    new Category()
                    {
                        Name = "Action",
                        Description = "Welcome to the Action Games category! Dive into the most thrilling and dynamic games that will give you an adrenaline rush and unforgettable experiences. From epic battles to high-speed chases, our action games offer non-stop excitement.",
                        ParentCategoryId = null,
                        ChildCategories = new List<Category>
                        {
                            new Category(){
                                Name = "First-Person Shooters",
                                Description = "Experience intense first-person shooter action with these thrilling games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game A",
                                        Description = "An exhilarating first-person shooter game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 59.99m,
                                        StockQuantity = 100,
                                        Weight = 0.5m,
                                        Dimensions = "10x10x2 cm",
                                        Brand = "Brand A",
                                        Manufacturer = "Manufacturer A",
                                        DiscountPrice = 49.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(10),
                                        IsActive = true,
                                        IsDeleted = false
                                    },
                                    new Product(){
                                        Name = "Game B",
                                        Description = "A popular first-person shooter game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 39.99m,
                                        StockQuantity = 200,
                                        Weight = 0.4m,
                                        Dimensions = "9x9x2 cm",
                                        Brand = "Brand B",
                                        Manufacturer = "Manufacturer B",
                                        DiscountPrice = 29.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(5),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            },
                            new Category(){
                                Name = "Platformers",
                                Description = "Jump, run, and explore in these exciting platformer games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game C",
                                        Description = "A fun and challenging platformer game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 29.99m,
                                        StockQuantity = 150,
                                        Weight = 0.3m,
                                        Dimensions = "8x8x2 cm",
                                        Brand = "Brand C",
                                        Manufacturer = "Manufacturer C",
                                        DiscountPrice = 19.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(7),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            }
                        },
                        Products = new List<Product>()
                    },
                    new Category()
                    {
                        Name = "RPG",
                        Description = "Immerse yourself in rich narratives and character development in our RPG Games category. Customize your hero and embark on epic quests.",
                        ParentCategoryId = null,
                        ChildCategories = new List<Category>
                        {
                            new Category(){
                                Name = "Fantasy RPG",
                                Description = "Enter magical worlds and embark on epic quests in these fantasy RPG games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game D",
                                        Description = "A captivating fantasy RPG game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 49.99m,
                                        StockQuantity = 80,
                                        Weight = 0.6m,
                                        Dimensions = "11x11x2 cm",
                                        Brand = "Brand D",
                                        Manufacturer = "Manufacturer D",
                                        DiscountPrice = 39.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(12),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            },
                            new Category(){
                                Name = "Sci-Fi RPG",
                                Description = "Explore futuristic worlds and technologies in these sci-fi RPG games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game E",
                                        Description = "A thrilling sci-fi RPG game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 59.99m,
                                        StockQuantity = 70,
                                        Weight = 0.7m,
                                        Dimensions = "12x12x2 cm",
                                        Brand = "Brand E",
                                        Manufacturer = "Manufacturer E",
                                        DiscountPrice = 49.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(8),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            }
                        },
                        Products = new List<Product>()
                    },
                    new Category()
                    {
                        Name = "Strategy",
                        Description = "Test your tactical skills and strategic thinking in our Strategy Games category. Plan, manage resources, and lead your forces to victory.",
                        ParentCategoryId = null,
                        ChildCategories = new List<Category>
                        {
                            new Category(){
                                Name = "Real-Time Strategy",
                                Description = "Engage in real-time battles and manage your resources efficiently in these RTS games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game F",
                                        Description = "A strategic real-time strategy game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 39.99m,
                                        StockQuantity = 90,
                                        Weight = 0.5m,
                                        Dimensions = "10x10x2 cm",
                                        Brand = "Brand F",
                                        Manufacturer = "Manufacturer F",
                                        DiscountPrice = 29.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(10),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            },
                            new Category(){
                                Name = "Turn-Based Strategy",
                                Description = "Plan your moves carefully and outsmart your opponents in these turn-based strategy games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game G",
                                        Description = "A challenging turn-based strategy game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 29.99m,
                                        StockQuantity = 110,
                                        Weight = 0.4m,
                                        Dimensions = "9x9x2 cm",
                                        Brand = "Brand G",
                                        Manufacturer = "Manufacturer G",
                                        DiscountPrice = 19.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(7),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            }
                        },
                        Products = new List<Product>()
                    },
                    new Category()
                    {
                        Name = "Simulation",
                        Description = "Experience life-like scenarios and control various aspects of reality in our Simulation Games category. From managing cities to piloting aircraft, the possibilities are endless.",
                        ParentCategoryId = null,
                        ChildCategories = new List<Category>
                        {
                            new Category(){
                                Name = "Life Simulation",
                                Description = "Simulate real-life scenarios and control the lives of virtual characters in these life simulation games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game H",
                                        Description = "A popular life simulation game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 49.99m,
                                        StockQuantity = 130,
                                        Weight = 0.5m,
                                        Dimensions = "10x10x2 cm",
                                        Brand = "Brand H",
                                        Manufacturer = "Manufacturer H",
                                        DiscountPrice = 39.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(15),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            },
                            new Category(){
                                Name = "Flight Simulation",
                                Description = "Experience the thrill of flying with these realistic flight simulation games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game I",
                                        Description = "A highly realistic flight simulation game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 59.99m,
                                        StockQuantity = 50,
                                        Weight = 0.6m,
                                        Dimensions = "11x11x2 cm",
                                        Brand = "Brand I",
                                        Manufacturer = "Manufacturer I",
                                        DiscountPrice = 49.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(10),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            }
                        },
                        Products = new List<Product>()
                    },
                    new Category()
                    {
                        Name = "Sports",
                        Description = "Get in the game with our Sports Games category. From football to racing, enjoy realistic sports simulations and compete for the championship.",
                        ParentCategoryId = null,
                        ChildCategories = new List<Category>
                        {
                            new Category(){
                                Name = "Football",
                                Description = "Play the most popular sport in the world with these football simulation games.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game J",
                                        Description = "A highly realistic football simulation game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 49.99m,
                                        StockQuantity = 120,
                                        Weight = 0.5m,
                                        Dimensions = "10x10x2 cm",
                                        Brand = "Brand J",
                                        Manufacturer = "Manufacturer J",
                                        DiscountPrice = 39.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(7),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            },
                            new Category(){
                                Name = "Racing",
                                Description = "Rev up your engines and race to the finish line in our Racing Games category. Experience high-speed thrills and intense competition.",
                                ChildCategories = new List<Category>(),
                                Products = new List<Product>
                                {
                                    new Product(){
                                        Name = "Game K",
                                        Description = "A thrilling racing game.",
                                        CategoryId = Guid.NewGuid(), // This will be set automatically by EF
                                        Price = 39.99m,
                                        StockQuantity = 100,
                                        Weight = 0.5m,
                                        Dimensions = "10x10x2 cm",
                                        Brand = "Brand K",
                                        Manufacturer = "Manufacturer K",
                                        DiscountPrice = 29.99m,
                                        DiscountStartDate = DateTime.UtcNow,
                                        DiscountEndDate = DateTime.UtcNow.AddDays(10),
                                        IsActive = true,
                                        IsDeleted = false
                                    }
                                }
                            }
                        },
                        Products = new List<Product>()
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
