using Dripple.Logic;
using Dripple.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        [HttpPost]
        public IActionResult DonatePool(DonatePoolModel donatePoolModel)
        {
            Dripple.DonatePool(donatePoolModel.Ammount);

            return RedirectToAction("Stats");
        }

        [HttpPost]
        public IActionResult Buy(BuyModel buyModel)
        {
            Player player = new Player(buyModel.Address);
            
            Dripple.Buy(player, buyModel.Ammount);

            return RedirectToAction("Stats");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}