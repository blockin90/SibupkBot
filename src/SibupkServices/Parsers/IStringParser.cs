using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SibupkCommonServices.Parsers
{
    public interface IStringParser<T>
    {
        T Parse(string value);
    }
}
