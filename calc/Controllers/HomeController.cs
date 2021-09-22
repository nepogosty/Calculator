using calc.Models;
using calc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace calc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ICalculator actions;

        public HomeController(ILogger<HomeController> logger, ICalculator actions2)
        {
            _logger = logger;
            actions = actions2;
        }
       
        public IActionResult Index()
        {

            return View();
        }
        
        [HttpPost]
        public  ActionResult Index(Calculator calculator)
        {
            ModelState.Clear();

            calculator = actions.FilingDatas(calculator);

            actions.Savexml(calculator);

            return View(calculator);
        }
        
        public IActionResult Report()
        {
            //Calculator.Read() ---- метод, который читает данные из .xml
            var list = actions.Read().OrderByDescending(x => x.dateTime);
            return View(list.ToList()); 
        }  

        [HttpPost]
        public ActionResult Report( DateTime datestart, DateTime dateend)
        {
            // Новый List с данными в конкретном интервале
           
            var calculatorr = actions.Read();

            var list = actions.SaveTOxls(datestart, dateend, calculatorr);
            return View(list.ToList());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }

       
    }
}
