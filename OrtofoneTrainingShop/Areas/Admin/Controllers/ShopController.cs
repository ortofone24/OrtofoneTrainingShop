using OrtofoneTrainingShop.Models.Data;
using OrtofoneTrainingShop.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;


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

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public ActionResult ReorderCategories(int[] id)
        {
            //db contekst
            using (Database db = new Database())
            {
                // inicjalizacja licznika count
                int count = 1;

                // deklaracja DTO
                CategoryDTO dto;

                // sortowanie kategorii
                foreach (var catId in id)
                {
                    // przypisanie do dto idiki z bazy
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;


                    // zapis na bazie
                    db.SaveChanges();
                    count++;
                }
            }

            return View();
        }

        //GET: Admin/Shop/DeleteCategory
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            //kontekst
            using (Database db = new Database())
            {
                //pobieramy kategorię do usunięcia o podanym id
                CategoryDTO dto = db.Categories.Find(id);

                //usuwanie kategorie
                db.Categories.Remove(dto);

                // zapis na bazie
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }

        // POST Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            //kontekst

            using (Database db = new Database())
            {
                // sprawdzamy czy kategoria jest unikalna
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "tytulzajety";
                }

                //pobieramy kategorie 
                CategoryDTO dto = db.Categories.Find(id);

                // edycja kategorii
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //zapis na bazie
                db.SaveChanges();
            }
            return "Ok";
        }


        //GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Inicjalizacja modelu
            ProductVM model = new ProductVM();

            // pobieramy liste kategorii z dbkontekstu
            using (Database db = new Database())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }


            return View(model);
        }

        //Post: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //sprawdzamy model state
            if (!ModelState.IsValid)
            {
                //kontekst
                using (Database db = new Database())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }


            // sprawdzenie czy nazwa produktu jest unikalna
            //kontekst
            using (Database db = new Database())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Ta nazwa produktu jest zajęta");
                    return View(model);
                }
            }

            //deklaracja prductId
            int id;

            // dodawanie produktu i zapis na bazie
            using (Database db = new Database())
            {
                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDto.Name;

                db.Products.Add(product);
                db.SaveChanges();

                // pobranie id dodanego produktu
                id = product.Id;
            }

            //ustawiamy komunikat 
            TempData["SM"] = "Dodałeś produkt";

            #region Upload Image

            //utworzenie potrzebnej struktury katalogów
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }

            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }

            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }

            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            if (file != null && file.ContentLength > 0)
            {
                // sprawdzenie rozszerzenia pliku czy mamy do czynienia z obrazkiem
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Database db = new Database())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Obraz nie został przesłany - nieprawidłowe rozszerzenie obrazu.");
                        return View(model);
                    }
                }

                // inicjalizacja nazwy obrazka
                string imgName = file.FileName;

                // zapis nazwy obrazka do bazy
                using (Database db = new Database())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imgName;
                    db.SaveChanges();
                }

                var path = string.Format("{0}\\{1}", pathString2, imgName);
                var path2 = string.Format("{0}\\{1}", pathString3, imgName);

                //zapisujemy oryginalny obrazek
                file.SaveAs(path);

                //zapisujemy miniaturkę
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);


            }



            #endregion

            return RedirectToAction("AddProduct");
        }


        //GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            // deklaracja listy productVM
            List<ProductVM> listOfProductVM;

            //ustawiamy numer strony
            var pageNumber = page ?? 1;

            //kontekst
            using (Database db = new Database())
            {
                // inicjalizacja listy produktów

                listOfProductVM = db.Products.ToArray()
                                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                    .Select(x => new ProductVM(x))
                                    .ToList();

                // lista kategorii do dropdownlist
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // ustawiamy wybraną kategorie
                ViewBag.SelectedCat = catId.ToString();
            }

            //ustawienie stronicowania
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            //zwracamy widok z listą produktów
            return View(listOfProductVM);
        }
    }
}