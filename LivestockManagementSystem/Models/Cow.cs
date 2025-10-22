using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Models
{
    public class Cow : Animal
    {

        public double Milk { get; set; } // kg per day

        public override double Income => Milk * 9.4;
        public override double ProduceAmount => Milk;

    }
}
