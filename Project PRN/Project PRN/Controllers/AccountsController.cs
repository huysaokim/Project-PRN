﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_PRN.Models;

namespace Project_PRN.Controllers {
    public class AccountsController : Controller {
        private ProjectPRNEntities3 db = new ProjectPRNEntities3();

        // GET: Accounts
        public ActionResult SignIn() {
            if (Session["user"] == null) {
                return View();
            } else {
                return RedirectToRoute(new {
                    controller = "Home", action = "Index", id = UrlParameter.Optional
                });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckLogin([Bind(Include = "email,password")] Account account) {
            if (ModelState.IsValid) {
                string checkEmail = account.email;
                string checkPassword = account.password;
                List<Account> list = db.Accounts.Where(a => a.email.Equals(checkEmail)).ToList();
                if (list.Count > 0) {
                    if (BCrypt.Net.BCrypt.Verify(checkPassword, list[0].password)) {
                        HttpSessionStateBase session = HttpContext.Session;
                        session.Add("user", list[0].userID);
                        return RedirectToRoute(new {
                            controller = "Home", action = "Index", id = UrlParameter.Optional
                        });
                    }
                }
            }
            return RedirectToAction("SignIn");
        }

        public ViewResult SignUp() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userID,email,password,userName,role,address,phoneNumber")] Account account) {
            if (ModelState.IsValid) {
                List<Account> list = db.Accounts.Where(a => a.email.Equals(account.email)).ToList();
                if (list.Count == 0) {
                    account.role = 3;
                    string pass = account.password;
                    int cost = 12;
                    string newPassword = BCrypt.Net.BCrypt.HashPassword(pass, cost);
                    account.password = newPassword;
                    Console.WriteLine($"{account.userID} - {account.userName} - {account.password} - {account.phoneNumber} - {account.role} - {account.address}");
                    db.Accounts.Add(account);
                    db.SaveChanges();
                    return RedirectToRoute(new {
                        controller = "Home", action = "Index", id = UrlParameter.Optional
                    });
                }
            }
            return RedirectToAction("SignUp");
        }

        public ActionResult SignOut() {
            try {
                if (Session["user"] != null) {
                    HttpSessionStateBase session = HttpContext.Session;
                    session.Remove("user");
                }
            } catch (Exception e) {
                //chuyen toi trang bao loi
            }
            return RedirectToRoute(new {
                controller = "Home", action = "Index", id = UrlParameter.Optional
            });
        }

        public ViewResult Manager() {
            return View();
        }

        public JsonResult ManagerJson(int? index) {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit() {
            return View();
        }

        public ActionResult EditJson() {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userID,email,password,userName,role,address,phoneNumber")] Account account) {
            if (ModelState.IsValid) {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
