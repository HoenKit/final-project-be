using Azure.Core;
using System.Text.RegularExpressions;

namespace final_project_be.Ultils
{
    public class Validate
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Validate(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public  bool IsValidToken()
        {
            var token = _httpContextAccessor?.HttpContext?.Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            return true;
        }

        public static bool IsNotNullOrEmpty(string input)
        {
            return !string.IsNullOrEmpty(input);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
        public static bool EmailExist(string email, List<string> existingEmails)
        {
            if (string.IsNullOrEmpty(email)) return false;

            return existingEmails.Any(e => e.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            string passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        public static bool IsNumberInRange(int number, int min, int max)
        {
            return number >= min && number <= max;
        }

        public static bool IsFutureDate(DateTime date)
        {
            return date > DateTime.Now;
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return false;

            string phonePattern = @"^\+?[1-9]\d{1,14}$"; 
            return Regex.IsMatch(phoneNumber, phonePattern);
        }
        public static bool DoesUserExist(string userId, List<string> existingUserIds)
        {
            if (string.IsNullOrEmpty(userId)) return false;

            return existingUserIds.Contains(userId);
        }
    }
}

