using Dripple.Logic;
using Dripple.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dripple.Controllers
{
    public class DrippleController : Controller
    {
        private readonly DrippleEngine Dripple;

        public DrippleController(DrippleEngine DrippleEngine)
        {
            this.Dripple = DrippleEngine;
        }

        public IActionResult Stats()
        {
            ViewData["dripple"] = Dripple;

            return View(Dripple);
        }

        public IActionResult DonatePool()
        {
            return View();
        }

        public IActionResult Buy()
        {
            return View();
        }

        public IActionResult Reinvest()
        {
            ReinvestModel reinvestModel = new ReinvestModel
            {
                AvailableAddresses = Dripple.Players.Select(x => x.Address).ToList().Select(x => new SelectListItem(x.ToString(), x.ToString())).ToList()
            };

            return View(reinvestModel);
        }

        [HttpPost]
        public IActionResult DonatePool(DonatePoolModel donatePoolModel)
        {
            Dripple.DonatePool(donatePoolModel.Ammount);

            return RedirectToAction("Stats");
        }

        [HttpPost]
        public IActionResult Buy(BuyModel buyModel)
        {
            Dripple.Buy(buyModel.Address, buyModel.Ammount);

            return RedirectToAction("Stats");
        }

        [HttpPost]
        public IActionResult Reinvest(ReinvestModel reinvestModel)
        {
            Dripple.Reinvest(reinvestModel.Address);

            return RedirectToAction("Stats");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}