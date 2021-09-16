using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace calc.Models
{
    public class Calculator
    {
        [Required]
       public double Fterm { get; set; }
        [Required]
        public double Sterm { get; set; }
        public Action Action { get; set; }

        public string AAction { get; set; }
        public double Result { get; set; }

        public string Solution { get; set; }

        public string IP { get; set; }

        public DateTime dateTime { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.DateTime)]
        public DateTime StartBT { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.DateTime)]
        public DateTime EndBT { get; set; }

        public static List<Calculator> Read()
        {
            //Чтение из .xml
            XmlDocument xDocc = new XmlDocument();
            xDocc.Load("datas.xml");
            int count = xDocc.SelectNodes("rootElement/ace").Count;

            List<Calculator> calculatorr = new List<Calculator>(count);

            XmlElement xRoot = xDocc.DocumentElement;

            foreach (XmlNode xnode in xRoot)
            {
                // получаем атрибут date
                Calculator cal = new Calculator();
                XmlNode attr = xnode.Attributes.GetNamedItem("date");
                if (attr != null)
                {
                    cal.dateTime = Convert.ToDateTime(attr.Value);
                }

                //// обходим все дочерние узлы элемента ace
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    // если узел - solution
                    if (childnode.Name == "solution")
                    {
                        cal.Solution = childnode.InnerText;
                    }
                    // если узел ip
                    if (childnode.Name == "ip")
                    {
                        cal.IP = childnode.InnerText;
                    }
                }
                calculatorr.Add(cal);
            }
            return (calculatorr);
        }

    }
    public enum Action
    {
        Add = '+',
        Subtract = '-',
        Multiply = '*',
        Divide = '/'
    }
   

}
