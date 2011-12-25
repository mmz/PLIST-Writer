using System;
using System.Reflection;
using System.Linq;

namespace EmitLib.Utils
{
	public class ReflectionUtils
	{
		public class MatchedMember
		{
			public MemberInfo First { get; set; }
			public MemberInfo Second { get; set; }
			public MatchedMember(MemberInfo first, MemberInfo second)
			{
				First = first;
				Second = second;
			}
		}

		public static bool IsNullable(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static MemberInfo[] GetPublicFieldsAndProperties(Type type)
		{
			return type
				.GetMembers(BindingFlags.Instance | BindingFlags.Public)
				.Where(mi => mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field)
				.ToArray();
		}

		public static MatchedMember[] GetCommonMembers(Type first, Type second, Func<string, string, bool> matcher)
		{
			if (matcher == null)
			{
				matcher = (f, s) => f == s;
			}
			var firstMembers = GetPublicFieldsAndProperties(first);
			var secondMembers = GetPublicFieldsAndProperties(first);
			return (from f in firstMembers
			        let s = secondMembers.FirstOrDefault(sm => matcher(f.Name, sm.Name))
			        where s != null
			        select new MatchedMember(f, s)).ToArray();
		}

		public static Type GetMemberType(MemberInfo mi)
		{
			if (mi is PropertyInfo)
			{
				return ((PropertyInfo)mi).PropertyType;
			}
			if (mi is FieldInfo)
			{
				return ((FieldInfo)mi).FieldType;
			}
			if (mi is MethodInfo)
			{
				return ((MethodInfo)mi).ReturnType;
			}
			return null;
		}

		public static Type GetMemberType2(MemberInfo mi)
		{
			if (mi is PropertyInfo)
			{
				return ((PropertyInfo)mi).PropertyType;
			}
			if (mi is FieldInfo)
			{
				return ((FieldInfo)mi).FieldType;
			}
			if (mi is MethodInfo)
			{
				var rt = ((MethodInfo)mi).ReturnType;
				return rt != typeof(void) ? rt : mi.DeclaringType;
			}
			return null;
		}

		public static bool HasDefaultConstructor(Type type)
		{
			return type.GetConstructor(new Type[0]) != null;
		}
	}
}