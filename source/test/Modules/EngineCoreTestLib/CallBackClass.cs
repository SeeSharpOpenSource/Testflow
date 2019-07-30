using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Testflow.Usr.Common;

namespace EngineCoreTestLib
{
    public static class CallBackClass
    {
        [CallBack("I'm a CallBack")]
        public static void StartForm()
        {
            Application.Run(new CallBackForm());
        }

        [CallBack("I'm a CallBack")]
        public static void StartForm(int x, int y)
        {
            Application.Run(new CallBackForm(x, y));
        }
    }
}
