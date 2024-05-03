using eUseControl.BusinessLogic.Core;
using eUseControl.BusinessLogic.DBModel;
using eUseControl.BusinessLogic.Interfaces;
using eUseControl.Domain.Entities.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eUseControl.BusinessLogic
{
    public class SessionBL : UserApi, ISession
    {
        public ULoginResp UserLogin(UserLogin data)
        {
            return UserLoginAction(data);
        }

        public ULoginResp UserRegister(UserRegister data)
        {
            return UserRegisterAction(data);
        }

        public HttpCookie GenCookie(string loginCredential)
        {
            return Cookie(loginCredential);
        }

        public UserMinimal GetUserByCookie(string apiCookieValue)
        {
            return UserCookie(apiCookieValue);
        }

        public bool UpdateUser(ULoginData user)
        {
            var adminApi = new AdminApi();
            return adminApi.UpdateUserDetails(user);
        }

        public void DeleteUser(int userId)
        {
            var adminApi = new AdminApi();
            adminApi.DeleteUser(userId);
        }
        public List<ULoginData> GetAllUsers()
        {
            var adminApi = new AdminApi();
            return adminApi.FetchAllUsers();
        }
    }
}

