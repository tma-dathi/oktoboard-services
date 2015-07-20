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
using System.IO;

namespace OkToBoardServices.Controllers
{
    public class VesselViewModel
    {
        public Guid Id;
        public string Name;
    }

    public class ArrangementViewModel
    {
        public string ETADateTime;
    }

    [RequireHttps]
    public class VesselController : ApiController
    {
        private DBContext db = new DBContext();

        // GET api/Vessel
        public object GetVessels(bool isBulkData = false)
        {
            Logger.log.Info("Valid token, here is GetVessels().");
            object items = null;
            if (isBulkData)
            {
                items = GetWholeVessels();
            }
            else
            {
                items = db.Vessels.Select(
                   v => new VesselViewModel
                   {
                       Id = v.Id,
                       Name = v.Name.Trim()
                   }).OrderBy(c => c.Name).AsEnumerable();
            }
            return items;
        }

        private object GetWholeVessels()
        {
            // query database and store in collection object (enumerable)
            var enumerable = db.Vessels.Select(
               v => new
               {
                   Id = v.Id,
                   Name = v.Name.Trim(),
                   Arrangements = v.Arrangements
               }).OrderBy(c => c.Name).AsEnumerable();

            // query on collection object
            var items = enumerable.Select(vv => new
            {
                Id = vv.Id,
                Name = vv.Name,
                Arrangements = vv.Arrangements.Select(x => new 
                {
                    Id = x.Id,
                    ETADateTime = PopulateDateTime(x.ETADate, x.ETATime) 
                })
            }).AsEnumerable();
            return items;
        }

        // GET api/GetEtaByShip/ecb7f9a0-c5ec-43ed-bf70-059d88e5e663
        [HttpGet]
        public IEnumerable<ArrangementViewModel> GetEtaByShip(Guid id)
        {
            Logger.log.Info("Valid token, here is GetEtaByShip(Guid id).");
            Vessel vessel = db.Vessels.Find(id);
            if (vessel == null)
            {
                Logger.log.Info("Vessel not found.");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            var items = vessel.Arrangements.Select(
                x => new ArrangementViewModel
                {
                    ETADateTime = PopulateDateTime(x.ETADate, x.ETATime)
                }).AsEnumerable();

            return items;
        }

        private string PopulateDateTime(DateTime date, DateTime time)
        {
            var result = DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
            var d = String.Format("{0:d-MMM-yyyy}", date);
            var t = String.Format("{0:HH:mm}", time);
            result = String.Format("{0} {1}", d, t);
            return result;
        }

        // NOT IN USE FOR NOW
        // GET api/Vessel/5
        public Vessel GetVessel(Guid id)
        {
            Vessel vessel = db.Vessels.Find(id);
            if (vessel == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return vessel;
        }

        // NOT IN USE FOR NOW
        // PUT api/Vessel/5
        public HttpResponseMessage PutVessel(Guid id, Vessel vessel)
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

        // NOT IN USE FOR NOW
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

        // NOT IN USE FOR NOW
        // DELETE api/Vessel/5
        public HttpResponseMessage DeleteVessel(Guid id)
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
