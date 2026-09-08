using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using Path = System.IO.Path;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace EukyreConsortium;

/// <summary>
/// The old EpicsEdits.ts: widening filters on vanilla and other mods' items so this mod's ammo
/// and attachments fit where they belong. Rewritten as data plus one applier, because every
/// edit was the same shape - "add these tpls to that filter".
///
/// <para>The three .338 chamber entries replaced <c>_props.Chambers</c> outright on 3.11.
/// Appending to the chamber that is already there is equivalent and does not throw away a
/// chamber another mod may have touched first.</para>
/// </summary>
[Injectable(InjectionType.Singleton)]
public class CompatibilityEdits(
    TemplateTable templates,
    ModHelper modHelper,
    ISptLogger<CompatibilityEdits> logger)
{
    private sealed class EditFile
    {
        [JsonPropertyName("cartridgeFilterAdds")]
        public Dictionary<string, List<string>> CartridgeFilterAdds { get; set; } = [];

        [JsonPropertyName("chamberFilterAdds")]
        public Dictionary<string, List<string>> ChamberFilterAdds { get; set; } = [];

        [JsonPropertyName("slotFilterAdds")]
        public Dictionary<string, Dictionary<int, List<string>>> SlotFilterAdds { get; set; } = [];

        [JsonPropertyName("numericPropOverrides")]
        public Dictionary<string, Dictionary<string, double>> NumericPropOverrides { get; set; } = [];
    }

    public void Apply(Assembly assembly)
    {
        var path = Path.Combine(modHelper.GetAbsolutePathToModFolder(assembly),
                                "db", "CompatibilityEdits", "CompatibilityEdits.json");
        if (!File.Exists(path))
        {
            logger.Warning($"[ECOT] compatibility edits not found at {path}");
            return;
        }

        EditFile? edits;
        try
        {
            edits = JsonSerializer.Deserialize<EditFile>(File.ReadAllText(path),
                new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
        }
        catch (Exception ex)
        {
            logger.Error($"[ECOT] could not read compatibility edits: {ex.Message}");
            return;
        }
        if (edits is null) return;

        var applied = 0;

        foreach (var (tpl, add) in edits.CartridgeFilterAdds)
            applied += AddToFirstFilter(tpl, add, item => item.Properties?.Cartridges, "cartridge");

        foreach (var (tpl, add) in edits.ChamberFilterAdds)
            applied += AddToFirstFilter(tpl, add, item => item.Properties?.Chambers, "chamber");

        foreach (var (tpl, bySlot) in edits.SlotFilterAdds)
        {
            if (!TryGet(tpl, out var item)) continue;
            var slots = item.Properties?.Slots?.ToList();
            foreach (var (index, add) in bySlot)
            {
                if (slots is null || index >= slots.Count)
                {
                    logger.Warning($"[ECOT] {tpl} has no slot #{index}; skipping");
                    continue;
                }
                applied += AddIds(slots[index].Properties?.Filters?.FirstOrDefault()?.Filter, add);
            }
        }

        foreach (var (tpl, props) in edits.NumericPropOverrides)
        {
            if (!TryGet(tpl, out var item) || item.Properties is null) continue;
            foreach (var (name, value) in props)
            {
                var p = item.Properties.GetType().GetProperty(name);
                if (p is null || !p.CanWrite)
                {
                    logger.Warning($"[ECOT] {tpl} has no writable property '{name}'");
                    continue;
                }
                try
                {
                    var target = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    p.SetValue(item.Properties, Convert.ChangeType(value, target));
                    applied++;
                }
                catch (Exception ex)
                {
                    logger.Warning($"[ECOT] could not set {tpl}.{name}: {ex.Message}");
                }
            }
        }

        logger.Info($"[ECOT] compatibility edits applied ({applied} filter/property change(s))");
    }

    private bool TryGet(string tpl, out TemplateItem item)
    {
        if (templates.Items.TryGetValue(new MongoId(tpl), out var found) && found is not null)
        {
            item = found;
            return true;
        }
        // Expected when the mod this edit targets is not installed.
        logger.Debug($"[ECOT] item {tpl} not in the database, skipping its compatibility edit");
        item = null!;
        return false;
    }

    private int AddToFirstFilter(string tpl, List<string> add,
        Func<TemplateItem, IEnumerable<Slot>?> pick, string what)
    {
        if (!TryGet(tpl, out var item)) return 0;
        var target = pick(item)?.FirstOrDefault();
        if (target is null)
        {
            logger.Warning($"[ECOT] {tpl} has no {what} to widen");
            return 0;
        }
        return AddIds(target.Properties?.Filters?.FirstOrDefault()?.Filter, add);
    }

    private static int AddIds(HashSet<MongoId>? filter, List<string> add)
    {
        if (filter is null) return 0;
        var n = 0;
        foreach (var id in add)
            if (filter.Add(new MongoId(id))) n++;
        return n;
    }
}
