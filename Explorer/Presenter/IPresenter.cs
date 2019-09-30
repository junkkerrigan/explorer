using Explorer.View;

namespace Explorer.Presenter
{
    public interface IPresenter
    {
        void Run();

        void LoadDrives();

        void FillNode(FileSystemNode node);

        void LoadSubdirs(FileSystemNode node);
    }
}