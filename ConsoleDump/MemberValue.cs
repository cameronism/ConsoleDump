using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleDump
{
    internal struct MemberValue
    {
        public readonly TypeDetails Details;
        public readonly string MemberName;
        public readonly object Value;
        public readonly Exception Exception;

        public MemberValue(MemberDetails md, object value, Exception exception) : this(md.TypeDetails, md.Name, value, exception)
        {
        }

        public MemberValue(TypeDetails td, string memberName, object value, Exception exception)
        {
            Details = td;
            MemberName = memberName;
            this.Value = value;
            this.Exception = exception;
        }
    }
}
