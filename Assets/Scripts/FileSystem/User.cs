using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileSystemNS
{
    public enum SecurityLevel
    {
        Root = 0,
        Admin,
        Nonadmin
    }

    /// <summary>
    /// A class representing a user on a given NetworkNode.
    /// </summary>
    public class User
    {
        public string username;
        private string password;

        public SecurityLevel adminLevel;

        public User(string user, string pass, SecurityLevel level)
        {
            username = user;
            password = pass;
            adminLevel = level;
        }

        /**
         * I know this isn't cryptographically secure, but it doesn't need to be.
         * This is a game, and this provides the basic security we need in it.
         */
        public bool verifyPassword(string pass)
        {
            return pass.Equals(password);
        }

        public void changePassword(string oldPass, string newPass)
        {
            if (verifyPassword(oldPass))
            {
                this.password = newPass;
            }
        }

        public bool CanRead(int permissions)
        {
            int R = permissions / 100;
            int A = (permissions / 10) % 10;
            int N = permissions % 10;

            if (adminLevel == SecurityLevel.Root && ((R & 4) == 4))
            {
                return true;
            }
            
            if (adminLevel == SecurityLevel.Admin && ((A & 4) == 4))
            {
                return true;
            }
            
            if (adminLevel == SecurityLevel.Nonadmin && ((N & 4) == 4))
            {
                return true;
            }
            return false;
        }

        public bool CanWrite(int permissions)
        {
            int R = permissions / 100;
            int A = (permissions / 10) % 10;
            int N = permissions % 10;

            if (adminLevel == SecurityLevel.Root && ((R & 2) == 2))
            {
                return true;
            }
            
            if (adminLevel == SecurityLevel.Admin && ((A & 2) == 2))
            {
                return true;
            }
            
            if (adminLevel == SecurityLevel.Nonadmin && ((N & 2) == 2))
            {
                return true;
            }
            return false;
        }

        public bool canExecute(int permissions)
        {

            int R = permissions / 100;
            int A = (permissions / 10) % 10;
            int N = permissions % 10;

            if (adminLevel == SecurityLevel.Root && ((R & 1) == 1))
            {
                return true;
            }
            
            if (adminLevel == SecurityLevel.Admin && ((A & 1) == 1))
            {
                return true;
            }
            
            if (adminLevel == SecurityLevel.Nonadmin && ((N & 1) == 1))
            {
                return true;
            }
            return false;
        }
    }
}
