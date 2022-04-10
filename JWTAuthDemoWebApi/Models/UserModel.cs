using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi.Models
{
	public class UserModel
	{
		public string Name { get; set; }
		public string Password { get; set; }

		public string UserName { get; set; }
		public string Email { get; set; }
		public List<RoleModel> Roles { get; set; }
	}
}
