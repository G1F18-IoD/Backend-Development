﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server_backend.RPiConnectionNS;
using Server_backend.utility;
using Npgsql;

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
            return this.rpiConService.GetRPiConnections();
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public RPiConnection Get(int id)
        {
            return this.rpiConService.GetRPiConnection(id);
        }

        [HttpPost("offer")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public RPiConnection PostOffer([FromBody]OfferRPiConnectionModel oRPiConModel)
        {

            SendHttpService test = new SendHttpService();
            test.SendPost("", ref oRPiConModel);
            return this.rpiConService.OfferRPiConnection(oRPiConModel.ip, oRPiConModel.port, oRPiConModel.password);
        }

        [HttpPost("status/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public RPiConnection PostStatus(int id, [FromBody]StatusRPiConnectionModel sRPiConModel)
        {
            return this.rpiConService.SetRPiConnectionStatus(id, sRPiConModel.status);

        }

        [HttpGet("status/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public string GetRPiStatus(int id)
        {
            return "Not implemented yet!";

        }

        [HttpPost("test/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult PostTest(int id, [FromBody]StatusRPiConnectionModel sRPiConModel)
        {
            try
            {
                return Json(this.rpiConService.SetRPiConnectionStatus(id, sRPiConModel.status));
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json(new AllowedStatusRPiConnectionModel());
            }

        }

        [HttpPost("execute_flightplan/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public bool[] PostExecute(int id, [FromBody]ExecuteFlightplanModel eFPModel)
        {
            return new bool[] { this.rpiConService.HandFlightplanToRPiConnection(id, eFPModel.flightplanId, eFPModel.priority) };

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
        public int flightplanId { get; set; }
        public int priority { get; set; }
    }
}