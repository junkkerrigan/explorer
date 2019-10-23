using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace Explorer
{
    public abstract class FileSystemItemEntity : IFileSystemItemEntity
    {
        public string Path { get; set; }

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
            catch (Win32Exception)
            {
                MessageBox.Show($"Impossible to open {this.Node.Name}.", "Opening error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
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

        public virtual void MergeTo(string filePath)
        {
        }
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
            try
            {
                Directory.Delete(this.Path, true);
            }
            catch (IOException)
            {
                MessageBox.Show($"Impossible to delete {this.Node.Name}.", "Deleting error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
            try
            {
                File.Delete(this.Path);
            }
            catch(IOException)
            {
                MessageBox.Show($"Impossible to delete {this.Node.Name}.", "Deleting error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void Move(string destinationPath)
        {
            try
            {
                Console.WriteLine(this.Path);
                Console.WriteLine(this);
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

        public override void MergeTo(string filePath)
        {
            string text = File.ReadAllText(this.Path);

            try
            {
                File.AppendAllText(filePath, text);
            }
            catch
            {
                throw;
            }
            
        }
    }

    public class TextFileEntity : FileEntity
    {
        public string Text { get; set; }

        public FileEntity FileEntity { get; set; }

        public new string Path
        {
            get => FileEntity.Path;
            set => FileEntity.Path = value;
        }

        public TextFileEntity(FileEntity file) : base(file.Node)
        {
            FileEntity = file;

            this.Text = "";
        }

        public string Read()
        {
            try
            {
                this.Text = File.ReadAllText(this.Path, Encoding.GetEncoding(1251));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return this.Text;
        }

        public void Write(string text)
        {
            this.Text = text;
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(this.Path, this.Text);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public abstract class Saver
    {
        protected TextFileEntity TextFile { get; set; }

        public Saver(TextFileEntity file)
        {
            TextFile = file;
        }

        public abstract void Save();
    }

    public class HTMLSaver : Saver
    {
        public HTMLSaver(TextFileEntity file) : base(file)
        {
        }

        public override void Save()
        {
            TextFile.Save();

            string newPath = Path.ChangeExtension(TextFile.Path, ".html");

            try
            {
                TextFile.FileEntity.Move(newPath);

                TextFile.Path = newPath;

                TextFile.Node.Name = Path.GetFileName(newPath);

                TextFile.Node.ListItem.Name = Path.GetFileName(newPath);

                TextFile.Node.ListItem.List.UpdateRefresh();
            }
            catch(AlreadyExistsException)
            {
                MessageBox.Show($"Impossible to save:"
                    + $" {Path.GetFileName(newPath)} already exists.", "Saving error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class TXTSaver : Saver
    {
        public TXTSaver(TextFileEntity file) : base(file)
        {
        }

        public override void Save()
        {
            TextFile.Save();
            
            string newPath = Path.ChangeExtension(TextFile.Path, ".txt");

            try
            {
                TextFile.FileEntity.Move(newPath);

                TextFile.Path = newPath;

                TextFile.Node.Name = Path.GetFileName(newPath);

                TextFile.Node.ListItem.Name = Path.GetFileName(newPath);

                TextFile.Node.ListItem.List.UpdateRefresh();
            }
            catch (AlreadyExistsException)
            {
                MessageBox.Show($"Impossible to save:"
                    + $" {Path.GetFileName(newPath)} already exists.", "Saving error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
