using eUseControl.BusinessLogic.DBModel;
using eUseControl.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using eUseControl.Helpers;
using eUseControl.Domain.Entities.Product;

namespace eUseControl.BusinessLogic.Core
{
    public class AdminApi
    {
        public bool UpdateUserDetails(ULoginData userData)
        {
            using (var db = new UserContext())
            {
                var existingUser = db.Users.Find(userData.Id);
                if (existingUser != null)
                {
                    existingUser.Username = userData.Username;
                    existingUser.Email = userData.Email;
                    existingUser.Password = LoginHelper.HashGen(userData.Password);
                    existingUser.LastLogin = userData.LastLogin;
                    existingUser.LasIp = userData.LasIp;
                    existingUser.Level = userData.Level;

                    try
                    {
                        db.Entry(existingUser).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        return false;
                    }
                }
                return false;
            }
        }


        public void DeleteUser(int userId)
        {
            using (var db = new UserContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }
        }
        public List<ULoginData> FetchAllUsers()
        {
            using (var db = new UserContext())
            {
                return db.Users.ToList();
            }
        }

        public List<Product> FetchAllProducts()
        {
            using (var _db = new ProductContext())
            {
                return _db.Products.ToList();
            }
        }

        public Product GetProductById(int productId)
        {
            using (var _db = new ProductContext())
            {
                return _db.Products.FirstOrDefault(p => p.Id == productId);
            }
        }

        public List<Product> GetAllProductsWithCategories()
        {
            using (var _db = new ProductContext())
            {
                return _db.Products.Include(p => p.Category).ToList();
            }
        }

        public bool AddProduct(Product product)
        {
            using (var _db = new ProductContext())
            {
                _db.Products.Add(product);
                return _db.SaveChanges() > 0; // SaveChanges returns the number of objects written to the database
            }
        }

        public bool UpdateProductDetails(Product product)
        {
            using (var _db = new ProductContext())
            {
                var existingProduct = _db.Products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Description = product.Description;
                    existingProduct.Category = product.Category;
                    _db.Entry(existingProduct).State = EntityState.Modified;
                    return _db.SaveChanges() > 0;
                }
                return false;
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (var _db = new ProductContext())
            {
                var product = _db.Products.Find(productId);
                if (product != null)
                {
                    _db.Products.Remove(product);
                    return _db.SaveChanges() > 0;
                }
                return false;
            }
        }

        public List<Category> FetchAllCategories()
        {
            using (var _db = new CategoryContext())
            {
                return _db.Categories.ToList();
            }
        }

        public Category GetCategoryById(int categoryId)
        {
            using (var _db = new CategoryContext())
            {
                return _db.Categories.FirstOrDefault(c => c.Id == categoryId);
            }
        }

        public bool AddCategory(Category category)
        {
            using (var _db = new CategoryContext())
            {
                _db.Categories.Add(category);
                return _db.SaveChanges() > 0;
            }
        }

        public bool UpdateCategoryDetails(Category category)
        {
            using (var _db = new CategoryContext())
            {
                var existingCategory = _db.Categories.Find(category.Id);
                if (existingCategory != null)
                {
                    existingCategory.Name = category.Name;
                    _db.Entry(existingCategory).State = EntityState.Modified;
                    return _db.SaveChanges() > 0;
                }
                return false;
            }
        }

        public bool DeleteCategory(int categoryId)
        {
            using (var _db = new CategoryContext())
            {
                var category = _db.Categories.Find(categoryId);
                if (category != null)
                {
                    _db.Categories.Remove(category);
                    return _db.SaveChanges() > 0;
                }
                return false;
            }
        }
    }
}
