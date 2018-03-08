using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConsoleDump
{
    internal class DataTableDetails : TypeDetails
    {
        readonly DataTable _table;
        public DataTableDetails(DataTable table) : base(table)
        {
            _table = table;
        }

        public override List<MemberValue[]> GetEnumerableMemberValues(object instance, int limit, out int? count)
        {
            var size = Math.Min(_table.Rows.Count, limit);
            var values = new List<MemberValue[]>(size);
            var rows = _table.Rows;
            count = rows.Count;
            var columns = _table.Columns.Count;
            var details = Members;

            for (var i = 0; i < size; i++)
            {
                var members = new MemberValue[columns];
                values.Add(members);

                for (int j = 0; j < members.Length; j++)
                {
                    members[j] = new MemberValue(details[j], rows[i][j], null);
                }
            }
            return values;
        }

        public override bool IsEnumerable => true;
    }
}
