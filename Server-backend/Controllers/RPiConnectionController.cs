using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server_backend.RPiConnectionNS;
using Server_backend.utility;
using Npgsql;
//using Microsoft.AspNetCore.Authorization;

namespace Server_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RPiConnectionController : Controller
    {
        private readonly IAuthenticationService auth;
        private readonly IRPiConnectionService rpiConService;

        public RPiConnectionController(IAuthenticationService _auth, IRPiConnectionService _rpiConService)
        {
            this.auth = _auth;
            this.rpiConService = _rpiConService;
        }

        [HttpGet]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public List<RPiConnection> Get()
        {
            List<RPiConnection> rpiConnections = this.rpiConService.GetRPiConnections();
            rpiConnections.ForEach(rpiCon =>
            {
                rpiCon.password = "***";
            });
            return rpiConnections;
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public RPiConnection Get(int id)
        {
            RPiConnection rpiCon = this.rpiConService.GetRPiConnection(id);
            rpiCon.password = "***";
            return rpiCon;
        }

        [HttpPost("offer")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public RPiConnection PostOffer([FromBody]OfferRPiConnectionModel oRPiConModel)
        {
            SendHttpService test = new SendHttpService();
            test.SendPost("", ref oRPiConModel);
            return this.rpiConService.OfferRPiConnection(oRPiConModel.ip, oRPiConModel.port, oRPiConModel.password);
        }

        [HttpGet("connect/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public RPiConnection GetConnect(int id)
        {
            try
            {
                RPiConnection rpiCon = this.rpiConService.SetRPiConnectionStatus(id, "connected");
                rpiCon.password = "***";
                return rpiCon;
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 403;
                return null;
            }
        }

        [HttpGet("disconnect/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public RPiConnection GetDisconnect(int id)
        {
            try
            {
                RPiConnection rpiCon = this.rpiConService.SetRPiConnectionStatus(id, "disconnected");
                rpiCon.password = "***";
                return rpiCon;
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 403;
                return null;
            }
        }

        [HttpGet("status/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public string[] GetRPiStatus(int id)
        {
            return new string[] { this.rpiConService.GetRPiConnection(id).status };
        }

        [HttpPost("test/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        [System.Web.Http.Authorize(Roles = "user")]
        public string PostTest(int id, [FromBody]StatusRPiConnectionModel sRPiConModel)
        {
            return sRPiConModel.status;
            /*
            try
            {
                return Json(this.rpiConService.SetRPiConnectionStatus(id, sRPiConModel.status));
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json(new AllowedStatusRPiConnectionModel());
            }
            */
        }

        [HttpPost("testauth/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        //[System.Web.Http.Authorize(Roles = "admin")]
        //[JwtAuthentication]
        public JsonResult PostAuthTest(int id, [FromBody]StatusRPiConnectionModel sRPiConModel)
        {
            Console.WriteLine(this.auth.GetToken());
            if(!this.auth.ValidateToken(this.auth.GetToken()))
            {
                Console.WriteLine("LAWLLL");
                return Json(new string[] { "FUCKING AUTH NOT WORK" });
            }
            return Json(sRPiConModel);
            /*
            try
            {
                return Json(this.rpiConService.SetRPiConnectionStatus(id, sRPiConModel.status));
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json(new AllowedStatusRPiConnectionModel());
            }
            */
        }

        [HttpPost("execute_flightplan/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public bool[] PostExecute(int id, [FromBody]ExecuteFlightplanModel eFPModel)
        {
            return new bool[] { this.rpiConService.HandFlightplanToRPiConnection(id, eFPModel.flightplanName, eFPModel.priority) };

        }
    }

    public class OfferRPiConnectionModel
    {
        public string ip { get; set; }
        public int port { get; set; }
        public string password { get; set; }
    }

    public class StatusRPiConnectionModel
    {
        public string status { get; set; }
    }

    public class AllowedStatusRPiConnectionModel
    {
        public string[] allowedStatuses = new string[] { "disconnected", "connected" };
    }

    public class ExecuteFlightplanModel
    {
        public string flightplanName { get; set; }
        public int priority { get; set; }
    }
}