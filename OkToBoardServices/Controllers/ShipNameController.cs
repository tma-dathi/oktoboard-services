using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace OkToBoardServices.Controllers
{
    public class ShipNameShortcut
    {
        public Guid ShipNameId;
        public string ShipName;
    }
    public class ArrangementShortcut
    {
        public string ETADate;
    }

    [RequireHttps]
    public class ShipNameController : ApiController
    {
        private GWdbEntities db = new GWdbEntities();

        // GET api/ShipName
        public IEnumerable<ShipNameShortcut> GettblCCShipNames()
        {
            var items = db.tblCCShipNames.Select(
                sn => new ShipNameShortcut
                {
                    ShipNameId = sn.ccsnShipNameID,
                    ShipName = sn.ccsnName.Trim()
                }).AsEnumerable();
            return items;
            //return db.tblCCShipNames.AsEnumerable();
        }

        // GET api/GetEtaByShip/ecb7f9a0-c5ec-43ed-bf70-059d88e5e663
        public IEnumerable<ArrangementShortcut> GetEtaByShip(Guid id)
        {
            tblCCShipName tblccshipname = db.tblCCShipNames.Find(id);
            if (tblccshipname == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            var items = tblccshipname.tblCCArrangements.Select(
                x => new ArrangementShortcut
                {
                    ETADate = String.Format("{0:d-MMM-yyyy}", x.ccarETADate)
                }).AsEnumerable();
            return items;
        }

        public string PostImage()
        {
            try
            {
                Stream input = HttpContext.Current.Request.InputStream;
                string path = @"C:\inetpub\wwwroot\OkToBoardServices\chupoke.jpeg";
                using (FileStream output = File.OpenWrite(path))
                {
                    input.CopyTo(output);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Debug(ex);
                throw;
            }
            return "ok";
        }

        // NOT IN USE FOR NOW
        // GET api/ShipName/5
        public tblCCShipName GettblCCShipName(Guid id)
        {
            tblCCShipName tblccshipname = db.tblCCShipNames.Find(id);
            if (tblccshipname == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return tblccshipname;
        }

        // NOT IN USE FOR NOW
        // PUT api/ShipName/5
        public HttpResponseMessage PuttblCCShipName(Guid id, tblCCShipName tblccshipname)
        {
            if (ModelState.IsValid && id == tblccshipname.ccsnShipNameID)
            {
                db.Entry(tblccshipname).State = EntityState.Modified;

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
        // POST api/ShipName
        public HttpResponseMessage PosttblCCShipName(tblCCShipName tblccshipname)
        {
            if (ModelState.IsValid)
            {
                db.tblCCShipNames.Add(tblccshipname);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, tblccshipname);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = tblccshipname.ccsnShipNameID }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // NOT IN USE FOR NOW
        // DELETE api/ShipName/5
        public HttpResponseMessage DeletetblCCShipName(Guid id)
        {
            tblCCShipName tblccshipname = db.tblCCShipNames.Find(id);
            if (tblccshipname == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.tblCCShipNames.Remove(tblccshipname);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, tblccshipname);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}