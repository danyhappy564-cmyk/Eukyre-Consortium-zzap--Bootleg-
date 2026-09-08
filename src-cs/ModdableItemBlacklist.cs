using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace EukyreConsortium;

/// <summary>
/// WTT-ServerCommonLib's mod-slot pass has no "except on these guns" list, but the 3.11 mod did
/// (<c>ModdableItemBlacklist</c>) and 16 items rely on it. The item configs still carry the key -
/// the library ignores it - so this runs afterwards and takes the item back out of the slots of
/// the parents it was never meant to fit.
/// </summary>
[Injectable(InjectionType.Singleton)]
public class ModdableItemBlacklist(
    TemplateTable templates,
    ModHelper modHelper,
    ISptLogger<ModdableItemBlacklist> logger)
{
    private sealed class Entry
    {
        [JsonPropertyName("ModdableItemBlacklist")]
        public List<string>? Blacklist { get; set; }
    }

    public void Apply(Assembly assembly)
    {
        var dir = Path.Combine(modHelper.GetAbsolutePathToModFolder(assembly), "db", "CustomItems");
        if (!Directory.Exists(dir)) return;

        var opts = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
        var removed = 0;
        var pairs = 0;

        foreach (var file in Directory.GetFiles(dir, "*.json"))
        {
            Dictionary<string, Entry>? configs;
            try
            {
                configs = JsonSerializer.Deserialize<Dictionary<string, Entry>>(File.ReadAllText(file), opts);
            }
            catch (Exception ex)
            {
                logger.Warning($"[ECOT] could not read blacklist data from {Path.GetFileName(file)}: {ex.Message}");
                continue;
            }
            if (configs is null) continue;

            foreach (var (itemId, entry) in configs)
            {
                if (entry.Blacklist is not { Count: > 0 }) continue;
                var id = new MongoId(itemId);

                foreach (var parentTpl in entry.Blacklist)
                {
                    pairs++;
                    if (!templates.Items.TryGetValue(new MongoId(parentTpl), out var parent) || parent?.Properties?.Slots is null)
                    {
                        continue;
                    }
                    foreach (var slot in parent.Properties.Slots)
                    {
                        foreach (var filter in slot.Properties?.Filters ?? [])
                        {
                            if (filter.Filter?.Remove(id) == true) removed++;
                        }
                    }
                }
            }
        }

        logger.Info($"[ECOT] moddable-item blacklist: {removed} slot filter entr(y/ies) removed across {pairs} item/parent pair(s)");
    }
}
