using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WOWHelperWPF.Models;
using System.IO;

namespace WOWHelperWPF.DataPersistence
{
    public class ModelToJson
    {
        public static void SaveToLocal(Hero hero)
        {
            var strJson = JsonConvert.SerializeObject(hero);
            File.WriteAllText(hero.Name, strJson);
        }

        public static Hero LoadJson(string heroName)
        {
            var strJson = File.ReadAllText(heroName);
            var hero = JsonConvert.DeserializeObject<Hero>(strJson);
            return hero;
        }

    }
}
