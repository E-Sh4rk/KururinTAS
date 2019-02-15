using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KuruBot
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        public static void UIThread(this Control @this, System.Action code)
        {
            if (@this.InvokeRequired)
                @this.BeginInvoke(code);
            else
                code.Invoke();
        }
    }
}
