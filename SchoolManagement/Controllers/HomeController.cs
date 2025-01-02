using SchoolManagement.Data;
using SchoolManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace SchoolManagement.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index(string term = "", int currentPage = 1)
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }
    }
}