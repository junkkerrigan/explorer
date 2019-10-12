﻿using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Presenters;

namespace Explorer.Views
{
    // TODO: if required, remove FSE and implement IFSE directly in DE, FE
    public abstract class FileSystemElement : IFileSystemElement
    {
        public string Path { get; set; }
        public IFileSystemNode Node { get; set; }

        public FileSystemElement(IFileSystemNode node)
        {
            Node = node;
        }

        void IFileSystemElement.CopyTo(string destinationPath)
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

        public abstract void OpenWithDefaultApplication();

        public void UpdatePath(string newPath)
        {
            this.Path = newPath;

            foreach (IFileSystemNode node in this.Node.SubNodes)
            {
                node.Element.UpdatePath(System.IO.Path.Combine(newPath, node.Text));
            }
        }

        public abstract void Move(string destinationPath);
    }

    public class DirectoryElement : FileSystemElement
    {
        public DirectoryElement(IFileSystemNode node) : base(node)
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

        public override void OpenWithDefaultApplication()
        {
            throw new NotSupportedException();
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
                Console.WriteLine("Error in FileElement.Move:");
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class FileElement : FileSystemElement
    {
        public FileElement(IFileSystemNode node) : base(node)
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

        public override void OpenWithDefaultApplication()
        {
            try
            {
                Process.Start(this.Path);
            }
            catch(Exception ex)
            {
                // TODO: ShowModalInvalidLink();
                Console.WriteLine("Error in FileElement.OpenWithDefaultApplication");
                Console.WriteLine(ex.Message);
            }
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
                Console.WriteLine("Error in FileElement.Move:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
