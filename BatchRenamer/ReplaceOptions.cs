namespace BatchRenamer
{
    public struct ReplaceOptions
    {
        public string Find { get; private set; }
        public string Replace { get; private set; }
        public bool IgnoreCase { get; private set; }

        public ReplaceOptions(string find, string replace, bool ignoreCase)
        {
            Find = find;
            Replace = replace;
            IgnoreCase = ignoreCase;
        }
    }
}
