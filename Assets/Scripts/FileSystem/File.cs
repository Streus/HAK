using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System.Reflection;
using System;

namespace FileSystemNS
{

    /// <summary> 
    /// A class representing the structure and function of a file.
    /// 
    /// A file has a name, of the form:
    ///     <name>.<extension>
    /// as well as a string of contents.
    /// 
    /// A directory is a file with a name of the form:
    ///     <name>
    /// as well as a List of File objects "contained" in it.
    /// 
    /// Both files and directories are created with a full Path.
    /// </summary>
    public class File
    {

        // A reference to the FileSystem this File is contained in.
        // Needed to get full names across networks and user permissions.
        protected FileSystem filesystem;

        // The name of this file/directory.
        protected string name;

        // Non-directory-only. The extension of this file.
        private string extension = null;

        // The absolute path of this file.
        protected string path;

        // The parent file of this file.
        protected Directory parent;

        /** 
         * Represents what users are allowed to do what.
         * 
         * Each digit can be a number from 0-7. 4 = read, 2 = write, 1 = execute.
         * The sum of these values determine the final permissions.
         * 
         * The ones place is for nonadmins, the tens for admins, and
         * the hundreds place is for root user.
         */
        protected int permissions = 777;

        // Allows subclasses to figure out their own constructors
        protected File() { }

        /**
         * Create a new File in the provided directory.
         */
        public File(FileSystem fs, Directory parent, string name)
        {
            if (name.Contains("."))
            {
                // Create a new file with sliced name and extension
                Setup(
                    fs,
                    parent,
                    name.Substring(0, name.IndexOf(".")),
                    name.Substring(name.LastIndexOf(".") + 1));
            }
            //Should this throw an exception otherwise?
        }

        public File(FileSystem fs, Directory parent, string name, string ext)
        {
            Setup(fs, parent, name, ext);
        }

        private void Setup(FileSystem fs, Directory parent, string name, string ext)
        {
            this.filesystem = fs;
            this.name = name;
            this.parent = parent;
            this.path = parent == null ? "" : parent.path + "/" + name + "." + ext;
            this.extension = ext;

            if (parent != null)
            {
                parent.addFile(this);
            }
        }

        public void delete()
        {
            if (this.name.Equals(""))
            {
                throw new InvalidFileException("Cannot delete root file.");
            }

            if (!filesystem.currentUser.canWrite(this.permissions))
            {
                throw new InvalidUserException("Cannot delete file: insufficient permissions. (requires write)");
            }

            this.parent.deleteFile(this);
        }

        public virtual string getFullName()
        {
            return name + "." + extension;
        }

        public virtual string getName()
        {
            return name;
        }

        public virtual void setName(string name, string ext)
        {
            if (ext == null || ext.Length == 0)
            {
                throw new InvalidFileException("Files must have a valid extension.");
            }
            this.name = name;
            this.extension = ext;
        }

        // Non-directory-file only
        public string getExtension()
        {
            return this.extension;
        }

        public string getPath()
        {
            return path;
        }

        public File getParent()
        {
            return parent;
        }

        public void moveTo(Directory newDir)
        {
            this.parent.deleteFile(this);
            newDir.addFile(this);

            this.parent = newDir;
            this.path = newDir.path + "/" + this.getFullName();
        }

        // TODO: find a way to make this only callable from FileSystem
        public void setPermissions(int to)
        {
            this.permissions = to;
        }

        // TODO: find a way to make this only callable from FileSystem
        public int getPermissions()
        {
            return permissions;
        }
    }
}