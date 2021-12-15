namespace PcieLookup
{
    public sealed class Link
    {
        public Lane Lane { get; set; }
        public To To { get; set; }
        // comment
        public Link(Lane lane, To to)
        {
            Lane = lane;
            To = to;
        }
    }
}
