using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server_backend.Database;

namespace Server_backend.FlightplanNS
{
    public class Command
    {
        public int RowId { get; set; }
        public int FlightplanId { get; set; }
        public int CmdId { get; set; }
        private string cmdString;
        public string CmdString
        {
            get { return this.cmdString; }
            set
            {
                this.cmdString = value;
                CmdId = this.GetCmdIdOfString();
            }
        }
        public string Message { get; set; }
        public int Order { get; set; }
        public List<int> Params { get; }
        private int ParamsLength = 7;

        public Command()
        {
            this.Params = new List<int>(this.ParamsLength);
        }

        public string GetJson()
        {
            throw new NotImplementedException();
        }


        private int GetCmdIdOfString()
        {
            switch(this.cmdString)
            {
                case "arm":
                    return 400;
                default:
                    return -1;
            }
        }
    }

    public interface ICommandCommon
    {
        Dictionary<int, Command> GetCommands(int flightplanId);
        Command GetCommand(int CommandId);
        Command SaveCommand(Command command);
    }

    public interface ICommandService : ICommandCommon
    {
        string[] GetPossibleCommands();
    }

    public class CommandService : ICommandService
    {
        private readonly ICommandDatabaseService commandDBService;

        private readonly string[] PossibleCommands = new string[] { "arm", "disarm", "throttle", "land", "turn cw", "turn ccw" };

        public CommandService(ICommandDatabaseService _commandDBService)
        {
            this.commandDBService = _commandDBService;
        }

        public Command GetCommand(int CommandId)
        {
            return this.commandDBService.GetCommand(CommandId);
        }

        public Dictionary<int, Command> GetCommands(int flightplanId)
        {
            return this.commandDBService.GetCommands(flightplanId);
        }

        public string[] GetPossibleCommands()
        {
            return this.PossibleCommands;
        }

        public Command SaveCommand(Command command)
        {
            return this.commandDBService.SaveCommand(command);
        }
    }
}
