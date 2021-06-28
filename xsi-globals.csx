#load "xsi-extension.csx"

using System.Threading;
using Xabbo;
using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.GEarth;
using Xabbo.Core;
using Xabbo.Core.Game;
using Xabbo.Core.Tasks;

using static System.Console;

const int DEFAULT_TIMEOUT = 5000;

CancellationTokenSource __cts;
CancelKeyPress += (s, e) => {
    if (__cts is not null) {
        e.Cancel = true;
        __cts?.Cancel();
    }
};

async Task<TResult> __cancelable<TResult>(Func<CancellationToken, Task<TResult>> task) {
    try {
        using (__cts = new())
            return await task(__cts.Token);
    }
    finally { __cts = null; }
}
async Task __cancelable(Func<CancellationToken, Task> task) {
    try {
        using (__cts = new())
            await task(__cts.Token);
    }
    finally { __cts = null; }
}

Incoming In => __xtn.In;
Outgoing Out => __xtn.Out;
ClientType ClientType => __xtn.ClientType;
void Send(Header header, params object[] values) => __xtn.Send(header, values);
void Send(IReadOnlyPacket packet) => __xtn.Send(packet);
Task<IPacket> ReceiveAsync(int timeout, params Header[] targetHeaders)
    => __cancelable(ct => __xtn.ReceiveAsync(timeout, ct, targetHeaders));
IPacket Receive(int timeout, params Header[] targetHeaders)
    => ReceiveAsync(timeout, targetHeaders).GetAwaiter().GetResult();

// Utility
Task DelayAsync(int ms) => __cancelable(ct => Task.Delay(ms, ct));
void Delay(int ms) => DelayAsync(ms).GetAwaiter().GetResult();

// Client-side
void Bubble(string message,
    int index = -1,
    int bubble = 0,
    ChatType type = ChatType.Whisper) {
  Send(type switch {
      ChatType.Talk => In.Chat,
      ChatType.Shout => In.Shout,
      ChatType.Whisper => In.Whisper,
      _ => throw new Exception("Invalid chat type.")
  }, index, message, 0, bubble, 0, 0);
}

// Profile
IUserData User => __xtn.ProfileManager.UserData;
int? Credits => __xtn.ProfileManager.Credits;

// Friends
IEnumerable<IFriend> Friends => __xtn.FriendManager.Friends;
bool IsFriend(long id) => __xtn.FriendManager.IsFriend(id);
bool IsFriend(string name) => __xtn.FriendManager.IsFriend(name);

// Room
bool IsInRoom => __xtn.RoomManager.IsInRoom;
long RoomId => __xtn.RoomManager.CurrentRoomId;
IRoom Room => __xtn.RoomManager.Room;
IFloorPlan FloorPlan => Room?.FloorPlan;
IHeightmap Heightmap => Room?.Heightmap;

// Entities
IEnumerable<IEntity> Entities => Room?.Entities ?? Enumerable.Empty<IEntity>();
IEnumerable<IRoomUser> Users => Room?.Users ?? Enumerable.Empty<IRoomUser>();
IEnumerable<IBot> Bots => Room?.Bots ?? Enumerable.Empty<IBot>();
IEnumerable<IPet> Pets => Room?.Pets ?? Enumerable.Empty<IPet>();
IEntity GetEntity(int index) => Room?.GetEntity<IEntity>(index);
IEntity GetEntity(string name) => Room?.GetEntity<IEntity>(name);
IEntity GetEntityById(long id) => Room?.GetEntityById<IEntity>(id);
IRoomUser GetUser(int index) => Room?.GetEntity<IRoomUser>(index);
IRoomUser GetUser(string name) => Room?.GetEntity<IRoomUser>(name);
IRoomUser GetUserById(long id) => Room?.GetEntityById<IRoomUser>(id);
IBot GetBot(int index) => Room?.GetEntity<IBot>(index);
IBot GetBot(string name) => Room?.GetEntity<IBot>(name);
IBot GetBotById(long id) => Room?.GetEntityById<IBot>(id);
IPet GetPet(int index) => Room?.GetEntity<IPet>(index);
IPet GetPet(string name) => Room?.GetEntity<IPet>(name);
IPet GetPetById(long id) => Room?.GetEntityById<IPet>(id);

IRoomUser Self => Room?.GetEntityById<IRoomUser>(User?.Id ?? -1);

