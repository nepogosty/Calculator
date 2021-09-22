using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
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


       
    }
    public enum Action
    {
        Add = '+',
        Subtract = '-',
        Multiply = '*',
        Divide = '/'
    }
   

}
