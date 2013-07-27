using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDump
{
	internal class ConsoleWriter
	{
		#region console interaction - all virtual methods
		protected virtual void WritePlain(string s)
		{
			Console.Write(s);
		}

		protected virtual void Write(string s, ConsoleColor foreground, ConsoleColor background)
		{
			Console.ForegroundColor = foreground;
			Console.BackgroundColor = background;
			Console.Write(s);
			Console.ResetColor();
		}

		protected virtual void WriteLine()
		{
			Console.WriteLine();
		}
		#endregion

		#region const-ish
		static readonly ColorString _Null = new ColorString("null", ConsoleColor.Green, ConsoleColor.Black);
		static readonly ColorString _Separator = new ColorString(" ", ConsoleColor.White, ConsoleColor.DarkMagenta);
		static readonly ColorString _Ellipsis = new ColorString("\u2026", ConsoleColor.DarkMagenta, ConsoleColor.Cyan); // TODO see how this looks

		const ushort ENUMERABLE_LIMIT = 24;
		const ConsoleColor HEADING_FOREGROUND = ConsoleColor.White;
		const ConsoleColor HEADING_BACKGROUND = ConsoleColor.DarkCyan;
		#endregion

		#region write helpers
		private void Write(ColorString cs)
		{
			Write(cs.String, cs.Foreground, cs.Background);
		}

		private void WriteFixed(ColorString value, int width, TypeCode typeCode)
		{
			var len = value.String.Length;
			if (len == width)
			{
				Write(value);
			}
			else if (len < width)
			{
				// right align numbers
				string padded = typeCode >= TypeCode.SByte && typeCode <= TypeCode.Decimal ? 
					value.String.PadLeft(width) :
					value.String.PadRight(width);
				Write(padded, value.Foreground, value.Background);
			}
			else
			{
				Write(value.String.Substring(0, width - 1), value.Foreground, value.Background);
				Write(_Ellipsis);
			}
		}

		private void WritePadding(int padding)
		{
			if (padding < 1) return;
			WritePlain(new String(' ', padding));
		}
		
		private void WriteLabel(string label, int padding)
		{
			WritePadding(padding);
			Write(label, ConsoleColor.White, ConsoleColor.DarkBlue);
		}

		private void WriteEnumerableLabel(TypeDetails details, int shown, int? count, int padding)
		{
			string label = details.TypeLabel + " (";
			if (shown < ENUMERABLE_LIMIT || count == ENUMERABLE_LIMIT)
			{
				label += shown + " items)";
			}
			else
			{
				label += shown + " items of " + (count.HasValue ? count.Value.ToString() : "?") + ")";
			}
			WriteLabel(label, padding);
			WriteLine();
		}
		#endregion

		private ColorString GetString(TypeDetails details, object instance)
		{
			if (instance == null)
			{
				return _Null;
			}

			var typeCode = details.SimpleTypeCode;

			var foreground = 
				typeCode == TypeCode.String ? ConsoleColor.Cyan : // string
				typeCode != TypeCode.Object ? ConsoleColor.White : // primitive
				details.Type.IsClass ? ConsoleColor.Magenta : // class
				ConsoleColor.Yellow; // struct

			return new ColorString(
				instance.ToString(), 
				foreground, 
				details.NullableStruct ? ConsoleColor.DarkGreen : ConsoleColor.Black);
		}

		private ColorString GetString(MemberValue value)
		{
			if (value.Exception != null)
			{
				return new ColorString(value.Exception.Message, ConsoleColor.Red, ConsoleColor.Black);
			}
			else
			{
				return GetString(value.Details, value.Value);
			}
		}

		private void DumpComplexEnumerable(TypeDetails details, object instance, int padding)
		{
			int? count;
			var items = details.GetEnumerableMemberValues(instance, ENUMERABLE_LIMIT, out count);

			var itemType = details.ItemDetails;
			int columnCount = itemType.Members.Length;
			var columnWidths = new int[columnCount];
			int columnIndex;
			ColorString[] row;
			var allValues = new List<ColorString[]>(items.Count + 1);

			// get the column headings
			columnIndex = 0;
			row = new ColorString[columnCount];
			foreach (var member in itemType.Members)
			{
				row[columnIndex] = new ColorString(member.Name, HEADING_FOREGROUND, HEADING_BACKGROUND);
				columnWidths[columnIndex] = member.Name.Length;

				columnIndex++;
			}
			allValues.Add(row);

			// get all the values
			foreach (var item in items)
			{
				if (item == null)
				{
					allValues.Add(null);
					continue;
				}

				columnIndex = 0;
				row = new ColorString[columnCount];
				foreach (var value in item)
				{
					var cs = GetString(value);
					row[columnIndex] = cs;
					if (cs.String.Length > columnWidths[columnIndex])
					{
						columnWidths[columnIndex] = cs.String.Length;
					}

					columnIndex++;
				}
				allValues.Add(row);
			}


			// echo
			WriteEnumerableLabel(details, items.Count, count, padding);
			padding++;

			int rowCount = 0;
			foreach (var item in allValues)
			{
				WritePadding(padding);
				if (item == null)
				{
					Write(_Null);
				}
				else
				{
					columnIndex = 0;
					foreach (var value in item)
					{
						// grab TypeCode but not for headings
						var typeCode = rowCount == 0 ?
							TypeCode.String :
							itemType.Members[columnIndex].TypeDetails.SimpleTypeCode;

						Write(_Separator);
						WriteFixed(value, columnWidths[columnIndex], typeCode);
						columnIndex++;
					}
				}
				WriteLine();
				rowCount++;
			}
		}

		private void DumpSimpleEnumerable(TypeDetails details, object instance, int padding)
		{
			int? count;
			var items = details.GetEnumerableSimpleValues(instance, ENUMERABLE_LIMIT, out count);

			// get all strings
			int maxLength = 0;
			var values = new List<ColorString>(items.Count);
			foreach (var value in items)
			{
				var cs = GetString(details.ItemDetails, value);
				values.Add(cs);

				if (cs.String.Length > maxLength)
				{
					maxLength = cs.String.Length;
				}
			}

			// echo
			WriteEnumerableLabel(details, items.Count, count, padding);
			padding++;
			foreach (var value in values)
			{
				WritePadding(padding);
				WriteFixed(value, maxLength, details.ItemDetails.SimpleTypeCode);
				WriteLine();
			}
		}

		private void DumpMembers(TypeDetails details, object instance, int padding)
		{
			WriteLabel(details.TypeLabel, padding);
			WriteLine();

			var stringified = instance.ToString();
			if (!String.IsNullOrEmpty(stringified) && stringified != details.Type.FullName && stringified != instance.GetType().FullName)
			{
				WritePadding(padding);
				Write(
					stringified,
					details.Type.IsClass ?
						ConsoleColor.Magenta : // class
						ConsoleColor.Yellow,  // struct
					ConsoleColor.Black);
				WriteLine();
			}

			padding++;
			var values = details.GetMemberValues(instance);

			foreach (var value in values)
			{
				WritePadding(padding);
				Write(value.MemberName.PadRight(details.MaxMemberNameLength, ' '), HEADING_FOREGROUND, HEADING_BACKGROUND);
				Write(_Separator);
				Write(GetString(value));
				WriteLine();
			}
		}

		private void Dump(TypeDetails details, object instance, int padding)
		{
			if (instance == null || details.SimpleTypeCode != TypeCode.Object)
			{
				WritePadding(padding);
				Write(GetString(details, instance));
				WriteLine();
			}
			else if (!details.IsEnumerable)
			{
				DumpMembers(details, instance, padding);
				WriteLine(); // extra
			}
			else if (details.ItemDetails.SimpleTypeCode != TypeCode.Object)
			{
				DumpSimpleEnumerable(details, instance, padding);
				WriteLine(); // extra
			}
			else
			{
				DumpComplexEnumerable(details, instance, padding);
				WriteLine(); // extra
			}
		}

		public T Dump<T>(T it, string label)
		{
			object o = it;
			int padding = 0;
			
			if (!String.IsNullOrEmpty(label))
			{
				Write(label, ConsoleColor.Black, ConsoleColor.Gray);
				WriteLine();
				padding = 1;
			}
			
			if (o == null)
			{
				WritePadding(padding);
				Write(_Null);
				WriteLine();
				return it;
			}
			
			var type = o.GetType();
			if (type != typeof(T) && typeof(T) != typeof(object) && !type.IsPublic && typeof(T).IsPublic)
			{
				type = typeof(T);
			}

			var details = TypeDetails.Get(type);
			Dump(details, o, padding);
			
			return it;
		}
	}
}
