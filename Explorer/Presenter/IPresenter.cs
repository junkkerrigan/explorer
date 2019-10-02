using Explorer.View;

namespace Explorer.Presenter
{
    public interface IPresenter
    {
        void Run();

        void CheckAccess(FileSystemNode node);

        void LoadDrives();

        void FillNode(FileSystemNode node);

        void LoadSubdirs(FileSystemNode node);
    }
}