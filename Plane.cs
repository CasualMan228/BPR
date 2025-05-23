using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPR
{
    public class Plane //сделал public для того, чтобы передавать экземпляр этого класса как аргумент
    {
        public int id { get; set; }
        public string name { get; set; }
        public int year { get; set; }
        public string maker { get; set; }
        public string regnum { get; set; }
        public string country { get; set; }
        public string type { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public int totalFly { get; set; }
        public int price { get; set; }
        public string photoNeed { get; set; }
        public string photo1 { get; set; }
        public string photo2 { get; set; }
        public Plane() { }
        public Plane(string name, int year, string maker, string regnum, string country, string type,
            string category, string description, int totalFly, int price, string photoNeed, string photo1, string photo2)
        {
            this.name = name;
            this.year = year;
            this.maker = maker;
            this.regnum = regnum;
            this.country = country;
            this.type = type;
            this.category = category;
            this.description = description;
            this.totalFly = totalFly;
            this.price = price;
            this.photoNeed = photoNeed;
            this.photo1 = photo1;
            this.photo2 = photo2;
        }
    }
}
