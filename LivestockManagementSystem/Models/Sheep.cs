using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Models
{
    public class Sheep : Animal
    {
        public double Wool { get; set; }   // kg per day
        public override double Income => Wool * 6.2;
        public override double ProduceAmount => Wool;

    }
}
