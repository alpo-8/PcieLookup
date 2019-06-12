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

    public sealed class Link
    {
        public Lane Lane { get; set; }
        public To To { get; set; }

        public Link(Lane lane, To to)
        {
            Lane = lane;
            To = to;
        }
    }

    public enum Lane
    {
        X16 = 0,
        X8 = 1,
        X4 = 2,
        X0 = 3
    }

    public enum To
    {
        CPU1 = 0,
        CPU2 = 1,
        PCH = 2
    }
}