using MMOGS.Encryption;
using System;

namespace MMOGS.Models.Database
{
    public class DbAccountsData
    {
        public enum PasswordType
        {
            Encrypted,
            Decrypted
        }

        public int AccId { get; private set; } = -1;
        public string Login { get; private set; } = "";
        public string PassEncrypted { get; private set; } = "";
        public string PassDecrypted { get; private set; } = "";
        public int AccessLevel { get; private set; } = 0;

        public DbAccountsData
        (
            int accId,
            string login,
            string pass,
            PasswordType passType,
            int accessLevel
        )
        {
            this.AccId = accId;
            this.Login = login;
            this.AccessLevel = accessLevel;

            switch (passType)
            {
                case PasswordType.Decrypted:
                    {
                        this.PassDecrypted = pass;
                        this.PassEncrypted =
                        (
                            !String.IsNullOrEmpty(pass) ?
                            Crypto.EncryptStringAES(pass, GlobalData.PassKey) :
                            ""
                        );
                    }
                    break;
                case PasswordType.Encrypted:
                    {
                        this.PassEncrypted = pass;
                        this.PassDecrypted =
                        (
                            !String.IsNullOrEmpty(pass) ?
                            Crypto.DecryptStringAES(pass, GlobalData.PassKey) :
                            ""
                        );
                    }
                    break;
            }
        }
    }
}
