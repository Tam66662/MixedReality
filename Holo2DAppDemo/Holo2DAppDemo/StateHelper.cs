using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holo2DAppDemo
{
    public static class StateHelper
    {
        public static string PreviousString { get; private set; }

        public static void AppendString(string message)
        {
            PreviousString += string.Format("{0} | {1}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), message);
        }

        public static void InitializeString(string message)
        {
            PreviousString = message;
            AppendString("=================== BEGIN ===================");
        }
    }
}
