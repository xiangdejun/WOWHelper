using System.Collections.Generic;

namespace WOWHelperWPF.Models
{
    public class Hero
    {
        public string Name { get; set; }

        public string ServerName { get; set; }

        public string Faction { get; set; }

        public string race { get; set; }

        public string Gender { get; set; }

        public int Level { get; set; }

        public string Vocation { get; set; }

        public List<Reputaion> Reputations { get; set; }

    }
}
