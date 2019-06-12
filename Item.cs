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
        X16,
        X8,
        X4,
        X0
    }

    public enum To
    {
        CPU1,
        CPU2,
        PCH
    }
}