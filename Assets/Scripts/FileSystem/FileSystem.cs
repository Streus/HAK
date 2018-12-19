using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System;

namespace FileSystemNS
{
    public class FileSystem
    {

        // The only valid filenames are "([0-9][a-z][A-Z]-_)*"
        private static Regex validNameRegex = new Regex(@"(\w?-?_?)+");
        private static string[] validFileExtensions = { "txt", "src" };

        public readonly Directory root;

        public List<User> allUsers;
        public User currentUser;

        public FileSystem()
        {
            root = new Directory(this, null, "");

            //TODO: should this be publicly visible? readonly?
            User rootUser = new User("root", "", SecurityLevel.ROOT);
            allUsers = new List<User>() { rootUser };
            currentUser = rootUser;
        }

        /**
         * In this case, "name" includes the full path. Thus,
         * createFile("/home/test/pass.txt") is equivalent to createFile("pass.txt", test).
         * 
         * TODO: simplify this code a bit more. (ditto for directory below)
         */
        public EditableFile createFile(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw new InvalidFileException("Invalid file name (null or 0 length).");
            }

            if (getFile(name) != null)
            {
                throw new InvalidFileException("File already exists!");
            }

            // This is a file to be placed in the root directory.
            if (!name.Contains("/"))
            {
                if (!isValidFullFileName(name))
                {
                    throw new InvalidFileException("Invalid file name.");
                }
                EditableFile file = new EditableFile(this, root, name);
                return file;
            }
            else
            {
                // Otherwise, try to get the directory we're placing this into.
                string path = name.Substring(0, name.LastIndexOf("/"));
                string filename = name.Substring(name.LastIndexOf("/") + 1);
                File dir = getFile(path);

                if (path == null || !(dir is Directory))
                {
                    // TODO: Can change this to generate the required path instead of failing.
                    throw new InvalidFileException("Cannot create file: path \"" + path + "\" does not exist.");
                }
                if (!isValidFullFileName(filename))
                {
                    throw new InvalidFileException("Invalid file name.");
                }

                EditableFile file = new EditableFile(this, dir as Directory, filename);
                return file;
            }
        }

        public Directory createDirectory(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw new InvalidFileException("Invalid directory name (null or 0 length).");
            }

            if (getFile(name) != null)
            {
                throw new InvalidFileException("Directory already exists!");
            }

