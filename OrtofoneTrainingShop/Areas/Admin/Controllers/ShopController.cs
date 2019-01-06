using OrtofoneTrainingShop.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OrtofoneTrainingShop.Models.Data;


namespace OrtofoneTrainingShop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //deklaracja listy kategorii do wyświetlenia
            List<CategoryVM> categoryVMList;

            using (Database db = new Database())
            {
                categoryVMList = db.Categories
                                   .ToArray()
                                   .OrderBy(x => x.Sorting)
                                   .Select(x => new CategoryVM(x))
                                   .ToList();
            }

            return View(categoryVMList);
        }


        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Deklaracja id
            string id;

            using (Database db = new Database())
            {
                // Sprawdzenie czy nazwa kategoria jest unikalna
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "tytulzajety";
                }

                // Inicjalizacja dto
                CategoryDTO dto = new CategoryDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 1000;

                // Zapis do bazy
                db.Categories.Add(dto);
                db.SaveChanges();

                //Pobieramy id
                id = dto.Id.ToString();
            }
            return id;
        }
    }
}