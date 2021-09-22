using calc.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace calc.Models
{
    public class Actions:ICalculator
    {
        static object locker = new object();

        Calculator ICalculator.FilingDatas(Calculator _calculator)
        {
            lock (locker)
            {
                Calculator calculator = _calculator;
                switch (calculator.Action)
                {
                    case Models.Action.Add:
                        calculator.Result = calculator.Fterm + calculator.Sterm;
                        calculator.AAction = "+";
                        break;
                    case Models.Action.Subtract:
                        calculator.Result = calculator.Fterm - calculator.Sterm;
                        calculator.AAction = "-";
                        break;
                    case Models.Action.Multiply:
                        calculator.Result = calculator.Fterm * calculator.Sterm;
                        calculator.AAction = "*";
                        break;
                    case Models.Action.Divide:
                        calculator.Result = calculator.Fterm / calculator.Sterm;
                        calculator.AAction = "/";
                        break;
                }

                //Получение IP
                string Host = System.Net.Dns.GetHostName();
                calculator.IP = System.Net.Dns.GetHostByName(Host).AddressList[1].ToString();

                // Дата выполнения
                DateTime date = DateTime.Now;
                calculator.dateTime = date;

                calculator.Solution = Convert.ToString(calculator.Fterm + calculator.AAction + calculator.Sterm + "=" + calculator.Result);

                return calculator;
            }
        }

        public  void Savexml(Calculator calculator)
        {
            lock (locker)
            {

                //Запись в файл .xml
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("bin/Debug/datas.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                XmlElement act = xDoc.CreateElement("ace");

                XmlAttribute datet = xDoc.CreateAttribute("date");
                XmlElement first = xDoc.CreateElement("fterm");
                XmlElement second = xDoc.CreateElement("sterm");
                XmlElement result = xDoc.CreateElement("result");
                XmlElement action = xDoc.CreateElement("action");
                XmlElement solution = xDoc.CreateElement("solution");
                XmlElement ip = xDoc.CreateElement("ip");

                XmlText firsttext = xDoc.CreateTextNode(Convert.ToString(calculator.Fterm));
                XmlText secondtext = xDoc.CreateTextNode(Convert.ToString(calculator.Sterm));
                XmlText resulttext = xDoc.CreateTextNode(Convert.ToString(calculator.Result));
                XmlText actiontext = xDoc.CreateTextNode(Convert.ToString(calculator.AAction));
                XmlText solutiontext = xDoc.CreateTextNode(Convert.ToString(calculator.Solution));
                XmlText ipttext = xDoc.CreateTextNode(Convert.ToString(calculator.IP));
                XmlText datetext = xDoc.CreateTextNode(calculator.dateTime.ToString());

                first.AppendChild(firsttext);
                second.AppendChild(secondtext);
                result.AppendChild(resulttext);
                action.AppendChild(actiontext);
                solution.AppendChild(solutiontext);
                ip.AppendChild(ipttext);
                datet.AppendChild(datetext);

                act.Attributes.Append(datet);
                act.AppendChild(first);
                act.AppendChild(second);
                act.AppendChild(solution);
                act.AppendChild(ip);
                act.AppendChild(actiontext);
                act.AppendChild(result);

                xRoot.AppendChild(act);
                xDoc.Save(@"bin/Debug/datas.xml");
            }
        }

        public  List<Calculator> Read()
        {
            lock (locker)
            {

                //Чтение из .xml
                XmlDocument xDocc = new XmlDocument();
                xDocc.Load(@"bin/Debug/datas.xml");
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

        public  List<Calculator> SaveTOxls(DateTime datestart, DateTime dateend, List<Calculator> calculatorr)
        {
            lock (locker) { 
                List<Calculator> calinterval = new List<Calculator>();
                foreach (var c in calculatorr)
                {
                    if (Convert.ToDateTime(c.dateTime) >= datestart && Convert.ToDateTime(c.dateTime) <= dateend)
                    {
                        calinterval.Add(c);
                    }
                }
                var list = calinterval.OrderByDescending(x => x.dateTime);

                //Создание .xlsx отчета

                //Создание файла в определенном пути
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string fileName = "datas.xml";
                string fullPath = Path.GetFullPath(fileName);
                fullPath = fullPath.Substring(0, fullPath.Length - 9);
                fullPath = fullPath + @"reports\" + System.Guid.NewGuid() + ".xlsx";
                var fi = new FileInfo(fullPath);

                //Формирование отчета

                //Список действий по интервалу
                using (var p = new ExcelPackage(fi))
                {
                    var ws = p.Workbook.Worksheets.Add("Отчет");


                    ws.Cells["A1"].Value = "c " + Convert.ToString(datestart);
                    ws.Cells["C1"].Value = "по " + Convert.ToString(dateend);
                    ws.Cells["A2"].Value = "Время";
                    ws.Cells["B2"].Value = "Вычисление";
                    ws.Cells["C2"].Value = "IP";
                    ws.Cells["E1"].Value = "К-во вычислений по часам";
                    ws.Column(1).Width = 20; //Размер ячеек
                    ws.Column(2).Width = 35;
                    ws.Column(3).Width = 35;
                    ws.Column(4).Width = 20;
                    ws.Column(5).Width = 20;
                    ws.Column(6).Width = 20;
                    ws.Cells[1, 1, 2, 3].Style.Font.Bold = true;
                    ws.Cells["E1"].Style.Font.Bold = true;


                    int row = 3;
                    int collumn = 1;
                    foreach (var item in calinterval)
                    {
                        ws.Cells[row, collumn].Value = Convert.ToString(item.dateTime);
                        ws.Cells[row, collumn + 1].Value = item.Solution;
                        ws.Cells[row, collumn + 2].Value = item.IP;
                        row++;
                    }
                    int totalhours = 0;
                    var interval = dateend - datestart;
                    if (Math.Truncate(interval.TotalHours) != interval.TotalHours)
                    {
                        totalhours = Convert.ToInt32(Math.Truncate(interval.TotalHours)) + 1;
                    }
                    else totalhours = Convert.ToInt32(Math.Truncate(interval.TotalHours));

                    //Количество действий по интервалу (Если интервал к примеру 21.07.2021 22:25 по  21.07.2021 22:26,
                    // то будет выводится количество действий с 21.07.2021 22:25 по 21.07.2021 23:25 (т.е +1ч))
                    row = 2;
                    collumn = 5;

                    DateTime dateTime = datestart;
                    for (int i = 1; i <= totalhours; i++)
                    {
                        DateTime datehour = dateTime.AddHours(1);

                        ws.Cells[row, collumn].Value = Convert.ToString(dateTime);
                        ws.Cells[row, collumn + 1].Value = Convert.ToString(datehour);

                        int chislo = 0;
                        foreach (var item in calculatorr)
                        {
                            if (item.dateTime >= dateTime && item.dateTime <= datehour)
                            {
                                chislo++;
                            }
                        }
                        ws.Cells[row, collumn + 2].Value = Convert.ToString(chislo);
                        row += 1;
                        dateTime = datehour;
                    }
                    p.Save();

                    return list.ToList();

                }
            }
        }

      
    }
}
