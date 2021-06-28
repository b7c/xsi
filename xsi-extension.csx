#load "xsi-loader.csx"

using System.Threading;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.GEarth;
using Xabbo.Core;
using Xabbo.Core.Game;
using Xabbo.Core.GameData;
using Xabbo.Core.Tasks;

const string VERSION = "0.2";

class Extension : GEarthExtension {

    private readonly TaskCompletionSource _connectSignal = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    public Task ConnectSignal => _connectSignal.Task;

    public ProfileManager ProfileManager { get; }
    public FriendManager FriendManager { get; }
    public RoomManager RoomManager { get; }
    public TradeManager TradeManager { get; }

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
        ProfileManager = new ProfileManager(this);
        FriendManager = new FriendManager(this);
        RoomManager = new RoomManager(this);
        TradeManager = new TradeManager(this, ProfileManager, RoomManager);
    }

    protected override void OnConnected(GameConnectedEventArgs e)
    {
        WriteLine($"Game connected. Client = {e.ClientType}, Ver = {e.ClientVersion}");

        try
        {
            Dispatcher.Bind(ProfileManager, ClientType);
            Dispatcher.Bind(FriendManager, ClientType);
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