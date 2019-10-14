using System.Linq;
using System.Collections.Generic;

namespace PcieLookup
{
    static class Program
    {
        static void Main()
        {
            var mb = new MBoard(new []
            {
                (7, Lane.X16, To.CPU1),
                (6, Lane.X16, To.CPU2),
                (5, Lane.X16, To.CPU1),
                (4, Lane.X8, To.CPU2),
                (3, Lane.X16, To.CPU2),
                (2, Lane.X8, To.CPU1),
                (1, Lane.X16, To.CPU2)
            });
            
            mb.Rule((3,4),(Lane.X16,Lane.X0));
            mb.Rule((3,4),(Lane.X8,Lane.X8));
            
            var devices = new List<Item>
            {
                new Item("GTX 1080 Ti", new Link(Lane.X16, To.CPU1), 3),
                new Item("GTX TITAN", new Link(Lane.X16, To.CPU2), 2),
                new Item("HYPER M.2 X16 CARD", new Link(Lane.X16, To.CPU2))
            };
            
            Combine(devices, devices.Count)
                .ToList()
                .ForEach(mb.Place);
            
            mb.Display();
        }

        private static IEnumerable<IEnumerable<T>> Combine<T>(IEnumerable<T> list, int l)
            => l == 1
                ? list.Select(t => new[] {t})
                : Combine(list, l - 1)
                    .SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new[] {t2}));
    }
}