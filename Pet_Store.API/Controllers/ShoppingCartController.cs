using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetStore.DataAccess.Repository.UnityOfWork;
using PetStore.Domain.Models.ViewModels;
using Project_PetStore.API.DataAccess;
using Project_PetStore.API.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace Pet_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        IApplicationDbContext Context;
        private readonly IUnityOfWork _unityOfWork;
        public ShoppingCartController(IUnityOfWork unityOfWork, IApplicationDbContext context)
        {
            _unityOfWork = unityOfWork;
            Context = context;
        }

        [HttpPost]
        [Route("add-products")]
        public IActionResult addToCart([FromBody] NewShoppingCart model)
        {
            if (ModelState.IsValid)
            {
                var getUser = _unityOfWork.UsersRepository.GetFirstOrDefault(x => x.UserId == model.UserId);
                var getProduct = _unityOfWork.ProductsRepository.GetFirstOrDefault(x => x.Name == model.Product);
                ShoppingCart shoppingCart = new ShoppingCart();

                if (getUser != null && getProduct != null)
                {

                    var CartExist = _unityOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.User.UserId == model.UserId);
                    var products = _unityOfWork.ShoppingCartRepository.getProductsName(model.UserId);

                    if (CartExist != null && products.Contains(model.Product))
                    {
                        //actualizar producto existente con nuevos datos
                        CartExist.Count += model.Count;
                        CartExist.Subtotal = CartExist.Count * getProduct.Price;

                        _unityOfWork.ShoppingCartRepository.Update(CartExist);
                        _unityOfWork.Save();
                        return Ok(CartExist);
                    }
                    //crear carrito al usuario
                    shoppingCart = new ShoppingCart
                    {
                        Count = model.Count,
                        Product = getProduct,
                        User = getUser,
                        Subtotal = (model.Count * getProduct.Price),
                    };

                    _unityOfWork.ShoppingCartRepository.Add(shoppingCart);
                    _unityOfWork.Save();
                    return Ok(shoppingCart);
                }
                return BadRequest("Producto o usuario no existe");
            }
            return BadRequest("Error al agregar producto al carrito");
        }


        [HttpDelete]
        [Route("remove-product")]
        public IActionResult DeleteItem(int Userid, int count, int ProductoID)
        {
            var CartExist = _unityOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.User.UserId == Userid);
            var products = _unityOfWork.ShoppingCartRepository.getProducts(Userid);
            var product = _unityOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Product.ProductId == ProductoID);
            var getProduct = _unityOfWork.ProductsRepository.GetFirstOrDefault(x => x.Name == product.Product.Name);

            if (product != null && CartExist != null)
            {

                if (CartExist.Product.ProductId == ProductoID)
                {
                    if (CartExist.Count >= 1)
                    {
                        //actualizar producto existente con nuevos datos
                        CartExist.Count = product.Count - count;
                        CartExist.Subtotal = CartExist.Count * getProduct.Price;

                        if (CartExist.Count <= 0)
                        {
                            _unityOfWork.ShoppingCartRepository.Remove(product);
                            _unityOfWork.Save();
                            return Ok($"El producto ha sido eliminado del carrito ");
                        }

                        _unityOfWork.ShoppingCartRepository.Update(CartExist);
                        _unityOfWork.Save();
                        return Ok(CartExist);
                    }


                    return Ok($"Es probable que el articulo no exista en el carrito ");
                }
            }
            else if (CartExist.Product == null)
            {

                _unityOfWork.ShoppingCartRepository.Remove(CartExist);
                _unityOfWork.Save();
                return Ok($"Se elimino el carrito ");
            }
            else if (product == null)
            {

                return BadRequest("Error al remover el producto del carrito puede que el usuario no tenga carrito o el articulo no exista");
            }

            return BadRequest("por favor intentelo de nuevo");
        }





        [HttpGet]
        [Route("GetAll-Products")]
        public IActionResult GetProducts(int userId)
        {
            List<ShoppingCart> allProductosShoppinCart = _unityOfWork.ShoppingCartRepository.GetShoppingcartByUser(userId);
            _unityOfWork.Save();

            if (allProductosShoppinCart != null)
            {
                return Ok(allProductosShoppinCart);
            }
            return BadRequest("No hay productos en el carrito");
        }

    }
}
