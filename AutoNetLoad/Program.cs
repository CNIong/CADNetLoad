namespace AutoNetLoad
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    internal static class Program
    {
        [MethodImpl(MethodImplOptions.NoInlining), STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormAutoLoad());
        }
    }
}

