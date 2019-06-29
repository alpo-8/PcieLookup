using System;
using System.Collections.Generic;
using System.Linq;

namespace PcieLookup
{
    public static class Rig
    {
        private static readonly Dictionary<int, Link> Capability = new Dictionary<int, Link>
        {
            { 7, new Link(Lane.X16, To.CPU1)},
            { 6, new Link(Lane.X16, To.CPU2)},
            { 5, new Link(Lane.X16, To.CPU1)},
            { 4, new Link(Lane.X8, To.CPU2)},
            { 3, new Link(Lane.X16, To.CPU2)},
            { 2, new Link(Lane.X8, To.CPU1)},
            { 1, new Link(Lane.X16, To.CPU2)}
        };

        private static List<Dictionary<int, Item>> Configs { get; set; } =
            new List<Dictionary<int, Item>> {new Dictionary<int, Item>()};
        
        public static void Display() =>
            Configs.Distinct().ToList().ForEach(y =>
            {
                Console.WriteLine();
                y.OrderByDescending(x=>x.Key).ToList().ForEach(x => Console.WriteLine($"{x.Key.ToString()}\t{x.Value.Name}"));
            });

        private static void Place(Item item, Dictionary<int, Item> config)
        {
            var avail = Capability.Where(x => !config.ContainsKey(x.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);

            if (avail.ContainsKey(4) && config.ContainsKey(3) && config[3].Required.Lane == Lane.X16)
                avail[4].Lane = Lane.X0;
            if (avail.ContainsKey(3) && config.ContainsKey(4) && config[4].Required.Lane < Lane.X0)
                avail[3].Lane = Lane.X8;
            
            var res1 = //new List<int>();
                avail.Where(i =>
                i.Value.Lane <= item.Required.Lane && i.Value.To <= item.Required.To &&
                avail.Keys.Min() - 1 <= i.Key - item.Size).Select(kv => kv.Key).ToList();
            
            var res2 = res1.ToList();
            
            foreach (var slot in res1)
                for (var i = 0; i < item.Size; i++)
                    if (config.ContainsKey(slot - i))
                        res2.Remove(slot);

            foreach (var slot in res2)
            {
                var cfg = config.ToDictionary(kv => kv.Key, kv => kv.Value);
                for (var i = 0; i < item.Size; i++)
                    cfg.Add(slot-i, item);
                Configs.Add(cfg);
            }
            
            Configs.Remove(config);
        }

        private static void Place(Item item) =>
            Configs.ToList().Where(x => !x.ContainsValue(item)).ToList().ForEach(x => Place(item, x));

        public static void Place(IEnumerable<Item> items) => 
            items.ToList().ForEach(Place);
    }
}