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

### Sending and receiving packets
Shout "hello, world":
```cs
Send(Out.Shout, "hello, world", 0)
```

Craft a packet manually and send it:
```cs
var p = new Packet(Out.Shout);
p.WriteString("hello, world");
p.WriteInt(0);
Send(p);
```

Receive and read from a packet:
```cs
Send(Out.GetCredits); var p = Receive(5000, In.WalletBalance); // Wait 5000ms to receive a packet with the WalletBalance header
p.ReadString() // Returns the amount of credits in your wallet
```
Notice the send and receive are on the same line.\
If you enter the send line first, it's likely the response to the packet would be received before you execute the next line to capture the packet.\
A better way to do this could be to create a receiver task first, send the request and then await the task:
```cs
var recv = ReceiveAsync(-1, In.WalletBalance); // Use -1 for no timeout
Send(Out.GetCredits);
var p = await recv;
p.ReadString() // Returns the amount of credits in your wallet
```
If it gets stuck awaiting the receiver task, you can press Ctrl+C to cancel it.\
A cancellation token source is created and passed into each receive/delay task to allow this.\
Use `Delay(ms)` or `await DelayAsync(ms)` to delay execution.

### Game state
Game state is being managed in the background to provide information about the current state of the room, its furni and entities, etc.

Get the current room ID:
```cs
RoomId
```

List all users in the room:
```cs
foreach (var user in Users) WriteLine(user.Name);
```

Count furni in the room:
```cs
Furni.Count()
FloorItems.Count()
WallItems.Count()
```

### Interactions
There are various methods defined in `xsi-globals.csx` to make it easier to interact with the game.\
These are just a few of the methods available.

Talk, shout or whisper:
```cs
Talk("Hello, world");
Shout("Hello, world!");
Whisper("world", "Hello, world.");
```

Walk to a tile:
```cs
Walk(5, 6);
```

Search for a user and grab their ID, then get their profile by their ID:
```cs
var result = SearchUser("name");
result.Id
var profile = GetProfile(result.Id);
profile
```

## Example scripts
Output the current room's floor plan to a text file:
```cs
File.WriteAllText($"floorplan-{Room.Id}.txt", FloorPlan.ToString())
```

Send a friend request to everyone in the room:
```cs
foreach (var user in Users) {
  Send(Out.RequestFriend, user.Name);
  WriteLine($"> {user.Name}");
  Delay(1000);
}
```

Download all photos in a room to the directory `photos/roomId`:
```cs
string dir = $"photos/{Room.Id}";
Directory.CreateDirectory(dir);
var photos = WallItems.OfKind(4597).ToArray(); // .com furni kind, furni data support will be added soon
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
