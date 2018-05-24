﻿using System;
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
        public JsonResult Get()
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            List<RPiConnection> rpiConnections = this.rpiConService.GetRPiConnections();
            rpiConnections.ForEach(rpiCon =>
            {
                rpiCon.password = "***";
            });
            return Json(rpiConnections);
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult Get(int id)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            RPiConnection rpiCon = this.rpiConService.GetRPiConnection(id);
            rpiCon.password = "***";
            return Json(rpiCon);
        }

        /**
         * New connections offered from raspberry pis. These does not have a login, which means they won't have an authentication token, therefore no filter to save the token.
         */
        [HttpPost("offer")]
        public JsonResult PostOffer([FromBody]OfferRPiConnectionModel oRPiConModel)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(this.rpiConService.OfferRPiConnection(oRPiConModel.ip, oRPiConModel.port, oRPiConModel.password));
        }

        [HttpPost("connect/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult GetConnect(int id)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            try
            {
                RPiConnection rpiCon = this.rpiConService.SetRPiConnectionStatus(id, "connected");
                rpiCon.password = "***";
                return Json(rpiCon);
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 403;
                return Json("Database exception!");
            }
        }

        [HttpPost("disconnect/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult GetDisconnect(int id)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            try
            {
                RPiConnection rpiCon = this.rpiConService.SetRPiConnectionStatus(id, "disconnected");
                rpiCon.password = "***";
                return Json(rpiCon);
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 403;
                return Json("Database exception!");
            }
        }

        [HttpGet("status/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult GetRPiStatus(int id)
        {
            if (!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            return Json(new string[] { this.rpiConService.GetRPiConnection(id).status });
        }

        // old token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRoeWdlIiwicHJpbWFyeXNpZCI6IjIiLCJuYmYiOjE1MjcwNjg1MTksImV4cCI6MTUyNzA2OTcxOSwiaWF0IjoxNTI3MDY4NTE5fQ.UhWA_5JY0i-XBN79kZVUMIEJSpWCzhjtGbIVLTOtags
        [HttpPost("test")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult PostAuthTest([FromBody]StatusRPiConnectionModel sRPiConModel)
        {
            if(!this.auth.ValidateToken(this.auth.GetToken(), out string authResponse))
            {
                this.HttpContext.Response.StatusCode = 401;
                return Json(new string[] { authResponse });
            }
            //return Json(sRPiConModel);
            try
            {
                return Json("success!!");
            }
            catch (NpgsqlException e)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json("Database failed!");
            }
            catch(Exception e)
            {
                this.HttpContext.Response.StatusCode = 400;
                return Json("General failure by the server!");
            }
        }

        [HttpPost("execute_flightplan/{id}")]
        [ServiceFilter(typeof(SaveAuthenticationHeader))]
        public JsonResult PostExecute(int id, [FromBody]ExecuteFlightplanModel eFPModel)
        {
            return Json(new bool[] { this.rpiConService.HandFlightplanToRPiConnection(id, eFPModel.flightplanName, eFPModel.priority) });

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