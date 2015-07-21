using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string RemoveHtmlTags(this string html)
        {
            var builder = new StringBuilder();
            var inside = true;
            foreach (var item in html.ToCharArray())
            {
                switch (item)
                {
                    case '<':
                        inside = false;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                    default:
                        if (inside)
                        {
                            builder.Append(item);
                        }
                        break;
                }
            }
            return builder.ToString();
        }

        public static char charAt(this string value, int index)
        {
            return value[index];
        }
    }
}