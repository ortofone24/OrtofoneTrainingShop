using System;
using OrtofoneTrainingShop.Models.Data;
using OrtofoneTrainingShop.Models.ViewModels.Cart;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace OrtofoneTrainingShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // inicjalizacja koszyka
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // sprawdzamy czy nasz koszyk jest pusty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Twój koszyk jest pusty";
                return View();
            }

            // obliczenie watości podsumowania koszyka i przekazanie do ViewBag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // inicjalizacja CartVM
            CartVM model = new CartVM();

            // inicjalizacja ilość i cena
            int qty = 0;
            decimal price = 0;

            // sprawdzamy czy mamy dane koszyka zapisane w sesji
            if (Session["cart"] != null)
            {
                //pobieranie wartości z sesji
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                // ustawiamy ilosc i cena na 0
                qty = 0;
                price = 0m;
            }

            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Inicjalizacja CartVM List
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Inicjalizacja cartVM
            CartVM model = new CartVM();

            using (Database db = new Database())
            {
                // pobieramy produkt który chcemy dodać do koszyka
                ProductDTO product = db.Products.Find(id);

                // sprawdzamy czy produkt nie znajduje się już w koszyku
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // w zależności od tego czy produkt jest w koszyku go dodajemy lub zwiekszamy ilosc
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;
                }
            }

            // pobieramy całkowite wartość ilości i ceny i dodajemy do modelu
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // zapis w sesji
            Session["cart"] = cart;

            return PartialView(model);
        }


        public JsonResult IncrementProduct(int productId)
        {
            // Inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            //pobieramy cartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            //zwiekszamy ilosc produktu
            model.Quantity++;

            //przygotowanie danych do JSONA
            var result = new {qty = model.Quantity, price = model.Price};

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DecrementProduct(int productId)
        {
            // Inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            //pobieramy cartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            //zmniejszamy ilosc produktu
            if (model.Quantity > 1)
            {
                model.Quantity--;
            }
            else
            {
                model.Quantity = 0;
                cart.Remove(model);
            }
            
            //przygotowanie danych do JSONA
            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void RemoveProduct(int productId)
        {
            // Inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            //pobieramy cartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // usuwamy produkt
            cart.Remove(model);
        }

        public ActionResult PaypalPartial()
        {
            // Inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }


        [HttpPost]
        public void PlaceOrder()
        {
            // pobieranie zawartości koszyka ze zmiennej sesji
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobranie nazwy użytkownika
            string username = User.Identity.Name;

            // deklaracja orderId numer zamówienia
            int orderId = 0;

            // db kontekst
            using (Database db = new Database())
            {
                // incijalizacja orderDTO
                OrderDTO orderDTO = new OrderDTO();

                // pobieramy user id
                var user = db.Users.FirstOrDefault(x => x.UserName == username);
                int userId = user.Id;

                // ustawienie orderDTO i zapis
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDTO);
                db.SaveChanges();

                // pobieramy id zapisanego zamowienia
                orderId = orderDTO.OrderId;

                // inicjalizacja OrderDetailsDTO
                OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();

                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.UserId = userId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);
                    db.SaveChanges();
                }

            }

            // wysyłanie emaila do admina
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("f410ef8f76e72e", "f7615c1fd678ee"),
                EnableSsl = true
            };
            client.Send("admin@example.com", "admin@example.com", "Nowe zamowienie", "Masz nowe zamowienie. Numer zamowienia: " + orderId);

            // reset session
            Session["cart"] = null;
        }

    }
}