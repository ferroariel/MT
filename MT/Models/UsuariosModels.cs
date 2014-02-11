using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;

namespace MT.Models
{
    #region modelos

    public class ChangePasswordModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva clave")]
        [Compare("NewPassword", ErrorMessage = "Las clavase no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La clave debe ser de al menos {2} caracteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva clave")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Clave actual")]
        public string OldPassword { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail")]
        public string Email { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Clave")]
        public string Password { get; set; }

        [Display(Name = "Recordar")]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        public string UserName { get; set; }
    }

    public class RegisterModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar clave")]
        [Compare("Password", ErrorMessage = "Las claves no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La clave debe ser de al menos {2} caracteres de largo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Clave")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Dirección de Email")]
        public string UserName { get; set; }

        /*
        [Required]
        [StringLength(150, ErrorMessage = "Ingrese su Nombre.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Ingrese su Apellido.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }
         * */
    }

    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Reset Code")]
        public string ResetCode { get; set; }
    }

    public class TempUserData
    {
        public string UserName { get; set; }

        public string UserResetCode { get; set; }

        public string UserTempPass { get; set; }
    }

    #endregion modelos

    #region Services

    // El tipo FormsAuthentication está sellado y contiene miembros estáticos, por lo que es difícil
    // realizar pruebas unitarias en el código que llama a sus miembros. La interfaz y la clase auxiliar siguientes muestran
    // cómo crear un contenedor abstracto en torno a un tipo como este para que puedan realizarse pruebas unitarias en el código de AccountController
    // .

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);

        void SignOut();
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ChangePassword(string userName, string oldPassword, string newPassword);

        MembershipCreateStatus CreateUser(string userName, string password, string email);

        bool ValidateUser(string userName, string password);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "newPassword");

            // El elemento ChangePassword() subyacente iniciará una excepción en lugar de
            // devolver false en determinados escenarios de error.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public void GetUsernameFromResetCode(ref TempUserData ud)
        {
            TextReader tw;
            try
            {
                tw = new StreamReader(String.Format(@"{0}\Usuarios\TempData\{1}.txt", ConfigurationManager.AppSettings["SitePath"].ToString(), ud.UserResetCode));
                ud.UserName = tw.ReadLine();
                ud.UserTempPass = tw.ReadLine();
                tw.Close();
                tw.Dispose();
                File.Delete(String.Format(@"{0}\Usuarios\TempData\{1}.txt", ConfigurationManager.AppSettings["SitePath"].ToString(), ud.UserResetCode));
            }
            catch (Exception ex)
            {
                string exm = ex.Message;
            }
            finally
            {
                tw = null;
            }
        }

        public void SetUsernameFromResetCode(string resetcode, string username, string userpass)
        {
            if (resetcode.Length > 1)
            {
                TextWriter tw;
                try
                {
                    tw = new StreamWriter(String.Format(@"{0}\Usuarios\TempData\{1}.txt", ConfigurationManager.AppSettings["SitePath"].ToString(), resetcode));
                    tw.WriteLine(username);
                    tw.WriteLine(userpass);
                    tw.Close();
                    tw.Dispose();
                }
                catch
                {
                }
                finally
                {
                    tw = null;
                }
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "password");
            try
            {
                return _provider.ValidateUser(userName, password);
            }
            catch
            {
                return false;
            }
        }
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    #endregion Services

    #region Validation

    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Vaya a http://go.microsoft.com/fwlink/?LinkID=177550 para
            // obtener una lista completa de códigos de estado.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El nombre de usuario ya existe. Escriba otro nombre de usuario.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ya existe un nombre de usuario para esa dirección de correo electrónico. Especifique otra dirección de correo electrónico.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contraseña especificada no es válida. Escriba un valor de contraseña válido.";

                case MembershipCreateStatus.InvalidEmail:
                    return "La dirección de correo electrónico especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La respuesta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La pregunta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidUserName:
                    return "El nombre de usuario especificado no es válido. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.ProviderError:
                    return "El proveedor de autenticación devolvió un error. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                case MembershipCreateStatus.UserRejected:
                    return "La solicitud de creación de usuario se ha cancelado. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                default:
                    return "Error desconocido. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' y '{1}' no coinciden.";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }

        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' debe tener al menos {1} caracteres.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }

    #endregion Validation
}