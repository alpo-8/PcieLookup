using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PcieLookup
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = new List<Item>
            {
                new Item("GTX 1080 Ti", new Link(Lane.X16, To.CPU1), 3),
                new Item("GTX TITAN", new Link(Lane.X16, To.CPU2), 2),
                new Item("HYPER M.2 X16 CARD", new Link(Lane.X8, To.CPU2))
            };
            
            Permutate(devices, devices.Count).ToList()
                .ForEach(x=>Rig.Place(x.Cast<Item>()));
            
            Rig.Display();
            
        }

        private static IEnumerable<IList> Permutate(IList seq, int cnt)
        {
            if (cnt != 1)
                for (var i = 0; i < cnt; i++)
                {
                    foreach (var perm in Permutate(seq, cnt - 1))
                        yield return perm;
                    var tmp = seq[cnt - 1];
                    seq.RemoveAt(cnt - 1);
                    seq.Insert(0, tmp);
                }
            else
                yield return seq;
        }
    }
}