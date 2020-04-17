using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Testflow.Usr.Common;
using Testflow.Data;
using Testflow.ExtensionBase.Common;

namespace EngineCoreTestLib
{
    public static class CallBackClass
    {
        [CallBack(CallBackType.Asynchronous)]
        public static void StartForm()
        {
            Application.Run(new CallBackForm());
        }

        [CallBack(CallBackType.Synchronous)]
        public static void StartForm(int x, int y)
        {
            Application.Run(new CallBackForm(x, y));
        }
    }
}
