using System.Reflection;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using WTTServerCommonLib.Services;

namespace EukyreConsortium;

/// <summary>
/// Everything this mod adds is data. On 3.11 the mod shipped its own copy of the WTT item
/// framework in TypeScript; on 4.1 that framework is WTT-ServerCommonLib, so the mod just
/// points it at its own db folders and the library does the registering.
///
/// <para>Runs after trader registration so the assort work lands on traders that already exist.</para>
/// </summary>
[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.TraderRegistration + 10)]
public class ConsortiumOfThings(
    WTTCustomItemServiceExtended items,
    WTTCustomAssortSchemeService assorts,
    WTTCustomWeaponPresetService weaponPresets,
    WTTCustomLocaleService locales,
    CompatibilityEdits compatibilityEdits,
    ModdableItemBlacklist moddableItemBlacklist,
    ISptLogger<ConsortiumOfThings> logger) : IOnLoad
{
    private const string ModName = "Eukyre's Consortium of Things";

    public async Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        var self = Assembly.GetExecutingAssembly();

        await items.CreateCustomItems(self, Path.Combine("db", "CustomItems"));
        await assorts.CreateCustomAssortSchemes(self, Path.Combine("db", "CustomAssortSchemes"));
        await weaponPresets.CreateCustomWeaponPresets(self, Path.Combine("db", "CustomWeaponPresets"));
        await locales.CreateCustomLocales(self, Path.Combine("db", "locales"));

        // Deferred passes the library only runs once every mod has registered its items.
        items.ProcessDeferredCalibers();
        items.ProcessDeferredModSlots();
        items.ProcessDeferredSecureFilters();

        // Two things the library has no equivalent for, so the mod still does them itself.
        compatibilityEdits.Apply(self);
        moddableItemBlacklist.Apply(self);

        logger.Success($"[{ModName}] Database: Loading complete.");
    }
}
