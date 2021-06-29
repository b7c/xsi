#r "nuget: System.IO.Pipelines, 5.0.1"

#load "xsi-loader.csx"
#load "xsi-extension.csx"
#load "xsi-globals.csx"

#load "util/game-data.csx"
#load "util/intercepts.csx"
#load "util/packet-deconstruction.csx"

#load "util/future.csx"

using Xabbo.Messages;
using Xabbo.Core;

using static Xabbo.ClientType;
using static Xabbo.Messages.Packet;

WriteLine($"Xabbo Script Interactive v{VERSION}");

bool __initError = false;
try {
    WriteLine($"Loading game data...");

    await __gameDataManager.InitializeAsync();
    await Task.WhenAll(
        __gameDataManager.GetExternalTextsAsync(),
        __gameDataManager.GetProductDataAsync(),
        __gameDataManager.GetFigureDataAsync(),
        __gameDataManager.GetFurniDataAsync()
    );

    Task extensionTask = __xtn.RunAsync()
        .ContinueWith(t => Environment.Exit(-1));

    WriteLine("Waiting for connection...");
    await __xtn.ConnectSignal;
} catch {
    __initError = true;
    throw;
} finally {
    if (__initError)
        Environment.Exit(0);
}

WriteLine("Interactive mode enabled.\n");