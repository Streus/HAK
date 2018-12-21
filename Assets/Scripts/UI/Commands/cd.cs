using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileSystemNS;


namespace Commands
{
    public class cd : Command
    {
        public override string getHelp()
        {
            return "Change directory. Usage: cd <directory>.";
        }

        public override string getInvocation()
        {
            return "cd";
        }

        public override string execute(params string[] args)
        {
            NetworkNode node = GameManager.currentHost;
            if (!(node is IFileSystem))
            {
                throw new ExecutionException("The current node does not support a file system.");
            }
            FileSystem currentFileSystem = (node as IFileSystem).fileSystem;

            string currentPath = GameManager.currentPath;
            string filename = args[1];
            File newFile = currentFileSystem.getFile(currentPath + "/" + filename);

            if (newFile == null)
            {
                throw new ExecutionException("Directory \"" + filename + "\" does not exist.");
            }

            if (!(newFile is Directory))
            {
                throw new ExecutionException("File \"" + filename + "\" is not a directory.");
            }

            // This forces a simplification of paths, so things like "/test/../test/.." get turned into "/".
            GameManager.currentPath = newFile.getPath();
            return "Current directory is now \"" + GameManager.currentPath + "/\".";
        }
    }
}
