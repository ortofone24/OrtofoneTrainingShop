using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrtofoneTrainingShop.Models.Data;
using OrtofoneTrainingShop.Models.ViewModels.Shop;

namespace OrtofoneTrainingShop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // deklarujemy CategoryVM list
            List<CategoryVM> categoryVMList;
           
            // inicjalizacja listy
            using (Database db = new Database())
            {
                categoryVMList = db.Categories.ToArray()
                                              .OrderBy(x => x.Sorting)
                                              .Select(x => new CategoryVM(x))
                                              .ToList();
            }

            // zwracamy partial z listą


            return PartialView(categoryVMList);
        }
    }
}