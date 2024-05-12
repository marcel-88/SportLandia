using eUseControl.Domain.Entities.Product;
using eUseControl.Domain.Entities.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eUseControl.BusinessLogic.Interfaces
{
    public interface ISession
    {
        List<Review> GetReviewsByProductId(int productId);
        bool UserSubmitReview(Review data);
        ULoginResp UserLogin(UserLogin data);
        ULoginResp UserRegister(UserRegister data);
        HttpCookie GenCookie(string loginCredential);
        UserMinimal GetUserByCookie(string apiCookieValue);
        Session GetSessionByCookie(string token);
        void LogoutUserByCookie(string token, HttpContextBase httpContext);
        bool ChangeUserPassword(string username, string oldPassword, string newPassword);
        bool UpdateUser(ULoginData user);
        void DeleteUser(int userId);
        List<ULoginData> GetAllUsers();

        List<Product> GetAllProducts();
        Product GetProductById(int productId);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int productId);
        List<Product> GetAllProductsIncludingCategories();

        List<Category> GetAllCategories();
        Category GetCategoryById(int categoryId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int categoryId);
    }
}
