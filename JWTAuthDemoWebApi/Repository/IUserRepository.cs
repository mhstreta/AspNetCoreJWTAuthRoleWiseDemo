using JWTAuthDemoWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi.Repository
{
    public interface IUserRepository
    {
        List<UserModel> GetAllUsers();
        UserModel Authenticate(AuthenticationRequestModel users);
    }
}
