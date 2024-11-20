using Demo.DataAccess.Repository.IRepository;
using Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Demo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            //return View(productList);

            return View();
        }

        public IActionResult Category(int categoryId)
        {
            TempData["category"] = categoryId.ToString();
            return View();
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpGet]
        public IActionResult GetCategory(int category)
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(u => u.CategoryId == category, includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUser.Id == userId && u.ProductId == shoppingCart.ProductId && u.Ice == shoppingCart.Ice && u.Sweetness == shoppingCart.Sweetness);
            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            //TempData["success"] = "加入購物車成功！";
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddDetail(IFormCollection formcollection)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCart = new ShoppingCart();
            shoppingCart.ApplicationUserId = userId;
            shoppingCart.ProductId= Convert.ToInt32(formcollection["ProductId"]);
            shoppingCart.Ice = formcollection["ice"].ToString();
            shoppingCart.Sweetness = formcollection["sweet"].ToString();
            shoppingCart.Count = Convert.ToInt32(formcollection["count"]);

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUser.Id == userId && u.ProductId == shoppingCart.ProductId && u.Ice == shoppingCart.Ice && u.Sweetness == shoppingCart.Sweetness);
            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            //TempData["success"] = "加入購物車成功！";
            _unitOfWork.Save();


            return Json(new { success = true, message = "加入購物車成功" });

            //return RedirectToAction(nameof(Index));
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
