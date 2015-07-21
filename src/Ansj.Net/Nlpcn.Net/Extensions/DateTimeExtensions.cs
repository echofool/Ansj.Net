namespace System
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime Start = new DateTime(1970, 1, 1);

        public static long CurrentTimeMillis(this DateTime dateTime)
        {
            return (long) (dateTime - Start).TotalMilliseconds;
        }
    }
}