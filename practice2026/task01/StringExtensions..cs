namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            string[] stringlower = input.ToLower().Split(' ');
            List<string> liststring = new List<string>(); bool polindrom = true;
            for (int i = 0; i < stringlower.Length; i++)
            {
                liststring.Add(stringlower[i]);
            }
            for (int i = 0; i < stringlower.Length; i++)
            {
                List<char> chars = liststring[i].ToList();
                for (int j = 0; j < chars.Count; j++)
                {
                    if (char.IsPunctuation(chars[j]))
                    {
                        chars.Remove(chars[j]);
                    }
                }
                string nopunctuation = new string(chars.Where(x => x != ' ').ToArray());
                liststring[i] = nopunctuation;
            }
            string proverka = string.Join("", liststring);
            char[] simvol = proverka.ToCharArray();
            for (int i = 0; i < simvol.Length; i++)
            {
                if (simvol[i] != simvol[simvol.Length - 1 - i]) { polindrom = false; }
            }
            return polindrom;
        }
    }
}
