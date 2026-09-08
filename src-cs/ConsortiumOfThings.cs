using System.Reflection;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace EukyreConsortium;

/// <summary>
/// Everything this mod adds is data. On 3.11 the mod shipped its own copy of the WTT item
/// framework in TypeScript; on 4.1 that framework is WTT-ServerCommonLib, so the mod points it
/// at its own db folders and the library does the registering.
///
/// <para>Runs after trader registration so the assort work lands on traders that already exist.
/// The library's own entry point sits at <see cref="OnLoadOrder.Preload"/>, so its Harmony
/// patches are in place before any of this.</para>
/// </summary>
[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.TraderRegistration + 10)]
public class ConsortiumOfThings(
    WTTServerCommonLib.WTTServerCommonLib wtt,
    ISptLogger<ConsortiumOfThings> logger) : IOnLoad
{
    internal const string ModName = "Eukyre's Consortium of Things";

    public async Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        var self = Assembly.GetExecutingAssembly();

        await wtt.CustomItemServiceExtended.CreateCustomItems(self, Path.Combine("db", "CustomItems"));
        await wtt.CustomAssortSchemeService.CreateCustomAssortSchemes(self, Path.Combine("db", "CustomAssortSchemes"));
        await wtt.CustomWeaponPresetService.CreateCustomWeaponPresets(self, Path.Combine("db", "CustomWeaponPresets"));
        await wtt.CustomLocaleService.CreateCustomLocales(self, Path.Combine("db", "locales"));

        // The deferred caliber/mod-slot/secure-filter passes are NOT called here on purpose:
        // WTTServerCommonLibPostSptLoad runs them at OnLoadOrder.PostLoad, once every mod has
        // registered its items. Running them early would miss other mods and then run twice.

        logger.Success($"[{ModName}] Database: Loading complete.");
    }
}

/// <summary>
/// The two things WTT-ServerCommonLib has no equivalent for. Both have to run after the
/// library's own post-load pass (OnLoadOrder.PostLoad), because that is where it finally writes
/// this mod's items into other items' mod slots - the blacklist has nothing to undo before then.
/// </summary>
[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.PostLoad + 20)]
public class ConsortiumOfThingsPostLoad(
    CompatibilityEdits compatibilityEdits,
    ModdableItemBlacklist moddableItemBlacklist) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        var self = Assembly.GetExecutingAssembly();
        compatibilityEdits.Apply(self);
        moddableItemBlacklist.Apply(self);
        return Task.CompletedTask;
    }
}
