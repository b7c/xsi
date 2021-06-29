# Xabbo Script Interactive
An interactive C# scripting shell for [G-Earth](https://github.com/sirjonasxx/G-Earth) which utilizes the
[Xabbo.Common](https://www.github.com/b7c/Xabbo.Common),
[Xabbo.GEarth](https://www.github.com/b7c/Xabbo.GEarth) and
[Xabbo.Core](https://www.github.com/b7c/Xabbo.Core) libraries.

## Installation

- Install the [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0).
  - Choose the installer for your operating system.
- Install [dotnet-script](https://github.com/filipw/dotnet-script).
  - Run `dotnet tool install -g dotnet-script` in the terminal.
- Download the latest release and place the files into a folder.
- If needed, get the latest [messages.ini](https://github.com/b7c/Xabbo.Common/blob/master/Xabbo.Common/messages.ini) from Xabbo.Common and place it in the folder.

## How to run
- Open a new terminal at the directory where the `xsi.csx` file is.
- Run `dotnet script xsi.csx -i`.
- Wait for it to connect to G-Earth and a game connection to be established.
- Once you see "interactive mode enabled" you can then start interacting with the script engine.

## Usage
### Accessing message headers
```cs
Out.Move
```
```cs
In.Talk
```
Message names are based on the ones defined in the Unity client.
You can still access a Flash header by its name:
```cs
Out["PlaceObject"] // Will return the header for Out.PlaceRoomItem
```
However this must be defined in `messages.ini` for it to be correctly mapped to the Unity message name.

### Sending, receiving, reading and writing packets
- Shout "hello, world":
```cs
Send(Out.Shout, "hello, world", 0)
```

- Craft a packet manually and send it:
```cs
var p = new Packet(Out.Shout);
p.WriteString("hello, world");
p.WriteInt(0);
Send(p);
// OR
var p = new Packet(Out.Shout)
  .WriteString("hello, world")
  .WriteInt(0);
Send(p);
```

- Receive and read from a packet:
```cs
Send(Out.GetCredits); var p = Receive(5000, In.WalletBalance); // Wait 5000ms to receive a packet with the WalletBalance header
p.ReadString() // Returns the amount of credits in your wallet
```
Notice the send and receive are on the same line. If you enter the send line first, it's likely the response to the packet would already be received by the time you enter the next line to capture the packet.\
Another way to do this could be to create a receiver task first, send the request and then await the task:
```cs
var recv = ReceiveAsync(-1, In.WalletBalance); // Use -1 for no timeout
Send(Out.GetCredits);
var p = await recv;
p.ReadString() // Returns the amount of credits in your wallet
```
If it gets stuck awaiting the receiver task, you can press Ctrl+C to cancel it.\
A cancellation token source is created and passed into each receive/delay task to allow this.

Use `Delay(ms)` or `await DelayAsync(ms)` to delay execution.

- Packet deconstruction syntax:
```cs
var p = Receive(-1, In.Chat);
// read an int, string, skip an int, read an int
var (index,msg,_,bubble) = p.Read<int,string,int,int>();
WriteLine($"{GetUser(index).Name}: {msg} ({bubble})");
```

### Intercepting and blocking packets, modifying data

- Register an intercept callback for a certain header:
```cs
Intercept(Out.Move, e => Print(e.Packet.Read<int,int>()));
```
Now each time you click a tile to move, the packet will be intercepted and the tile coordinates will be printed to the terminal.

- Remove the intercept callbacks for a certain header:
```cs
ClearIntercepts(Out.Move);
```
Calling this method with no arguments will remove all registered intercepts.

- Blocking packets:
```cs
Intercept(Out.LookTo, e => e.Block()); // prevents turning when selecting another user
```

- Modifying values in a packet:
```cs
// skip an int, string, int, then replace an int with 38
Intercept(In.Chat, e => e.Packet.Replace(Int, String, Int, 38));
```
```cs
// replace a string from the 5th byte (0 based index -> 4, skips the first 4-byte integer)
// using a transform function to change it to uppercase
Intercept(In.Chat, e => e.Packet.ReplaceAt(4, s => s.ToUpper()));
```

### Interactions
There are various methods defined in `xsi-globals.csx` to make it easier to interact with the game.\
These are just a few of the methods available.

- Talk, shout or whisper:
```cs
Talk("Hello, world");
Shout("Hello, world!");
Whisper("world", "Hello, world.");
```

- Walk to a tile:
```cs
Walk(5, 6);
```

- Search for a user and grab their ID, then get their profile by their ID:
```cs
var result = SearchUser("name");
result.Id
var profile = GetProfile(result.Id);
profile
```

### Game state
Game state is being managed in the background to provide information about the current state of the room, its furni and entities, etc.

- Get the current room ID:
```cs
RoomId
```

- List all users in the room:
```cs
foreach (var user in Users) WriteLine(user.Name);
```

- Count furni in the room:
```cs
Furni.Count()
FloorItems.Count()
WallItems.Count()
```

### Game data support

The current furni, figure, product data and external texts are loaded when the script starts.\
They can be accessed using `FurniData`, `FigureData`, `ProductData` and `Texts` respectively.\
To get the data of a furni you can use `FurniData.GetInfo(ItemType type, int kind)` where `type` is either `ItemType.Floor` or `ItemType.Wall`, and `kind` is the furni's ID specifier. (ex. `ItemType.Floor, 179` = Rubber Duck).\
Each furni also has a unique string identifier (its "class name").\
For example the rubber duck's identifier is `duck`, and this can be accessed using `FurniData["duck"]`.
There are also helpful extension methods to get a furni's info or name from its item instance.\
For example, to grab the first furni in a room and get its name:
```cs
var item = Furni.First();
item.GetName()
```

IItem enumerables can be filtered by a furni info:
```cs
// find furni info by name
// note this may not be the exact info you're looking for
// if another furni shares the same name
var info = FurniData.FindFloorItem("Rubber Duck");
Furni.OfKind(info).Count() // count the number of Rubber Duck furni in the room
```

Or a furni identifier:
```cs
Furni.OfKind("duck").Count() // count the number of Rubber Duck furni in the room
```

This can be useful for example to pick up all furni in the room of a specific type:
```cs
// pick up all Rubber Duck furni in the room
foreach (var item in Furni.OfKind("duck")) {
  Pickup(item);
  Delay(150);
}
```

## Example scripts
- Output the current room's floor plan to a text file:
```cs
File.WriteAllText($"floorplan-{Room.Id}.txt", FloorPlan.OriginalString)
```

- Send a friend request to everyone in the room:
```cs
foreach (var user in Users) {
  Send(Out.RequestFriend, user.Name);
  WriteLine($"> {user.Name}");
  Delay(1000);
}
```

- List the name and count of all furni in the room:
```cs
// group all furni by its descriptor
// using the IItem.GetDescriptor() extension method.
// the descriptor includes the item type,
// kind and a variant string (used for posters).
foreach (var group in Furni.GroupBy(GetDescriptor)) {
  string name = group.Key.GetName(); // call GetName() on the furni descriptor
  int count = group.Count();
  WriteLine($"{count,6:N0} x {name}");
}
```
This can also be done with your inventory. Just replace Furni with `GetInventory()`.\
Note you must be in a room to load your inventory.\
```cs
foreach (var group in GetInventory().GroupBy(GetDescriptor))
  WriteLine($"{group.Count(),6:N0} x {group.Key.GetName()}");
```

- Download all photos in a room to the directory `photos/roomId`:
```cs
string dir = $"photos/{Room.Id}";
Directory.CreateDirectory(dir);
var photos = WallItems.OfKind("external_image_wallitem_poster_small").ToArray();
for (int i = 0; i < photos.Length; i++) {
  string filePath = Path.Combine($"photos/{Room.Id}", $"{photos[i].Id}.png");
  if (File.Exists(filePath)) continue;
  WriteLine($"Downloading {i+1}/{photos.Length}");
  try {
    var photoInfo = System.Text.Json.JsonSerializer.Deserialize<PhotoInfo>(photos[i].Data);
    var photoData = await H.GetPhotoDataAsync(photoInfo.Id);
    byte[] image = await H.DownloadPhotoAsync(photoData);
    File.WriteAllBytes(filePath, image);
  } catch (Exception ex) {
    WriteLine($"Failed to download: {ex.Message}");
  }
}
```
