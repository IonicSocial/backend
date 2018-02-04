using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialApp.Data;

namespace SocialApp.Controllers
{
    public class TextToController : Controller
    {
        clsTextTo _textTo = new clsTextTo();
        public ActionResult Index()
        {
            var textToList=_textTo.List();
            return View(textToList);
        }

        // GET: TextTo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TextTo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TextTo/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TextTo/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TextTo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TextTo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TextTo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
