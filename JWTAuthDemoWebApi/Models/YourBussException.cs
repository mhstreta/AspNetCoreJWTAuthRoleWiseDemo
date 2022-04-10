using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace JWTAuthDemoWebApi.Models
{
	public class YourBussException : Exception
	{
		public string Type { get; } = "http://your.business.com/rfc7807/businessexception";
		public string Title { get; } = "A business validation exception occurred.";
		public object ExtendedInfo { get; } // this will be used to add more info to the returned object

		public YourBussException(
			[NotNull] string message,
			object extendedInfo = null,
			string type = null,
			string title = null)
			: base(message)
		{
			ExtendedInfo = extendedInfo ?? ExtendedInfo;
			Type = type ?? Type;
			Title = title ?? Title;
		}
	}
}
