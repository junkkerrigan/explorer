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

            var view = new View.Explorer();
            var presenter = new Presenter.Presenter(view);

            presenter.Run();
        }
    }
}
