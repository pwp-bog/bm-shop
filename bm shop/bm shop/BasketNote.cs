using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm_shop
{
    public class BasketNote
    {
        public int id { get; set; }
        public int materialId { get; set; }
        public int userId { get; set; }
        public int quantity { get; set; }
        public bool selected { get; set; }

        public BasketNote()
        {

        }

        public BasketNote(int id, int materialId, int userId, int quantity, bool selected)
        {
            this.id = id;
            this.materialId = materialId;
            this.userId = userId;
            this.quantity = quantity;
            this.selected = selected;
        }
    }
}
