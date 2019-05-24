using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecms.Core
{
    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        public static string[] Wrap(this string text, int length)
        {
            if (text == null || text.Length <= length)
                return new string[] { text };

            string[] words = text.Split(' ');
            IList<string> sentenceParts = new List<string>();
            sentenceParts.Add(string.Empty);

            int partCounter = 0;

            foreach (string word in words)
            {
                if ((sentenceParts[partCounter] + word).Length > length)
                {
                    partCounter++;
                    sentenceParts.Add(string.Empty);
                }

                sentenceParts[partCounter] += word + " ";
            }

            return sentenceParts.ToArray();
        }
    }
}
