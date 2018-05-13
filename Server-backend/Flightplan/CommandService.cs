using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server_backend.Database;

namespace Server_backend.Flightplan
{
    public class Command
    {
        public int RowId { get; set; }
        public int FlightPlanId { get; set; }
        public int CmdId { get; set; }
        public string CmdString { get; set; }
        public string Message { get; set; }
        public int Order { get; set; }
        public List<string> Params { get; }
        private int ParamsLength = 7;

        public Command()
        {
            this.Params = new List<string>(this.ParamsLength);
        }

        public string GetJson()
        {
            throw new NotImplementedException();
        }
    }

    public interface ICommandCommon
    {
        List<Command> GetCommands(int flightplanId);
        Command GetCommand(int CommandId);
        void SaveCommand(Command command);
    }

    public interface ICommandService : ICommandCommon
    {
        List<Command> GetPossibleCommands();
    }

    public class CommandService : ICommandService
    {
        private readonly ICommandDatabaseService commandDBService;
        
        public CommandService(ICommandDatabaseService _commandDBService)
        {
            this.commandDBService = _commandDBService;
        }

        public Command GetCommand(int CommandId)
        {
            return this.commandDBService.GetCommand(CommandId);
        }

        public List<Command> GetCommands(int flightplanId)
        {
            throw new NotImplementedException();
        }

        public List<Command> GetPossibleCommands()
        {
            throw new NotImplementedException();
        }

        public void SaveCommand(Command command)
        {
            throw new NotImplementedException();
        }
    }
}
