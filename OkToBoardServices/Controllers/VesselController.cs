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
    public class VesselController : ApiController
    {
        private DBContext db = new DBContext();

        // GET api/Vessel
        public IEnumerable<Vessel> GetVessels()
        {
            return db.Vessels.AsEnumerable();
        }

        // GET api/Vessel/5
        public Vessel GetVessel(int id)
        {
            Vessel vessel = db.Vessels.Find(id);
            if (vessel == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return vessel;
        }

        // PUT api/Vessel/5
        public HttpResponseMessage PutVessel(int id, Vessel vessel)
        {
            if (ModelState.IsValid && id == vessel.Id)
            {
                db.Entry(vessel).State = EntityState.Modified;

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

        // POST api/Vessel
        public HttpResponseMessage PostVessel(Vessel vessel)
        {
            if (ModelState.IsValid)
            {
                db.Vessels.Add(vessel);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, vessel);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = vessel.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Vessel/5
        public HttpResponseMessage DeleteVessel(int id)
        {
            Vessel vessel = db.Vessels.Find(id);
            if (vessel == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Vessels.Remove(vessel);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, vessel);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}