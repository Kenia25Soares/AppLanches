using System.Text.RegularExpressions;

namespace AppLanches.Validations
{
    public class Validator : IValidator
    {
        public string NameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneError { get; set; } = "";
        public string PasswordError { get; set; } = "";


        private const string EmptyNameErrorMsg = "Por favor, informe o seu nome.";
        private const string InvalidNameErrorMsg = "Por favor, informe um nome válido.";
        private const string EmptyEmailErrorMsg = "Por favor, informe um email.";
        private const string InvalidEmailErrorMsg = "Por favor, informe um email válido.";
        private const string EmptyPhoneErrorMsg = "Por favor, informe um telefone";
        private const string InvalidPhoneErrorMsg = "Por favor, informe um telefone válido.";
        private const string EmptyPasswordErrorMsg = "Por favor, informe a password.";
        private const string InvalidPasswordErrorMsg = "A Password deve conter pelo menos 8 caracteres, incluindo letras e números.";


        public Task<bool> Validate(string name, string email, string phoneNumber, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isPhoneValid = ValidatePhoneNumber(phoneNumber);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isPhoneValid && isPasswordValid);
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = InvalidNameErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                PhoneError = EmptyPhoneErrorMsg;
                return false;
            }

            if (phoneNumber.Length < 12)
            {
                PhoneError = InvalidPhoneErrorMsg;
                return false;
            }

            PhoneError = "";
            return true;
        }

        private bool ValidatePassword(string senha)
        {
            if (string.IsNullOrEmpty(senha))
            {
                PasswordError = EmptyPasswordErrorMsg;
                return false;
            }

            if (senha.Length < 8 || !Regex.IsMatch(senha, @"[a-zA-Z]") || !Regex.IsMatch(senha, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }
    }

}
