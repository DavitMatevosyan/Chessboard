using System;

namespace Chessboard.Data.Hash
{
    public static class Hasher
    {
        /// <summary>
        /// Gets the Custom hashcode of the given password
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string GetHash(string pass)
        {
            var a = HashingString(pass);
            var binNumber = Convert.ToInt64(a, 2);
            return binNumber.ToString();
        }

        static string HashingString(string text)
        {
            int numberOfIterations = text.Length / 4;
            numberOfIterations += text.Length % 4 >= 1 ? 1 : 0;
            string result = "";
            string hash = "";
            string resultBinary = "";
            for (int i = 0; i < numberOfIterations; i++)
            {
                int index = i;
                string code = "";
                int startIndex = 3;
                if (index >= (text.Length / 4))
                    startIndex = text.Length - (text.Length);
                for (int j = startIndex; j >= 0; j--)
                {
                    index = (i * 4) + j;
                    char txt = text[index];
                    code = Convert.ToString(txt, 2);
                    code = CheckCodeValidation(code);
                    result += code;
                }
                var binNumber = Convert.ToInt64(result, 2);

                hash = binNumber.ToString();
                resultBinary = addBinary(result, resultBinary);
                result = "";
                Console.WriteLine(hash);
            }
            return resultBinary;
        }

        private static string CheckCodeValidation(string code)
        {
            while (code.Length < 8)
            {
                code = "0" + code;
            }
            return code;
        }

        static string addBinary(string a, string b)
        {
            string result = "";
            int s = 0;

            int i = a.Length - 1, j = b.Length - 1;
            while (i >= 0 || j >= 0 || s == 1)
            {
                s += ((i >= 0) ? a[i] - '0' : 0);
                s += ((j >= 0) ? b[j] - '0' : 0);

                result = (char)(s % 2 + '0') + result;
                s /= 2;
                i--; j--;
            }
            return result;
        }
    }
}
