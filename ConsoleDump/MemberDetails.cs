using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleDump
{
	internal class MemberDetails
	{
		public readonly TypeDetails TypeDetails;
		public readonly FieldInfo FieldInfo;
		public readonly PropertyInfo PropertyInfo;
		public readonly string Name;
		
		private MemberDetails(FieldInfo fi)
		{
			FieldInfo = fi;
			Name = fi.Name;
			TypeDetails = TypeDetails.Get(fi.FieldType);
		}

		private MemberDetails(PropertyInfo pi)
		{
			PropertyInfo = pi;
			Name = pi.Name;
			TypeDetails = TypeDetails.Get(pi.PropertyType);
		}

		public MemberValue GetValue(object instance)
		{
			Exception exception = null;
			object value;

			if (FieldInfo != null)
			{
				value = FieldInfo.GetValue(instance);
			}
			else
			{
				try
				{
					value = PropertyInfo.GetValue(instance, null);
				}
				catch (TargetInvocationException tie)
				{
					value = null;
					exception = tie.InnerException ?? tie;
				}
			}

			return new MemberValue(this, value, exception);
		}

		public static MemberDetails[] GetAllMembers(TypeDetails details)
		{
			var properties = details.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var fields = details.Type.GetFields(BindingFlags.Instance | BindingFlags.Public);

			var members = properties
				.Where(pi => pi.GetGetMethod() != null && pi.GetIndexParameters().Length == 0)
				.Select(pi => new MemberDetails(pi));

			members = members.Concat(
				fields.Select(fi => new MemberDetails(fi))
			);

			return members.ToArray();
		}

		public override string ToString()
		{
			return ((object)PropertyInfo ?? FieldInfo).ToString();
		}
	}

}
