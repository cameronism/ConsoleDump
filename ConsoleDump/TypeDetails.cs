using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleDump
{
	internal class TypeDetails
	{
		const string _AnonymousLabel = "\u00f8";

		public readonly Type Type;
		public readonly string TypeLabel;

		/// <summary>
		/// TypeCode unwrapping Nullable<> if needed
		/// </summary>
		public readonly TypeCode SimpleTypeCode;
		public readonly bool NullableStruct;
		
		public readonly MemberDetails[] Members;
		public readonly TypeDetails ItemDetails;
		private readonly GetGenericEnumeratorDelegate _GetItemEnumerator;
		public readonly int MaxMemberNameLength;

		private TypeDetails(Type type)
		{
			Type = type;
			_UnderConstruction.Push(this);
			try
			{
				TypeLabel = GetFullName(type);
				Members = null;
				var underlying = Nullable.GetUnderlyingType(type);
				NullableStruct = underlying != null;
				SimpleTypeCode = Type.GetTypeCode(underlying ?? type);

				if (SimpleTypeCode != TypeCode.Object)
				{
					// primitive-ish, that's enough details
					return;
				}

				// IEnumerable<>
				Type elementType = null;
				if (type.IsArray)
				{
					elementType = type.GetElementType();
				}
				else
				{
					IEnumerable<Type> interfaces = type.GetInterfaces();
					if (type.IsInterface)
					{
						interfaces = new[] { type }.Concat(interfaces);
					}

					var ienumerableType = interfaces.FirstOrDefault(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>));

					if (ienumerableType != null)
					{
						elementType = ienumerableType.GetGenericArguments()[0];
					}
				}

				if (elementType != null)
				{
					ItemDetails = Get(elementType);
					_GetItemEnumerator = CreateDelegate(ItemDetails.Type);
					return;
				}

				// not IEnumerable<>
				Members = MemberDetails.GetAllMembers(this);
				if (Members.Any())
				{
					MaxMemberNameLength = Members.Max(m => m.Name.Length);
				}
			}
			finally
			{
				_UnderConstruction.Pop();
			}
		}

		public bool IsEnumerable { get { return _GetItemEnumerator != null;  } }
		public MemberValue[] GetMemberValues(object instance)
		{
			var members = Members;
			var values = new MemberValue[members.Length];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = members[i].GetValue(instance);
			}
			return values;
		}

		public List<object> GetEnumerableSimpleValues(object instance, ushort limit, out int? count)
		{
			IEnumerator enumerator;
			IDisposable disposable;
			
			_GetItemEnumerator(instance, out enumerator, out disposable, out count);
			var itemValues = new List<object>(Math.Min(count ?? ushort.MaxValue, limit));
			using (disposable)
			{
				int total = 0;
				while (enumerator.MoveNext() && total++ < limit)
				{
					itemValues.Add(enumerator.Current);
				}
			}

			return itemValues;
		}

		public List<MemberValue[]> GetEnumerableMemberValues(object instance, ushort limit, out int? count)
		{
			var values = GetEnumerableSimpleValues(instance, limit, out count);
			var memberValues = new List<MemberValue[]>(values.Count);
			foreach (var value in values)
			{
				memberValues.Add(value == null ? null : ItemDetails.GetMemberValues(value));
			}
			return memberValues;
		}

		public override string ToString()
		{
			return this.Type.ToString();
		}

		#region static
		private static string[] _Namespaces = { "System", "System.Collections.Generic" };
		internal static string GetFullName(Type type)
		{
			var underlying = Nullable.GetUnderlyingType(type);
			if (underlying != null)
			{
				return GetFullName(underlying) + "?";
			}

			if (type.IsArray)
			{
				return GetFullName(type.GetElementType()) + "[]";
			}

			var fullName = _Namespaces.Contains(type.Namespace) ? type.Name : type.FullName;
			
			if (type.IsGenericType)
			{
				if (fullName.StartsWith("<>", StringComparison.Ordinal))
				{
					return _AnonymousLabel;
				}

				int index = fullName.IndexOf('`');
				if (index != -1)
				{
					fullName = fullName.Substring(0, index);
				}

				var genericArgs = type.GetGenericArguments();
				fullName += 
					"<" +
					string.Join(", ", genericArgs.Select(t => GetFullName(t))) +
					">";
			}
			
			return fullName;
		}

		private delegate void GetGenericEnumeratorDelegate(object enumerable, out IEnumerator enumerator, out IDisposable disposable, out int? count);
		
		private static void GetGenericEnumerator<T>(object enumerable, out IEnumerator enumerator, out IDisposable disposable, out int? count)
		{
			var genericEnumerator = ((IEnumerable<T>)enumerable).GetEnumerator();
			enumerator = (IEnumerator)genericEnumerator;
			disposable = (IDisposable)genericEnumerator;
			
			var collection = enumerable as ICollection<T>;
			count = collection == null ? (int?)null : collection.Count;
		}
		
		private static MethodInfo _GenericDefinition = ((GetGenericEnumeratorDelegate)GetGenericEnumerator<int>).Method.GetGenericMethodDefinition();
		private static GetGenericEnumeratorDelegate CreateDelegate(Type itemType)
		{
			return (GetGenericEnumeratorDelegate)Delegate.CreateDelegate(
				typeof(GetGenericEnumeratorDelegate),
				_GenericDefinition.MakeGenericMethod(itemType));
		}



		private static Dictionary<Type, TypeDetails> _Cache = new Dictionary<Type, TypeDetails>();
		[ThreadStatic]
		static Stack<TypeDetails> _UnderConstruction;

		public static TypeDetails Get(Type type)
		{
			TypeDetails md;
			lock (_Cache)
			{
				if (_Cache.TryGetValue(type, out md))
				{
					return md;
				}
			}

			var underConstruction = _UnderConstruction;
			if (underConstruction == null)
			{
				_UnderConstruction = new Stack<TypeDetails>();
			}
			else
			{
				md = underConstruction.FirstOrDefault(td => td.Type == type);
				if (md != null) return md;
			}

			md = new TypeDetails(type);

			lock (_Cache)
			{
				_Cache[type] = md;
			}
			return md;
		}
		#endregion
	}
}
