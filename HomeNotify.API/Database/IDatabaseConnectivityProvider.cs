using System.Data;
using HomeNotify.API.Models;

namespace HomeNotify.API.Database
{
    public interface IDatabaseConnectivityProvider<TCommand> where TCommand : IDbCommand
    {
        ConnectionResult Connect();
        void Disconnect();
        TCommand CreateCommand(string query);
    }
}