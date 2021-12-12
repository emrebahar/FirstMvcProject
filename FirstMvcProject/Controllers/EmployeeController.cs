﻿using FirstMvcProject.Models;
using FirstMvcProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstMvcProject.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly NorthwindContext _northwindContext;
        public EmployeeController(NorthwindContext northwindContext)
        {
            _northwindContext = northwindContext;
        }
        public IActionResult Index()
        {
            var data = _northwindContext.Employees.OrderBy(x => x.EmployeeId).ToList();
            return View(data);
        }
        public IActionResult Detail(int id)
        {
            var employee = _northwindContext.Employees
                .Include(x => x.Orders)
                .FirstOrDefault(x => x.EmployeeId == id);
            return View(employee);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Employee employee = new Employee
            {
                FirstName = model.FirsName,
                LastName = model.LastName,
                Title = model.Title,
                City = model.City,
                Address = model.Adress,
                BirthDate = model.BirthDate
            };
            _northwindContext.Employees.Add(employee);
            try
            {
                _northwindContext.SaveChanges();
                return RedirectToAction("Detail", new { id = employee.EmployeeId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, $"{model.FirsName + model.LastName} eklenirken bir hata oluştu Tekrar deneyiniz.");
                return View(model);
            }
        }
        public IActionResult Delete(int id)
        {
            var deleteEmploye = _northwindContext.Employees.FirstOrDefault(x => x.EmployeeId == id);
            try
            {
                _northwindContext.Employees.Remove(deleteEmploye);
                _northwindContext.SaveChanges();
            }
            catch (Exception)
            {
                return RedirectToAction("Detail", new { id = id });
            }
            TempData["Silinen Çalışan"] = deleteEmploye.FirstName + " " + deleteEmploye.LastName;
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {
            var updateEmploye = _northwindContext.Employees.FirstOrDefault(x => x.EmployeeId == id);
            if (updateEmploye == null)
            {
                return RedirectToAction("Index");
            }
            var model = new EmployeeViewModel()
            {
                EmployeeId = updateEmploye.EmployeeId,
                FirsName = updateEmploye.FirstName,
                LastName = updateEmploye.LastName,
                Title = updateEmploye.Title,
                City = updateEmploye.City,
                Adress = updateEmploye.Address,
                BirthDate = updateEmploye.BirthDate
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(EmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var updateEmploye = _northwindContext.Employees.FirstOrDefault(x => x.EmployeeId == model.EmployeeId);
            try
            {
                updateEmploye.FirstName = model.FirsName;
                updateEmploye.LastName = model.LastName;
                updateEmploye.Title = model.Title;
                updateEmploye.City = model.City;
                updateEmploye.Address = model.Adress;
                updateEmploye.BirthDate = model.BirthDate;
                _northwindContext.Employees.Update(updateEmploye);
                _northwindContext.SaveChanges();
                return RedirectToAction("Detail", new { id = updateEmploye.EmployeeId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, $"{model.FirsName + model.LastName } Güncellenirken bir hata oluştu. Tekrar deneyiniz");
                return View(model);
            }
        }
    }

}
