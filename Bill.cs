using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPR
{
    public class Bill
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int planeId { get; set; }
        public int days { get; set; }
        public int totalPrice { get; set; }
        public DateTime date { get; set; }
        public bool isRentNow { get; set; }

        public Bill() { }
        public Bill(int userId, int planeId, int days, int totalPrice, DateTime date, bool isRentNow)
        {
            this.userId = userId;
            this.planeId = planeId;
            this.days = days;
            this.totalPrice = totalPrice;
            this.date = date;
            this.isRentNow = isRentNow;
        }
    }
}
