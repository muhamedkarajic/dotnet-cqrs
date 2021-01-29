using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class DatabaseRetryAttribute: Attribute
	{
		public DatabaseRetryAttribute()
		{

		}
	}
}
