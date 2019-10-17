using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Explorer
{
    public abstract class FileSystemItemEntity : IFileSystemItemEntity
    {
        public string Path { get; set; }

        public bool IsAccessible { get; set; }

        public IFileSystemTreeNode Node { get; set; }

        public FileSystemItemEntity(IFileSystemTreeNode node)
        {
            Node = node;
        }

        public void CopyTo(string destinationPath)
        {
            Copy(this.Path, destinationPath);
        }

        protected abstract void Copy(string sourcePath, string destinationPath);

        public abstract void Delete();

        public void EditName(string newName)
        {
            string newPath = this.Path.Remove(this.Path.LastIndexOf('\\') + 1) + newName;
            try
            {
                this.Move(newPath);
            }
            catch
            {
                throw;
            }
        }

        public void OpenWithDefaultApplication()
        {
            try
            {
                Process.Start(this.Path);
            }
            catch (Win32Exception ex)
            {
                // TODO: ShowModalImpossibleToOpen();
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // TODO: ShowModalInvalidLink();
                Console.WriteLine("Error in Entity.OpenWithDefaultApplication");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.GetType());
            }
        }

        public void UpdatePath(string newPath)
        {
            this.Path = newPath;

            foreach (IFileSystemTreeNode node in this.Node.SubNodes)
            {
                node.Entity.UpdatePath(System.IO.Path.Combine(newPath, node.Name));
            }
        }

        public abstract void Move(string destinationPath);
    }

    public class DriveEntity : FileSystemItemEntity
    {
        public DriveEntity(IFileSystemTreeNode node) : base(node)
        {
        }

        protected override void Copy(string sourcePath, string destinationPath)
        {
            throw new NotSupportedException("Error: impossible to copy drive");
        }

        public override void Delete()
        {
            throw new NotSupportedException("Error: impossible to delete drive");
        }

        public override void Move(string destinationPath)
        {
            try
            {
                Directory.Move(this.Path, destinationPath);
                this.UpdatePath(destinationPath);
            }
            catch (IOException)
            {
                throw new DirectoryAlreadyExistsException();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FileEntity.Move:");
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class FolderEntity : FileSystemItemEntity
    {
        public FolderEntity(IFileSystemTreeNode node) : base(node)
        {
        }

        protected override void Copy(string sourcePath, string destinationPath)
        {
            if (Directory.Exists(destinationPath))
            {
                throw new DirectoryAlreadyExistsException();
            }
            Directory.CreateDirectory(destinationPath);

            string[] subDirs = Directory.GetDirectories(sourcePath);
            foreach (string dir in subDirs)
            {
                string folderName = dir.Substring(dir.LastIndexOf('\\') + 1),
                    newDestPath = System.IO.Path.Combine(destinationPath, folderName);
                Copy(dir, newDestPath);
            }

            string[] innerFiles = Directory.GetFiles(sourcePath);
            foreach (string file in innerFiles)
            {
                string fileName = System.IO.Path.GetFileName(file),
                    destFileName = System.IO.Path.Combine(destinationPath, fileName);
                File.Copy(file, destFileName);
            }

            // ShowModalWhenFinished();
        }

        public override void Delete()
        {
            Directory.Delete(this.Path, true);
        }

        public override void Move(string destinationPath)
        {
            try
            {
                Directory.Move(this.Path, destinationPath);
                this.UpdatePath(destinationPath);
            }
            catch (IOException)
            {
                throw new DirectoryAlreadyExistsException();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FileEntity.Move:");
                Console.WriteLine(ex.Message);
            }
        }
    }


    public class FileEntity : FileSystemItemEntity
    {
        public FileEntity(IFileSystemTreeNode node) : base(node)
        {
        }

        protected override void Copy(string sourcePath, string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                throw new FileAlreadyExistsException();
            }
            File.Copy(sourcePath, destinationPath);

            // ShowModalWhenFinished();
        }

        public override void Delete()
        {
            File.Delete(this.Path);
        }

        public override void Move(string destinationPath)
        {
            try
            {
                File.Move(this.Path, destinationPath);
                this.Path = destinationPath;
            }
            catch(IOException)
            {
                throw new FileAlreadyExistsException();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in FileEntity.Move:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
