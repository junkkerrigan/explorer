using System;
using System.Windows.Forms;

namespace Explorer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var View = new Explorer();
            var presenter = new ExplorerPresenter(View);

            presenter.Run();
        }
    }
}
