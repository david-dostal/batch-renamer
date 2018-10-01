using System;
using System.Reflection;
using System.Windows.Forms;

namespace BatchRenamer
{
    public static class WinformsUtils
    {
        public static void SetDoubleBuffering(this Control control, bool enabled)
        {
            if (!SystemInformation.TerminalServerSession) // Double buffering can be slow in remote desktop
            {
                PropertyInfo propertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                propertyInfo.SetValue(control, enabled, null);
            }
        }
    }
}
