using JWTAuthDemoWebApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private List<RoleModel> _roles = new List<RoleModel>();

        private List<UserModel> _usersRecords = new List<UserModel>();

        public UserRepository()
        {
            FillRoles();
            FillUsersRecords();
        }

        private void FillRoles()
        {
            _roles.Add(new RoleModel()
            {
                Id = (int)RoleEnum.Admin,
                Name = RoleEnum.Admin.ToString()
            });

            _roles.Add(new RoleModel()
            {
                Id = (int)RoleEnum.Employee,
                Name = RoleEnum.Employee.ToString()
            });

            _roles.Add(new RoleModel()
            {
                Id = (int)RoleEnum.Student,
                Name = RoleEnum.Student.ToString()
            });
        }

        private void FillUsersRecords()
        {
            _usersRecords.Add(new UserModel()
            {
                Name = "User 1",
                Email = "User1@gmail.com",
                UserName = "user1",
                Password = "password1",
                Roles = _roles.Where(x => x.Id == (int)RoleEnum.Admin || x.Id == (int)RoleEnum.Employee).ToList()
            });
            _usersRecords.Add(new UserModel()
            {
                Name = "User 2",
                Email = "User2@gmail.com",
                UserName = "user2",
                Password = "password2",
                Roles = _roles.Where(x => x.Id == (int)RoleEnum.Employee).ToList()
            });
            _usersRecords.Add(new UserModel()
            {
                Name = "User 3",
                Email = "User3@gmail.com",
                UserName = "user3",
                Password = "password3",
                Roles = _roles.Where(x => x.Id == (int)RoleEnum.Student).ToList()
            });
        }

        public List<UserModel> GetAllUsers()
        {
            return _usersRecords;
        }

        public UserModel Authenticate(AuthenticationRequestModel users)
        {
            var user = _usersRecords
                .Where(x => x.UserName == users.Username && x.Password == users.Password)
                .FirstOrDefault();

            return user;

            //// Else we generate JSON Web Token
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //                {
            //                    new Claim(ClaimTypes.Name, users.Name)
            //                }),
            //    Expires = DateTime.UtcNow.AddMinutes(10),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return new Tokens { Token = tokenHandler.WriteToken(token) };

        }
    }
}
