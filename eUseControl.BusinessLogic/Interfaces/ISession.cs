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
    }
}
