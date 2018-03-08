using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConsoleDump
{
    internal class ColumnDetails : MemberDetails
    {
        public readonly DataColumn DataColumn;
        public ColumnDetails(DataColumn column) : base(column.ColumnName, TypeDetails.Get(column.DataType))
        {
            DataColumn = column;
        }

        public override MemberValue GetValue(object instance)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => DataColumn.ToString();
    }
}