// Trade
bool IsTrading => __xtn.TradeManager.IsTrading;
bool IsTrader => __xtn.TradeManager.IsTrader;
bool HasAcceptedTrade => __xtn.TradeManager.HasAccepted;
bool HasPartnerAcceptedTrade => __xtn.TradeManager.HasPartnerAccepted;
bool IsTradeWaitingConfirmation => __xtn.TradeManager.IsWaitingConfirmation;
IRoomUser TradePartner => __xtn.TradeManager.Partner;
ITradeOffer OwnOffer => __xtn.TradeManager.OwnOffer;
ITradeOffer PartnerOffer => __xtn.TradeManager.PartnerOffer;

// Furni
IEnumerable<IFurni> Furni => Room?.Furni ?? Enumerable.Empty<IFurni>();
IEnumerable<IFloorItem> FloorItems => Room?.FloorItems ?? Enumerable.Empty<IFloorItem>();
IEnumerable<IWallItem> WallItems => Room?.WallItems ?? Enumerable.Empty<IWallItem>();
IFloorItem GetFloorItem(long id) => Room?.GetFloorItem(id);
IWallItem GetWallItem(long id) => Room?.GetWallItem(id);

// Tasks
Task<IRoomData> GetRoomDataAsync(long roomId, int timeout = DEFAULT_TIMEOUT)
    => __cancelable(ct => new GetRoomDataTask(__xtn, roomId).ExecuteAsync(timeout, ct));
IRoomData GetRoomData(long roomId, int timeout = DEFAULT_TIMEOUT)
    => GetRoomDataAsync(roomId, timeout).GetAwaiter().GetResult();

Task<UserSearchResults> SearchUsersAsync(string name, int timeout = DEFAULT_TIMEOUT)
    => __cancelable(ct => new SearchUserTask(__xtn, name).ExecuteAsync(timeout, ct));
UserSearchResults SearchUsers(string name, int timeout = DEFAULT_TIMEOUT)
    => SearchUsersAsync(name, timeout).GetAwaiter().GetResult();

async Task<UserSearchResult> SearchUserAsync(string name, int timeout = DEFAULT_TIMEOUT)
    => (await SearchUsersAsync(name)).GetResult(name);
UserSearchResult SearchUser(string name, int timeout = DEFAULT_TIMEOUT)
    => SearchUserAsync(name).GetAwaiter().GetResult();

Task<IUserProfile> GetProfileAsync(long userId, int timeout = DEFAULT_TIMEOUT)
    => __cancelable(ct => new GetProfileTask(__xtn, userId).ExecuteAsync(timeout, ct));
IUserProfile GetProfile(long userId, int timeout = DEFAULT_TIMEOUT)
    => GetProfileAsync(userId, timeout).GetAwaiter().GetResult();

Task<IInventory> GetInventoryAsync(int timeout = DEFAULT_TIMEOUT)
    => __cancelable(ct => new GetInventoryTask(__xtn, true).ExecuteAsync(timeout, ct));
IInventory GetInventory(int timeout = DEFAULT_TIMEOUT)
    => GetInventoryAsync(timeout).GetAwaiter().GetResult();

// Interactions
public void Walk(int x, int y) => Send(Out.Move, x, y);

public void Express(Expressions expression) => Send(Out.Expression, (int)expression);
public void Wave() => Express(Expressions.Wave);

public void Dance(int dance) => Send(Out.Dance, dance);
public void Dance(Dances dance) => Dance((int)dance);
public void Dance(bool dance) => Dance(dance ? 1 : 0);
public void Dance() => Dance(1);
public void StopDancing() => Dance(0);

public void Sit(bool sit) => Send(Out.Posture, sit ? 1 : 0);
public void Sit() => Sit(true);
public void Stand() => Sit(false);

public void Chat(ChatType chatType, string message, int bubble = 0)
{
    switch (chatType)
    {
        case ChatType.Talk: Send(Out.Chat, message, bubble, -1); break;
        case ChatType.Shout: Send(Out.Shout, message, bubble); break;
        case ChatType.Whisper: Send(Out.Whisper, message, bubble, -1); break;
        default: throw new Exception($"Unknown chat type: {chatType}.");
    }
}

public void Whisper(IRoomUser recipient, string message, int bubble = 0)
    => Chat(ChatType.Whisper, $"{recipient.Name} {message}", bubble);
public void Whisper(string recipient, string message, int bubble = 0)
    => Chat(ChatType.Whisper, $"{recipient} {message}", bubble);
public void Talk(string message, int bubble = 0) => Chat(ChatType.Talk, message, bubble);
public void Shout(string message, int bubble = 0) => Chat(ChatType.Shout, message, bubble);

