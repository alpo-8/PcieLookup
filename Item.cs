namespace PcieLookup
{
    public sealed class Item
    {
        public string Name { get; set; }
        public Link Required { get; set; }
        public int Size { get; set; }

        public Item(string name, Link req, int size = 1)
        {
            Name = name;
            Required = req;
            Size = size;
        }
    }
}