﻿using FirstMvcProject.Models;
using FirstMvcProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstMvcProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly NorthwindContext _northwindContext;
        public ProductController(NorthwindContext northwindContext)
        {
            _northwindContext = northwindContext;
        }
        private int _pageSize = 10;
        public IActionResult Index(int? page = 1)
        {
            var data = _northwindContext.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .OrderBy(x => x.Category.CategoryName)
                .ThenBy(x => x.ProductName)
                .Skip((page.GetValueOrDefault() - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();
            ViewBag.Page = page.GetValueOrDefault(1);
            ViewBag.Limit = (int)(Math.Ceiling(_northwindContext.Products.Count() / (double)_pageSize));
            //ViewBag.Categories = _northwindContext.Categories.OrderBy(x => x.CategoryName).ToList();
            //ViewBag.Suppliers = _northwindContext.Suppliers.OrderBy(x => x.CompanyName).ToList();
            return View(data);
        }

        public IActionResult Detail(int? id)
        {
            var product = _northwindContext.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .FirstOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return RedirectToAction("index");
            }

            var model = new ProductViewModel()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName,
                SupplierId = product.SupplierId,
                CompanyName = product.Supplier?.CompanyName,
                UnitPrice = product.UnitPrice
            };
            return View(model);
        }
        public IActionResult Update(int? id)
        {
            var product = _northwindContext.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .FirstOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return RedirectToAction("index");
            }

            var model = new ProductViewModel()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName,
                SupplierId = product.SupplierId,
                CompanyName = product.Supplier?.CompanyName,
                UnitPrice = product.UnitPrice

            };
            var categories = _northwindContext.Categories.OrderBy(x => x.CategoryName);
            var suppliers = _northwindContext.Suppliers.OrderBy(x => x.CompanyName);

            var categoryList = new List<SelectListItem>()
            {
            new SelectListItem("Kategori Yok", null)
            };

            foreach (var category in categories)
            {
                categoryList.Add(new SelectListItem(category.CategoryName, category.CategoryId.ToString()));
            }

            var supplierList = new List<SelectListItem>() { new SelectListItem("Tedarikçi Yok", null) };
            foreach (var supplier in suppliers)
            {
                supplierList.Add(new SelectListItem(supplier.CompanyName, supplier.SupplierId.ToString()));
            }
            ViewBag.CategoryList = categoryList;
            ViewBag.SupplierList = supplierList;
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var updateProduct = _northwindContext.Products.FirstOrDefault(x => x.ProductId == model.ProductId);
            if (updateProduct == null)
            {
                return RedirectToAction("Index");
            }

            updateProduct.ProductName = model.ProductName;
            updateProduct.UnitPrice = model.UnitPrice;
            updateProduct.CategoryId = model.CategoryId;
            updateProduct.SupplierId = model.SupplierId;
            try
            {
                _northwindContext.Products.Update(updateProduct);
                _northwindContext.SaveChanges();
                TempData["Message"] = $"{updateProduct.ProductName} isimli ürün başarıyla güncellenmiştir.";
                return RedirectToAction("Detail", new { id = model.ProductId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductViewModel model)
        {
            return View(model);
        }
    }
}
