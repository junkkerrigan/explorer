using System.Threading.Tasks;
using Explorer.View;

namespace Explorer.Presenter
{
    /// <summary>
    /// Provides interface for interacting with <see cref="Presenter"/>.
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// Invokes application's run.
        /// </summary>
        void Run();

        //void CheckAccessibility(FileSystemNode node);

        /// <summary>
        /// Loads system drives.
        /// </summary>
        void LoadDrives();

        /// <summary>
        /// 
        /// </summary>
        void LoadSubDirectories(FileSystemNode node);
    }
}