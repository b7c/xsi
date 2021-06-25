#r "lib\Xabbo.Common.dll"
#r "lib\Xabbo.GEarth.dll"
#r "lib\Xabbo.Core.dll"

using System.Threading;
using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.GEarth;
using Xabbo.Core;
using Xabbo.Core.Game;
using Xabbo.Core.Tasks;

using static System.Console;

const string VERSION = "0.1";

class Extension : GEarthExtension {

    private readonly TaskCompletionSource _connectSignal = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    public Task ConnectSignal => _connectSignal.Task;

    public RoomManager RoomManager { get; }

    public Extension()
        : base(new GEarthOptions {
            Title = "xsi",
            Description = "xabbo script interactive",
            Version = VERSION,
            Author = "b7",
            ShowEventButton = true,
            ShowLeaveButton = true
        }, 9092)
    {
        RoomManager = new RoomManager(this);
    }

    protected override void OnConnected(GameConnectedEventArgs e)
    {
        Console.WriteLine($"Game connected. Client = {e.ClientType}, Ver = {e.ClientVersion}");

        try
        {
            Dispatcher.Bind(RoomManager, ClientType);
        }
        catch (Exception ex)
        {
            _connectSignal.TrySetException(ex);
            return;
        }

        _connectSignal.TrySetResult();        
    }
}

Extension __xtn = new();