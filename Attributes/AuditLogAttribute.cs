using System;

namespace Webapi.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class AuditLogAttribute:Attribute
	{
		public AuditLogAttribute()
		{

		}
	}
}