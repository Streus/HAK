using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileSystemNS
{
    public enum SecurityLevel
    {
        ROOT = 0,
        ADMIN,
        NONADMIN
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
            this.username = user;
            this.password = pass;
            this.adminLevel = level;
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

        public bool canRead(int permissions)
        {
            int R = permissions / 100;
            int A = (permissions / 10) % 10;
            int N = permissions % 10;

            if (adminLevel == SecurityLevel.ROOT && ((R & 4) == 4))
            {
                return true;
            }
            else if (adminLevel == SecurityLevel.ADMIN && ((A & 4) == 4))
            {
                return true;
            }
            else if (adminLevel == SecurityLevel.NONADMIN && ((N & 4) == 4))
            {
                return true;
            }
            return false;
        }

        public bool canWrite(int permissions)
        {
            int R = permissions / 100;
            int A = (permissions / 10) % 10;
            int N = permissions % 10;

            if (adminLevel == SecurityLevel.ROOT && ((R & 2) == 2))
            {
                return true;
            }
            else if (adminLevel == SecurityLevel.ADMIN && ((A & 2) == 2))
            {
                return true;
            }
            else if (adminLevel == SecurityLevel.NONADMIN && ((N & 2) == 2))
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

            if (adminLevel == SecurityLevel.ROOT && ((R & 1) == 1))
            {
                return true;
            }
            else if (adminLevel == SecurityLevel.ADMIN && ((A & 1) == 1))
            {
                return true;
            }
            else if (adminLevel == SecurityLevel.NONADMIN && ((N & 1) == 1))
            {
                return true;
            }
            return false;
        }
    }
}
