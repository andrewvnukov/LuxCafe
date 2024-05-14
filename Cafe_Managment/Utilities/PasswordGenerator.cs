using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment
{
    public class PasswordGenerator
    {
        // Генератор случайных чисел
        private static Random _random = new Random();

        // Метод для генерации безопасного пароля
        public static string GenerateSecurePassword(int length = 12)
        {
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_+=-[{]};:<>|./?,";

            // Создаем комбинацию всех возможных символов
            string allChars = lowerCase + upperCase + numbers + specialChars;

            // Генерируем пароль
            string password = new string(Enumerable.Repeat(allChars, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray());

            // Убедимся, что пароль содержит хотя бы одну заглавную букву, строчную букву, цифру и специальный символ
            if (!password.Any(char.IsUpper)) password = ReplaceCharacter(password, upperCase);
            if (!password.Any(char.IsLower)) password = ReplaceCharacter(password, lowerCase);
            if (!password.Any(char.IsDigit)) password = ReplaceCharacter(password, numbers);
            if (!password.Any(specialChars.Contains)) password = ReplaceCharacter(password, specialChars);

            return password;
        }

        // Метод для замены случайного символа в пароле
        private static string ReplaceCharacter(string password, string chars)
        {
            int index = _random.Next(password.Length);
            char replacement = chars[_random.Next(chars.Length)];
            char[] passwordArray = password.ToCharArray();
            passwordArray[index] = replacement;
            return new string(passwordArray);
        }
    }
}
