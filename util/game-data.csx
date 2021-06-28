#load "../services/game-data-manager.csx"

using Xabbo.Core.Web;
using Xabbo.Core;
using Xabbo.Core.GameData;

static GameDataManager __gameDataManager = new();
static FurniData FurniData => __gameDataManager.FurniData;
static FigureData FigureData=> __gameDataManager.FigureData;
static ProductData ProductData => __gameDataManager.ProductData;
static ExternalTexts Texts => __gameDataManager.ExternalTexts;

// Furni info extension methods
static ItemDescriptor GetDescriptor(this IItem item) => FurniData.GetItemDescriptor(item);

static FurniInfo GetInfo(this IItem item) => FurniData.GetInfo(item);
static FurniInfo GetInfo(this ItemDescriptor descriptor) => FurniData.GetInfo(descriptor.Type, descriptor.Kind);

static string GetIdentifier(this IItem item) => GetInfo(item)?.Identifier;
static string GetIdentifier(this ItemDescriptor descriptor) => GetInfo(descriptor)?.Name;

static string GetName(this ItemDescriptor descriptor) {
    var info = GetInfo(descriptor);
    if (info.Identifier == "poster") {
        if (!string.IsNullOrWhiteSpace(descriptor.Variant)) {
            if (Texts.TryGetValue($"poster_{descriptor.Variant}_name", out string posterName))
                return posterName;
        }
    }
    return string.IsNullOrWhiteSpace(info.Name) ? $"[{info.Identifier}]" : info.Name;
}
static string GetName(this IItem item) => GetName(GetDescriptor(item));

// Furni enumerable methods
static IEnumerable<TItem> OfKind<TItem>(this IEnumerable<TItem> items, string identifier)
    where TItem : IItem => items.OfKind(FurniData[identifier]);

static IEnumerable<TItem> OfKind<TItem>(this IEnumerable<TItem> items, IItem item)
    where TItem : IItem => items.OfKind(item.Type, item.Kind);