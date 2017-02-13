using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utils
{
    public static class Program
    {
        public static bool IsApp = false;
        [STAThread]
        
        public static void Main()
        {
            IsApp = true;
            ModelUtils.CheckOneBoltGroup();
        }

    }
}
