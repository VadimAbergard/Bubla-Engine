using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test_Winforms.utils
{
    public class Timer
    {
        protected internal delegate void Task();
        protected internal event Task task;

        protected internal static void Run(Task task, float seconds)
        {
            Thread thread = null;
            thread = new Thread(() => {
                thread.Join((int)(seconds * 1000));
                task.Invoke();
            });
            thread.Start();

        }
    }
}
