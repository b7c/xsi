#r "nuget: System.IO.Pipelines, 5.0.1"

#load "xsi-xtn.csx"
#load "xsi-globals.csx"

using Xabbo.Messages;
using Xabbo.Core;

_ = __xtn.RunAsync();

WriteLine($"Xabbo Script Interactive v{VERSION}");

WriteLine("Waiting for connection...");
await __xtn.ConnectSignal;

WriteLine("Interactive mode enabled.\n");