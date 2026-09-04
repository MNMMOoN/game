namespace WaddleRush.AI
{
    public static class BotNames
    {
        static readonly string[] Prefix = { "Ice","Fish","Snow","Pingu","Frost","Waddle","Chilly","Aurora","Glacier","Polar","Tundra","Arctic" };
        static readonly string[] Suffix = { "King","Hunter","Byte","Pro","Boss","Blade","Storm","Dash","Ace","Rider" };
        public const int Count = 120;
        public static string Get(int index) => Prefix[(index / Suffix.Length) % Prefix.Length] + Suffix[index % Suffix.Length];
    }
}
