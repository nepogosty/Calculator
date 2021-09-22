using calc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace calc.Services
{
   public interface ICalculator
    {
       public  Calculator FilingDatas(Calculator _calculator);
        public void Savexml(Calculator calculator);

        public List<Calculator> Read();

        public List<Calculator> SaveTOxls(DateTime datestart, DateTime dateend, List<Calculator> calculatorr);
    }
}
