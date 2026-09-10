# Eukyre's Consortium of Things — SPT 4.1

원작 **ECOT (Eukyre's Consortium of Things)** by **ProbablyEukyre** 를 **SPT 4.1.5** 로 포팅했습니다.
원작자 : **Created by probablyEukyre**
포지 : https://sp-mod.com/mod/2195/ecot-eukyres-consortium-of-things
프레임워크 원작: GrooveypenguinX · MIT 라이선스

**정말 미안합니다**

Glock 22, .40 S&W 탄종, .338 LM RIP, Unity FAST COG 마운트, Blahaj 등 184개 아이템을 추가합니다.

## 필수 의존성

> **WTT-ServerCommonLib 3.x 가 반드시 필요합니다.**
>
> 3.11 시절엔 이 모드가 WTT 아이템 프레임워크를 **타입스크립트로 복사해서 안에 들고 있었습니다.**
> 4.1에서는 그게 정식 C# 패키지(`WTT-ServerCommonLib`)로 존재하기 때문에, 848줄을 다시 구현하는
> 대신 그 라이브러리에 위임합니다. 없으면 아이템이 하나도 안 올라옵니다.

## 설치

`SPT\user\mods\EukyreConsortium\` 에 통째로 넣으면 됩니다 (`db`, `bundles`, dll, `bundles.json`).

## 빌드

.NET 10 SDK 만 있으면 됩니다. NuGet 이 SPTushonka 4.1.5 와 WTT-ServerCommonLib 3.0.6 을 알아서 받습니다.

```
dotnet build src-cs/EukyreConsortium.csproj -c Release
```

SPT 경로 기본값은 `E:\SPT 4.1`. 다르면:

```
dotnet build src-cs/EukyreConsortium.csproj -c Release -p:SptRoot="D:\내SPT경로"
```

빌드가 끝나면 `SPT\user\mods\EukyreConsortium\` 로 **dll + db + 번들 1.1GB** 가 전부 복사됩니다.

번들 복사가 느려서 코드만 확인하고 싶을 때는 배포를 끄면 됩니다:

```
dotnet build src-cs/EukyreConsortium.csproj -c Release -p:DeployToSpt=false
```

> 빌드에는 WTT-ServerCommonLib **NuGet 패키지**만 있으면 되지만, **게임을 켜려면 WTT-CommonLib
> 모드 자체도 설치**돼 있어야 합니다 (SPT Forge / GitHub 에서 받아 SPT 폴더에 압축 해제).
> 빌드 결과물에 라이브러리 dll 은 안 들어갑니다 — 그 모드가 제공합니다.

---

## 4.1 포팅에서 바뀐 것

### 1. 타입스크립트 → C#, 단 848줄을 옮긴 게 아닙니다

SPT 4.x 서버는 C#입니다. 그런데 이 모드의 `CustomItemService.ts` 848줄은 **WTT 프레임워크를
베껴온 것**이었고, 그 프레임워크는 4.1용 C#으로 이미 존재합니다:

| 3.11 TS가 손수 하던 일 | 4.1 WTT-ServerCommonLib |
| --- | --- |
| `processStaticLootContainers` | `StaticLootHelper` |
| `processModSlots` | `ModSlotHelper` |
| `processInventorySlots` | `InventorySlotHelper` |
| `processMasterySections` | `MasteryHelper` |
| `processWeaponPresets` | `WeaponPresetHelper` |
| `processTraders` | `TraderItemHelper` |
| `addtoHallofFame` / `addtoSpecialSlots` | `HallOfFameHelper` / `SpecialSlotsHelper` |
| `processBotInventories` | `BotLootHelper` |
| `CustomAssortSchemeService.ts` | `WTTCustomAssortSchemeService` |
| `CustomWeaponPresets.ts` | `WTTCustomWeaponPresetService` |
| `WTTInstanceManager.ts` | 필요 없음 (생성자 주입) |

그래서 **C# 코드는 약 250줄**이고, 대부분은 아래 두 개(라이브러리에 없는 기능)입니다.
나머지는 라이브러리에 db 폴더를 넘기는 게 전부입니다.

### 2. 진짜 작업은 JSON 184개 마이그레이션이었습니다

4.1의 JSON 파싱은 **대소문자를 구분**합니다 (`PropertyNameCaseInsensitive` 미설정). 3.11 스키마
그대로는 절반이 조용히 무시됩니다. `db/Items` → `db/CustomItems` 로 변환했습니다.

| 3.11 | 4.1 | 건수 |
| --- | --- | --- |
| `traderId` + `traderItems` + `barterScheme` + `loyallevelitems` | `traders` (중첩 dict) | 147 |
| `_tpl: "ROUBLES"` | `MONEY_ROUBLES` | 180 |
| `StaticLootContainers[].ContainerName/Probability` | `staticLootContainers[].containerName/probability` | 179 |
| `addweaponpreset` / `weaponpresets` | `addWeaponPreset` / `weaponPresets` | 55 |
| `addtoragfair` | `registerInFleaPrices` | 31 |
| `StaticLootContainer`(단수) + `Probability` | 위 리스트로 합침 | 6 |
| `clearClonedProps`, `ModdableItemWhitelist` | 4.1에 없음 (전부 기본값이라 무해) | 184 |

트레이더 어소트 항목마다 새 MongoId가 필요한데, 아이템 id에서 **결정적으로 유도**했습니다
(`sha1("ecot-assort:<itemId>:<n>")` 앞 24자). 변환기를 다시 돌려도 id가 안 바뀝니다.

### 3. 4.1의 엄격한 타입이 잡아낸 원본 데이터 버그 3개

- **`handbookParentId: "MOD_REFLEXSIGHT"` (12개)** — WTT의 핸드북 카테고리 맵에 그 키가
  없습니다. 3.11 모드 자체 테이블이 해석하던 리터럴 id `5b5f742686f774093e6cb4ff` 로 바꿨습니다.
- **`masterySections[].Templates: ["SerbuShotgun"]` (마운트 3개)** — 4.1은 `Templates` 를
  `MongoId[]` 로 강제합니다. 3.11은 아무 문자열이나 받았고, 이 값은 **어떤 무기와도 매칭된 적이
  없어서 원래부터 죽은 데이터**였습니다. 없는 id를 지어내는 대신 제거했습니다.
- **`db/CustomWeaponPresets/WeaponPresets.json` 의 래퍼** — 3.11 코드가 `data.ItemPresets` 로
  한 겹 벗겨서 읽었기 때문에 파일이 `{"ItemPresets": {...}}` 모양이었습니다. WTT 는 `{프리셋id: Preset}`
  **평면 dict** 를 기대하므로, `"ItemPresets"` 를 프리셋 이름으로 읽고 그 안이 비어 있어
  `Object reference not set to an instance of an object` 를 냈습니다 (실기동 로그에서 발견).
  원본도 어차피 프리셋 0개였고, 실제 프리셋 4개는 아이템 설정의 `weaponPresets` 로 들어갑니다.

### 4. 라이브러리에 없어서 직접 짠 것

- **`CompatibilityEdits`** (구 `EpicsEdits.ts` 195줄) — 바닐라/타 모드 아이템의 필터를 넓혀
  ECOT 부품·탄약이 들어가게 하는 작업. 전부 "이 필터에 이 tpl 추가" 한 가지 모양이라 **데이터
  + 적용기 하나**로 바꿨습니다 (`db/CompatibilityEdits/CompatibilityEdits.json`, 대상 20개).
  .338 약실 3개는 3.11이 `_props.Chambers` 를 통째로 갈아끼웠는데, **기존 약실에 추가하는 방식**
  으로 바꿨습니다 — 결과는 같고, 다른 모드가 먼저 건드린 약실을 날리지 않습니다.
- **`ModdableItemBlacklist`** — "이 총에는 붙지 마라" 목록. WTT에 대응 기능이 없습니다.
  아이템 설정에 데이터를 그대로 두고(라이브러리는 무시), 등록 후 해당 슬롯 필터에서 다시
  빼는 패스를 넣었습니다. 16개 아이템이 이걸 씁니다.

### 5. 로드 순서 (라이브러리와 맞물리는 부분)

이게 조용히 틀리기 쉬운 부분이라 실제 `TypePriority` 를 어셈블리에서 읽어 맞췄습니다.

| 순서 | 누가 | TypePriority |
| --- | --- | --- |
| 1 | WTT 라이브러리 진입점 (Harmony 패치, 자체 로케일) | 100,000 |
| 2 | **ECOT 등록** (아이템 / 어소트 / 프리셋 / 로케일) | **300,010** |
| 3 | WTT `PostSptLoad` — 구경·모드슬롯·시큐어필터 지연 패스 | 1,000,000 |
| 4 | **ECOT 후처리** (CompatibilityEdits + ModdableItemBlacklist) | **1,000,020** |

여기서 두 가지를 고쳤습니다:

- **지연 패스를 직접 호출하지 않습니다.** 라이브러리가 4번 직전(`PostSptLoad`)에 스스로 돌립니다.
  2번 단계에서 부르면 다른 모드가 아직 아이템을 등록하기 전이라 호환이 누락되고, 그 뒤에 라이브러리가
  한 번 더 돌립니다.
- **`ModdableItemBlacklist` 는 반드시 3번 뒤여야 합니다.** 라이브러리가 모드 슬롯에 아이템을 넣는 게
  3번인데, 그 전에 빼봐야 뺄 게 없습니다.

### 6. 그 밖

- `[Injectable(InjectionType.Singleton)]` 명시 — 4.1이 기본값을 `Scoped` → `Transient` 로 바꿨습니다
- `IModMetadata` + `OnLoadAsync(CancellationToken)`, `ModDependencies` 에 WTT commonlib 선언
- `QuestModifier.ts` (259줄) — **3.11에서도 `mod.ts` 에 연결돼 있지 않은 죽은 코드**였습니다.
  포팅하지 않았습니다. 딸린 `db/Quests/QuestSideData.json` 도 쓰이지 않습니다.
- `package.json` / `packageBuild.ts` / `src/*.ts` 삭제

## 검증

컴파일만으로는 JSON이 실제로 읽히는지 알 수 없어서, **WTT-ServerCommonLib의 진짜 `CustomItemConfig`
모델로 184개를 전부 역직렬화**하는 하네스를 돌렸습니다. SPT의 JSON 컨버터(MongoId, StringOrInt,
ListOrT 등)를 같이 등록해서 서버와 같은 조건으로 파싱합니다. 추가로:

- 라이브러리 자체 검증(`GetValidationErrors`) 통과 여부
- `itemTplToClone` / `parentId` / `handbookParentId` 심볼이 WTT 맵 또는 `ItemTpl` 에서 실제로 해석되는지
- 트레이더 키와 물물교환 `_tpl` 이 해석되는지

**결과: 184/184 통과.** 위 3번의 버그 2개가 이 하네스에서 나왔습니다.

## 상태

- 빌드: **성공**
- JSON 마이그레이션: **184/184 검증 통과**
- 인게임 테스트: **아직 안 함**
