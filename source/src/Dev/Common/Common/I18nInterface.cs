using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testflow.Common
{
    public interface I18NInterface
    {
        string GetStr(string labelKey);
        string GetFStr(string labelKey, params object[] param);
    }
}
