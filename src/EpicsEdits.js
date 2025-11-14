"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.epicItemClass = void 0;
const WTTInstanceManager_1 = require("./WTTInstanceManager");
class epicItemClass {
    Instance = new WTTInstanceManager_1.WTTInstanceManager(); // Based upon EpicRangeTime's edits. Cheers, epic! --Eukyre
    preSptLoad(Instance) {
        this.Instance = Instance;
    }
    postDBLoad() {
        this.epicEdits();
    }
    epicEdits() {
        const db = this.Instance.database;
        const dbItems = db.templates.items;
        for (let file in dbItems) {
            let fileData = dbItems[file];
            if (fileData._id === "673cbdfad0453ba50c0f76d6") {
                fileData._props.Cartridges[0]._props.filters[0].Filter.push("68561ab84857b945e0ce85e9", "6859749fb61f50b920ae5a2a");
            } //Pushing .338LM RIP to Sako M10 Mags
            if (fileData._id === "628120fd5631d45211793c9f") {
                fileData._props.Cartridges[0]._props.filters[0].Filter.push("68561ab84857b945e0ce85e9", "6859749fb61f50b920ae5a2a");
            } //Pushing .338LM RIP to AXMC Mags
            if (fileData._id === "5fc23426900b1d5091531e15") {
                fileData._props.Cartridges[0]._props.filters[0].Filter.push("68561ab84857b945e0ce85e9", "6859749fb61f50b920ae5a2a");
            } //Pushing .338LM RIP to MK-18 Mags
            if (fileData._id === "673cab3e03c6a20581028bc1") {
                fileData._props.Chambers = [
                    {
                        "_name": "patron_in_weapon",
                        "_id": "673cab3e03c6a20581028bc6",
                        "_parent": "673cab3e03c6a20581028bc1",
                        "_props": {
                            "filters": [
                                {
                                    "Filter": [
                                        "5fc382a9d724d907e2077dab",
                                        "5fc275cf85fd526b824a571a",
                                        "5fc382c1016cce60e8341b20",
                                        "5fc382b6d6fa9c00c571bbc3",
                                        "68561ab84857b945e0ce85e9",
                                        "6859749fb61f50b920ae5a2a"
                                    ]
                                }
                            ]
                        },
                        "_required": false,
                        "_mergeSlotWithChildren": false,
                        "_proto": "55d4af244bdc2d962f8b4571",
                    }
                ];
            } //Adding .338 LM RIP to the Sako TRG M10
            if (fileData._id === "627e14b21713922ded6f2c15") {
                fileData._props.Chambers = [
                    {
                        "_name": "patron_in_weapon",
                        "_id": "627e14b21713922ded6f2c1a",
                        "_parent": "627e14b21713922ded6f2c15",
                        "_props": {
                            "filters": [
                                {
                                    "Filter": [
                                        "5fc382a9d724d907e2077dab",
                                        "5fc275cf85fd526b824a571a",
                                        "5fc382c1016cce60e8341b20",
                                        "5fc382b6d6fa9c00c571bbc3",
                                        "68561ab84857b945e0ce85e9",
                                        "6859749fb61f50b920ae5a2a"
                                    ]
                                }
                            ]
                        },
                        "_required": false,
                        "_mergeSlotWithChildren": false,
                        "_proto": "55d4af244bdc2d962f8b4571",
                    }
                ];
            } //Adding .338 LM RIP to the AXMC
            if (fileData._id === "5fc22d7c187fea44d52eda44") {
                fileData._props.Chambers = [
                    {
                        "_name": "patron_in_weapon",
                        "_id": "5fc22d7c187fea44d52eda4b",
                        "_parent": "5fc22d7c187fea44d52eda44",
                        "_props": {
                            "filters": [
                                {
                                    "Filter": [
                                        "5fc382a9d724d907e2077dab",
                                        "5fc275cf85fd526b824a571a",
                                        "5fc382c1016cce60e8341b20",
                                        "5fc382b6d6fa9c00c571bbc3",
                                        "68561ab84857b945e0ce85e9",
                                        "6859749fb61f50b920ae5a2a"
                                    ]
                                }
                            ]
                        },
                        "_required": false,
                        "_mergeSlotWithChildren": false,
                        "_proto": "55d4af244bdc2d962f8b4571",
                    }
                ];
            } //Adding .338 LM RIP to the AXMC5c793fc42e221600114ca25d
            if (fileData._id === "616584766ef05c2ce828ef57") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("6857c3a0b4fec9f18e5e5e36", "685912a257837068f1460c7a");
            } //Pushing ROMEO7 to QRP2 Mount
            if (fileData._id === "5c7d55f52e221644f31bff6a") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("6857c3a0b4fec9f18e5e5e36", "685912a257837068f1460c7a");
            } //Pushing ROMEO7 to LRP Mount
            if (fileData._id === "5c7d560b2e22160bc12c6139") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("6857c3a0b4fec9f18e5e5e36", "685912a257837068f1460c7a");
            } //Pushing ROMEO7 to Aimpoint Standard Spacer
            if (fileData._id === "5c793fc42e221600114ca25d") {
                fileData._props.Accuracy = 0;
            } //Nerfing the Anodized Red SI ARE Buffer - Accuracy Stat
            if (fileData._id === "5c793fc42e221600114ca25d") {
                fileData._props.Velocity = 0;
            } //Nerfing the Anodized Red SI ARE Buffer - Velocity Stat
            if (fileData._id === "5fc3e272f8b6a877a729eac5") {
                fileData._props.Slots[2]._props.filters[0].Filter.push("686d787fe69d3f3f9e3f5396");
            } //Pushing HK UMP Buffertube to HK UMP45
            if (fileData._id === "5fbbfabed5cb881a7363194e") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("6875f48210e047d487e43052");
            } //Manually pushing A-TEC flash hider to MCX barrels
            if (fileData._id === "5fbbfacda56d053a3543f799") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("6875f48210e047d487e43052");
            } //Manually pushing A-TEC flash hider to MCX barrels
            if (fileData._id === "5fb65363d1409e5ca04b54f5") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("686ad244771ad06c4fd8c970", "686ad2e1cb1cd4ac811bfe81", "686d61367b65e4565ddeb659", "686d613f44fd735790193ee4");
            } //Manually pushing Suppressors to .45 ACP Vector Barrels
            if (fileData._id === "5fb653962b1b027b1f50bd03") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("686ad244771ad06c4fd8c970", "686ad2e1cb1cd4ac811bfe81", "686d61367b65e4565ddeb659", "686d613f44fd735790193ee4");
            } //Manually pushing Suppressors to .45 ACP Vector Barrels
            if (fileData._id === "55d459824bdc2d892f8b4573") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("5ba26b01d4351e0085325a51", "5dfa3d950dee1b22f862eae0", "5c17804b2e2216152006c02f", "5fb6567747ce63734e3fa1dc", "5bc09a30d4351e00367fb7c8", "5c18b90d2e2216152142466b", "5fc0fa362770a0045c59c677", "5894a73486f77426d259076c");
            } //Manually pushing Iron Sights to the KAC RIS short handguard, for use with the KAC 9.5" barrel.
            if (fileData._id === "2e681ada9836f6e030080191") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("6859411da7ac91f2e4088de4", "68922e39bacf39d24ca68ddf", "687e0ad6c6789ae4fd6855b9", "687e09aa7e70e1e285c59d4e", "68642e0200397ba2b9e4ea9a", "685d43c384852051ef64e4e8");
            } //Pushing my Optics to Epic's FAST FRT Riser
            if (fileData._id === "e1543def58291668bac8e2c8") {
                fileData._props.Slots[0]._props.filters[0].Filter.push("6859411da7ac91f2e4088de4", "68922e39bacf39d24ca68ddf", "687e0ad6c6789ae4fd6855b9", "687e09aa7e70e1e285c59d4e", "68642e0200397ba2b9e4ea9a", "685d43c384852051ef64e4e8");
            } //Pushing my Optics to Epic's FAST FRT Riser (FDE)
            if (fileData._id === "689ce8bfd4370ee552a641da") {
                fileData._props.Slots[5]._props.filters[0].Filter.push("689d099c264b71b2da14043f");
            } //Pushing SCAR-20S Ext. Rail to SCAR-20S upper
            if (fileData._id === "68b192a29352f61e8d8f8d6d") {
                fileData._props.Slots[1]._props.filters[0].Filter.push("687c0c34d7c15876af335f98", "687c0c225e00201a4d569c3b", "687c0c65a932f0bc6a5e89a4", "687d11609da5dc2dbb2b7d7e", "687d134da0544fe8d0e262d7");
            } //Pushing my uppers to the Modular AR platform
        }
    }
}
exports.epicItemClass = epicItemClass;
//# sourceMappingURL=EpicsEdits.js.map