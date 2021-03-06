﻿using BilgiYonetimSistemi.BLL;
using BilgiYonetimSistemi.DAL;
using BilgiYonetimSistemi.DATA.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCOrnek12.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BilgiYonetimSistemi.UI.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Login()
        {
            HttpCookie cookie = Request.Cookies["loginCookie"];
            if (cookie != null)
            {
                ViewBag.Email = cookie["Email"];
                ViewBag.Password = cookie["Password"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection frm)
        {
            Context db = new Context();
            string email = frm["Email"];
            string sifre = frm["Password"];
            bool beniHatirla = frm["RememberMe"] == "false" ? false : true;
           

            var userStore = new UserStore<Kullanici>(db);
            var userManager = new UserManager<Kullanici>(userStore);

            var kullanici = userManager.FindByEmail(email);
            if (kullanici == null || !userManager.CheckPassword(kullanici, sifre))
            {
                
                return RedirectToAction("Login", "Home");
            }
            else
            {
                if (beniHatirla)
                {
                    HttpCookie cookie = new HttpCookie("loginCookie");
                    cookie["Email"] = email;
                    cookie["Password"] = sifre;
                    cookie.Expires = DateTime.Now.AddYears(1);
                    Response.Cookies.Add(cookie);
                }

                var rol = userManager.GetRoles(kullanici.Id);
                Session["Kullanici"] = kullanici;

                if (rol.First() == "admin" || rol.First() == "developer")
                    return RedirectToAction("Anasayfa", "Yonetici");
                
                else if (rol.First() == "ogretmen")
                    return RedirectToAction("Anasayfa", "Ogretmen");
                
                else if (rol.First() == "ogrenci")
                    return RedirectToAction("Anasayfa", "Ogrenci");
                else
                    return RedirectToAction("Anasayfa", "Yonetici");
            }
        }

        public ActionResult ChangePassword()
        {

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel1 model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (ModelState.IsValid)
            {
                Context db = new Context();
                var userStore = new UserStore<Kullanici>(db);
                var userManager = new UserManager<Kullanici>(userStore);
                var eskiSifre = model.OldPassword;
                var yeniSifre = model.NewPassword;
                var sifreDogrulama = model.ConfirmationPassword;
                var kullanici = Session["Kullanici"] as Kullanici;
                var result1 = userManager.PasswordHasher.VerifyHashedPassword(kullanici.PasswordHash, eskiSifre);
                if (eskiSifre == yeniSifre)
                {
                    ModelState.AddModelError(string.Empty, "Girilen yeni ve eski sifreler ayni olmamali!");
                }
                else if (result1 != PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError(string.Empty, "Girilen eski sifre yanlis!");
                    return View(model);

                }

                else if (yeniSifre != sifreDogrulama)
                {
                    ModelState.AddModelError(string.Empty, "Yeni sifreler uyusmuyor!");
                    return View(model);
                }
                else
                {
                    var result2 = await userManager.ChangePasswordAsync(kullanici.Id, eskiSifre, yeniSifre);
                    if (result2.Succeeded)
                        Response.Write("<script>alert('Kaydetme islemi basariyla gerceklestirildi')</script>");
                }
            }
            return RedirectToAction("Login","Home");
        }

        public ActionResult LogOff()
        {
            Session["Kullanici"] = null;
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}