            // This is a file to be placed in the root directory.
            if (!name.Contains("/"))
            {
                if (!isValidFileName(name))
                {
                    throw new InvalidFileException("Invalid directory name.");
                }
                Directory file = new Directory(this, root, name);
                return file;
            }
            else
            {
                // Otherwise, try to get the directory we're placing this into.
                string path = name.Substring(0, name.LastIndexOf("/"));
                string filename = name.Substring(name.LastIndexOf("/") + 1);
                File dir = getFile(path);

                if (path == null || !(dir is Directory))
                {
                    // TODO: Can change this to generate the required path instead of failing.
                    throw new InvalidFileException("Cannot create directory: path \"" + path + "\" does not exist.");
                }
                if (!isValidFileName(filename))
                {
                    throw new InvalidFileException("Invalid directory name.");
                }

                Directory file = new Directory(this, dir as Directory, filename);
                return file;
            }
        }

        /**
         * Moves a file from one directory to a new one.
         */
        public void moveFile(File toMove, Directory newDir)
        {
            if (toMove == null || newDir == null)
            {
                throw new InvalidFileException("Input or output files are invalid.");
            }
            if (newDir.getPath() == null)
            {
                throw new InvalidFileException("Output file has an invalid path. (bug in path creation?)");
            }
            if (toMove == newDir)
            {
                throw new InvalidFileException("Output directory cannot match input directory.");
            }
            if (newDir.getPath().Contains(toMove.getPath()))
            {
                throw new InvalidFileException("Cannot move directory within itself.");
            }

            toMove.moveTo(newDir);
        }

        //TODO: possibly rename this to "find" or "search" or "exists".
        public File getFile(string path)
        {
            if (path == null)
            {
                return null;
            }

            string[] pathElements = path.Split(new char[] { '/' });
            File currentFile = root;
            for (int i = 0; i < pathElements.Length; i++)
            {
                if (currentFile is Directory)
                {
                    Directory currentDir = currentFile as Directory;
                    if (pathElements[i].Equals(".."))
                    {
                        if (currentDir == root)
                        {
                            continue;
                        }
                        else
                        {
                            currentFile = currentDir.getParent();
                        }
                    }
                    else if (pathElements[i].Equals(".") || pathElements[i].Equals(""))
                    {
                        continue;
                    }
                    else if (currentDir.containsFile(pathElements[i]))
                    {
                        currentFile = currentDir.getFile(pathElements[i]);
                    }
                    else
                    {
                        // We could not find a file, so fail
                        return null;
                    }
                }
                else
                {
                    // We found a terminal file in the middle of the path
                    if (i != pathElements.Length - 1)
                    {
                        return null;
                    }
                }
            }
            return currentFile;
        }

        /**
         * True iff: 
         *     - name is not null and name length is > 0
         *     - filename contains "."
         *     - name before "." contains word chars, "-", or "_" only
         *     - name after "." is a valid file extension
         */
        private static bool isValidFullFileName(string name)
        {
            if (name == null || name.Length == 0)
            {
                return false;
            }
            if (name.IndexOf(".") <= 0)
            {
                return false;
            }

            string nm = name.Substring(0, name.IndexOf("."));
            string ex = name.Substring(name.LastIndexOf(".") + 1).ToLower();

            return (isValidFileName(nm) && isValidExtension(ex));
        }

        /**
         * True iff name is not null or zero length, and contains word chars, "-", or "_" only.
         */
        private static bool isValidFileName(string name)
        {
            if (name == null || name.Length == 0)
            {
                return false;
            }

            return validNameRegex.Matches(name).Count <= 2;
        }

        /**
         * True iff extension is in validFileExtensions
         */
        private static bool isValidExtension(string extension)
        {
            bool isValid = false;
            foreach (string str in validFileExtensions)
            {
                if (extension.Equals(str))
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        /**
         * Adds a user to the list of users available to log in as.
         * Throws an exception if the user already exists.
         */
        public void addUser(string username, string password, SecurityLevel level)
        {
            if (level == SecurityLevel.ROOT)
            {
                throw new InvalidUserException("Only one root user can exist.");
            }

            foreach (User u in allUsers)
            {
                if (u.username.Equals(username))
                {
                    throw new InvalidUserException("A user with that username already exists.");
                }
            }

            this.allUsers.Add(new User(username, password, level));
        }

        /**
         * Sets currently active user.
         * TODO: Maybe the errors should be merged to prevent guessing usernames.
         */
        public void changeUser(string username, string password)
        {
            foreach (User u in allUsers)
            {
                if (u.username.Equals(username))
                {
                    if (u.verifyPassword(password))
                    {
                        this.currentUser = u;
                        return;
                    }
                    else
                    {
                        throw new InvalidUserException("Invalid password.");
                    }
                }
            }
            throw new InvalidUserException("Invalid username.");
        }

        public void setPermissions(File fi, int to)
        {
            int oldR = fi.getPermissions() / 100;
            int oldA = (fi.getPermissions() / 10) % 10;
            //int oldN = fi.getPermissions () % 10;

            int newR = to / 100;
            int newA = (to / 10) % 10;
            int newN = to % 10;

            if (newR < 0 || newR > 7 || newA < 0 || newA > 7 || newN < 0 || newN > 7)
            {
                throw new InvalidFileException("Invalid permission flag. Each flag is 0 >= x >= 7.");
            }

            // Non-admins can only change non-admin permissions.
            if (this.currentUser.adminLevel == SecurityLevel.NONADMIN && (oldR == newR) && (oldA == newA))
            {
                fi.setPermissions(to);
                return;
            }

            // Admins can change non-admin and admin permissions.
            if (this.currentUser.adminLevel == SecurityLevel.ADMIN && (oldR == newR))
            {
                fi.setPermissions(to);
                return;
            }

            // The root user can change anything.
            if (this.currentUser.adminLevel == SecurityLevel.ROOT)
            {
                fi.setPermissions(to);
                return;
            }

            throw new InvalidUserException("Cannot change permissions for users with higher privilege.");
        }
    }

    // TODO: consider merging exceptions to prevent need to remember multiple types.

    /** 
     * Thrown if you try to create or modify an invalid file; i.e., an invalid
     * name on creation.
     */
    public class InvalidFileException : Exception
    {
        public InvalidFileException() : base("Attempted to create or modify invalid file.") { }
        public InvalidFileException(string msg) : base(msg) { }
    }

    /*
     * Thrown if you try to do an unsupported operation, like setting the contents of a directory.
     */
    public class InvalidOperationException : Exception
    {
        public InvalidOperationException() : base("Unsupported file operation") { }
        public InvalidOperationException(string msg) : base(msg) { }
    }

    /*
     * Thrown if you have invalid permissions for a file operation, or if you fail
     * to do a valid user operation.
     */
    public class InvalidUserException : Exception
    {
        public InvalidUserException() : base("Unsupported user operation") { }
        public InvalidUserException(string msg) : base(msg) { }
    }
}