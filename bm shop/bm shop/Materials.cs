using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm_shop
{
    public class Materials
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string color { get; set; }
        public string weigth { get; set; }
        public double cost { get; set; }
        public string advantages { get; set; }
        public string characteristics { get; set; }
        public string modeOfApplication { get; set; }
        public string storage { get; set; }
        public int quantity { get; set; }
        public string photo { get; set; }

        public Materials()
        {

        }

        public Materials(int id, string name, string description, string color, string weigth, double cost, string advantages, string characteristics, string modeOfApplication, string storage, int quantity, string photo)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.color = color;
            this.weigth = weigth;
            this.cost = cost;
            this.advantages = advantages;
            this.characteristics = characteristics;
            this.modeOfApplication = modeOfApplication;
            this.storage = storage;
            this.quantity = quantity;
            this.photo = photo;
        }
    }
}