// Furni interaction
public void ToggleFloorItem(long id, int state) => Send(Out.UseStuff, (LegacyLong)id, state);
public void UseFloorItem(long id) => ToggleFloorItem(id, 0);
public void ToggleWallItem(long id, int state) => Send(Out.UseWallItem, (LegacyLong)id, state);
public void UseWallItem(long id) => ToggleWallItem(id, 0);
public void UseFurni(IFurni furni) {
    if (furni.Type == ItemType.Floor) UseFloorItem(furni.Id);
    else if (furni.Type == ItemType.Wall) UseWallItem(furni.Id);
}
public void UseGate(long id) => Send(Out.EnterOneWayDoor, (LegacyLong)id);
public void UseGate(IFloorItem item) => UseGate(item.Id);

public void PlaceSticky(long itemId, WallLocation location) => Send(Out.PlacePostIt, (LegacyLong)itemId, location);
public void PlaceSticky(IInventoryItem item, WallLocation location) {
    if (item.Category != FurniCategory.Sticky)
        throw new InvalidOperationException($"Specified item is not a sticky. (category: {item.Category})");
    PlaceSticky(item.ItemId, location);
}

Task<Sticky> GetStickyAsync(long id, int timeout = DEFAULT_TIMEOUT)
    => __cancelable(ct => new GetStickyTask(__xtn, id).ExecuteAsync(timeout, ct));
Task<Sticky> GetStickyAsync(IWallItem item, int timeout = DEFAULT_TIMEOUT) => GetStickyAsync(item, timeout);
Sticky GetSticky(long id, int timeout = DEFAULT_TIMEOUT) => GetStickyAsync(id, timeout).GetAwaiter().GetResult();
Sticky GetSticky(IWallItem item, int timeout = DEFAULT_TIMEOUT) => GetSticky(item.Id, timeout);

void UpdateSticky(long id, string color, string text) => Send(Out.SetStickyData, (LegacyLong)id, color, text);
void UpdateSticky(IWallItem item, string color, string text) => UpdateSticky(item.Id, color, text);
void UpdateSticky(Sticky sticky) => UpdateSticky(sticky.Id, sticky.Color, sticky.Text);

void DeleteSticky(Sticky sticky) => DeleteWallItem(sticky.Id);
void DeleteWallItem(long id) => Send(Out.RemoveItem, (LegacyLong)id);

// Furni placement

void PlaceFloorItem(long itemId, int x, int y, int dir = 0) {
    if (ClientType == ClientType.Flash) {
        Send(Out.PlaceRoomItem, $"{itemId} {x} {y} {dir}");
    } else if (ClientType == ClientType.Unity) {
        Send(Out.PlaceRoomItem, itemId, x, y, dir);
    }
}
void PlaceFloorItem(long itemId, (int X, int Y) location, int dir = 0)
    => PlaceFloorItem(itemId, location.X, location.Y, dir);
void PlaceFloorItem(long itemId, Tile location, int dir = 0)
    => PlaceFloorItem(itemId, location.X, location.Y, dir);

void Place(IInventoryItem item, int x, int y, int dir = 0) {
    if (item.Type != ItemType.Floor)
        throw new InvalidOperationException("Specified item is not a floor item.");
    PlaceFloorItem(item.ItemId, x, y, dir);
}
void Place(IInventoryItem item, (int X, int Y) location, int dir = 0)
    => Place(item, location.X, location.Y, dir);
void Place(IInventoryItem item, Tile location, int dir = 0)
    => Place(item, location.X, location.Y, dir);

void PlaceWallItem(long itemId, WallLocation location) {
    if (ClientType == ClientType.Flash) {
        Send(Out.PlaceRoomItem, $"{itemId} {location}");
    } else if (ClientType == ClientType.Unity) {
        Send(Out.PlaceWallItem, location.WallX, location.WallY, location.X, location.Y, $"{location.Orientation.Value}");
    }
}

void Place(IInventoryItem item, WallLocation location) {
    if (item.Type != ItemType.Wall)
        throw new InvalidOperationException("Specified item is not a wall item.");
    PlaceWallItem(item.Id, location);
}

// Furni movement

void MoveFloorItem(long id, int x, int y, int dir = 0) => Send(Out.MoveRoomItem, (LegacyLong)id, x, y, dir);
void MoveFloorItem(long id, (int X, int Y) location, int dir = 0) => MoveFloorItem(id, location.X, location.Y, dir);
void MoveFloorItem(long id, Tile location, int dir = 0) => MoveFloorItem(id, location.X, location.Y, dir);

void Move(IFloorItem item, int x, int y, int dir = -1) {
    if (dir == -1) dir = item.Direction;
    MoveFloorItem(item.Id, x, y, dir);
}
void Move(IFloorItem item, (int X, int Y) location, int dir = -1) => Move(item, location.X, location.Y, dir);
void Move(IFloorItem item, Tile location, int dir = -1) => Move(item, location.X, location.Y, dir);


// Furni removal


