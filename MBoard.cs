using System;
using System.Linq;
using System.Collections.Generic;

namespace PcieLookup
{
    public class MBoard
    {
        private Dictionary<int, Link> Capability { get; set; }
        
        private static Dictionary<(int c, Lane cl), (int r, Lane rl)> Rulebook =>
            new Dictionary<(int, Lane), (int, Lane)>();
        
        public MBoard(IEnumerable<(int id, Lane lane, To to)> cap) =>
            Capability = cap
                .ToDictionary(x => x.id, x => new Link(x.lane, x.to));

        public void Rule((int, int) slot, (Lane, Lane) lanes)
        {
            var (c, r) = slot;
            if (Capability[c].To != Capability[r].To) return;
            var (lc, lr) = lanes;
            Rulebook.Add((c,lc),(r,lr));
            Rulebook.Add((r,lr),(c,lc));
        }

        private Dictionary<int, Link> ApplyRulebook(Dictionary<int, Item> cfg)
        {
            var avail = Capability
                .Where(x => !cfg.ContainsKey(x.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            
            foreach (var ((c, lc), (r, lr)) in Rulebook)
                if (cfg.ContainsKey(c) && cfg[c].Required.Lane == lc && avail.ContainsKey(r))
                    avail[r].Lane = lr;
            return avail;
        }
        
        private List<Dictionary<int, Item>> Configs { get; set; } =
            new List<Dictionary<int, Item>> {new Dictionary<int, Item>()};
        
        public void Display() =>
            Configs.Distinct().ToList().ForEach(y =>
            {
                Console.WriteLine();
                y.OrderByDescending(x => x.Key).ToList()
                    .ForEach(x => Console.WriteLine($"{x.Key.ToString()}\t{x.Value.Name}"));
            });

        private void Place(Item item, Dictionary<int, Item> config)
        {
            var avail = ApplyRulebook(config);
            
            var res1 = avail
                .Where(i => i.Value.Lane <= item.Required.Lane && 
                            i.Value.To <= item.Required.To &&
                            avail.Keys.Min() - 1 <= i.Key - item.Size)
                .Select(kv => kv.Key).ToList();
            
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

        private void Place(Item item) =>
            Configs.Where(x => !x.ContainsValue(item)).ToList()
                .ForEach(x => Place(item, x));

        public void Place(IEnumerable<Item> items) => 
            items.ToList()
                .ForEach(Place);
    }
}