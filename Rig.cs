using System;
using System.Collections.Generic;
using System.Linq;

namespace PcieLookup
{
    public static class Rig
    {
        public static Dictionary<int, Link> Capability = new Dictionary<int, Link>
        {
            { 7, Link(Lane.X16, To.CPU1)},
            { 6, Link(Lane.X16, To.CPU2)},
            { 5, Link(Lane.X16, To.CPU1)},
            { 4, Link(Lane.X8, To.CPU2)},
            { 3, Link(Lane.X16, To.CPU2)},
            { 2, Link(Lane.X8, To.CPU1)},
            { 1, Link(Lane.X16, To.CPU2)}
        };
        
        private static Link Link(Lane lane, To to) =>
            new Link(lane, to);
        
        public static void Display() =>
            Configs.Distinct().ToList().ForEach(y =>
            {
                Console.WriteLine();
                y.OrderByDescending(x=>x.Key).ToList().ForEach(x => Console.WriteLine($"{x.Key.ToString()}\t{x.Value.Name}"));
            });

        private static List<Dictionary<int, Item>> Configs { get; set; }

        private static void Place(Item item, Dictionary<int, Item> config)
        {
            var avail = Capability;
            foreach (var (busySlot, v) in config)
                avail.Remove(busySlot);

            if (avail.ContainsKey(4) && config.ContainsKey(3) && config[3].Required.Lane == Lane.X16)
                avail[4].Lane = Lane.X0;
            if (avail.ContainsKey(3) && config.ContainsKey(4) && config[4].Required.Lane < Lane.X0)
                avail[3].Lane = Lane.X8;
            
            var res1 = new List<int>();
            
            foreach (var (key, value) in avail)
                if (value.Lane<=item.Required.Lane && value.To<=item.Required.To && key+item.Size-1<=avail.Keys.Max())
                    res1.Add(key);
            
            var res2 = res1;
            
            foreach (var slot in res1)
                for (var i = 0; i < item.Size; i++)
                    if (config.ContainsKey(slot + i))
                        res2.Remove(slot);
            
            foreach (var slot in res2)
            {
                var cfg = config.ToDictionary(kv => kv.Key, kv => kv.Value);
                for (var i = 0; i < item.Size; i++)
                    cfg.Add(slot+i, item);
                Configs.Add(cfg);
            }
            
            Configs.Remove(config);
        }

        private static void Place(Item item)
        {
            Configs ??= new List<Dictionary<int, Item>> {new Dictionary<int, Item>()};
            Configs.ToList().Where(x => !x.ContainsValue(item)).ToList().ForEach(x => Place(item, x));
        }

        public static void Place(IEnumerable<Item> items) => 
            items.ToList().ForEach(Place);
    }
}