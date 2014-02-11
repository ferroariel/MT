using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace MT.Models
{
    // un objeto propio para almacenar toooda la data de perfil, recursos, empresas, etc
    //relacionado con el usuario actual
    public class DatosdeUsuario
    {
        public MT_RECURSO avatar { get; set; }
        public IList<MT_EMAIL> emails { get; set; }
        public IList<MT_EMPRESA> empresas { get; set; }
        public int nro_usuario { get; set; }
        public MT_USUARIO_PERFIL perfil { get; set; }
        public IList<MT_TELEFONO> tels { get; set; }
    }

    /// <summary>
    /// MODELO DE MEMBERSIA http://www.codeproject.com/Articles/165159/Custom-Membership-Providers
    /// </summary>
    public class Membresia : MembershipProvider
    {
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 1; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            // In a real application, you will essentially have to return true
            // and implement the GetUserNameByEmail method to identify duplicates
            get { return false; }
        }

        public override bool ChangePassword(string userName, string newPassword, string oldPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password,
               string email, string passwordQuestion, string passwordAnswer,
               bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != string.Empty)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser user = GetUser(username, true);

            if (user == null)
            {
                Varias v = new Varias();
                MT_USUARIO userObj = new MT_USUARIO()
                {
                    XDE_USERID = username,
                    XDE_CLAVE = v.HashClave(password)
                };
                MTrepository userRep = new MTrepository();
                userRep.RegisterUser(userObj);
                status = MembershipCreateStatus.Success;
                return GetUser(username, true);
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }

            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            MTrepository userRep = new MTrepository();
            MT_USUARIO user = userRep.GetAllUsers().SingleOrDefault
                    (u => u.XDE_USERID.Equals(username));
            if (user != null)
            {
                MembershipUser memUser = new MembershipUser("Membresia",
                                               username, user.XDE_USERID, user.XDE_USERID,
                                               string.Empty, string.Empty,
                                               true, false, DateTime.MinValue,
                                               DateTime.MinValue,
                                               DateTime.MinValue,
                                               DateTime.Now, DateTime.Now);
                return memUser;
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Esta función es la mas importante del Membership
        /// porque valida si el usuario es valido y, si lo es,
        /// graba una variable de sesion con el objeto MT_USUARIO_PERFIL
        /// que uso luego en las paginas del perfil
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool ValidateUser(string username, string password)
        {
            Varias v = new Varias();
            string sha1Pswd = v.HashClave(password);
            MTrepository userRep = new MTrepository();
            MT_USUARIO_PERFIL user = userRep.GetUserObjByUserName(username, sha1Pswd);

            //MT_USUARIO_PERFIL user = userRep.GetUserObjByUserName(username, password);
            if (user != null)
            {
                //HttpContext.Current.Session.Add("MT_U", user);
                return true;
            }
            return false;
        }
    }
}