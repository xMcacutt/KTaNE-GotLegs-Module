namespace KModkit
{
    public static class Utility
    {
        public static int Mod(this int a, int n)
        {
            int result = a % n;
            return result < 0 ? result + n : result;
        }
        
    }
}