namespace PowerSet.Attribute
{
    class ColSetAttribute : System.Attribute
    {
        public string Name { get; set; }
        public bool Show { get; set; }

        public string DefaultVal { get; set; } = "";

        public ColSetAttribute(string name = null, bool show = true)
        {
            Name = name;
            Show = show;
        }
    }
}
