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
        /// Loads drives of a system.
        /// </summary>
        void LoadDrives();

        /// <summary>
        /// Loads subdirectories of passed directory.
        /// </summary>
        /// <param name="node"></param>
        void LoadSubdirs(FileSystemNode node);
    }
}