using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using OkToBoardServices.Models;

namespace OkToBoardServices.Controllers
{
    public class ETAController : ApiController
    {
        private DBContext db = new DBContext();

        // GET api/ETA
        public IEnumerable<ETA> GetETAs()
        {
            var etas = db.ETAs.Include(e => e.Vessel);
            return etas.AsEnumerable();
        }

        // GET api/ETA/5
        public ETA GetETA(int id)
        {
            ETA eta = db.ETAs.Find(id);
            if (eta == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return eta;
        }

        // PUT api/ETA/5
        public HttpResponseMessage PutETA(int id, ETA eta)
        {
            if (ModelState.IsValid && id == eta.Id)
            {
                db.Entry(eta).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/ETA
        public HttpResponseMessage PostETA(ETA eta)
        {
            if (ModelState.IsValid)
            {
                db.ETAs.Add(eta);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, eta);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = eta.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/ETA/5
        public HttpResponseMessage DeleteETA(int id)
        {
            ETA eta = db.ETAs.Find(id);
            if (eta == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.ETAs.Remove(eta);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, eta);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}