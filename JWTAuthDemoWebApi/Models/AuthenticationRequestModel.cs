using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi.Models
{
	public class AuthenticationRequestModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
