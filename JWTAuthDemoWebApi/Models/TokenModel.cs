using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi.Models
{
	public class TokenModel
	{
		public string Access_Token { get; set; }
		public string TokenType { get; set; } = "Bearer";

		public DateTime IssuedAt { get; set; }

		public DateTime ExpiresAt { get; set; }
		public string Refresh_Token { get; set; }
	}
}
