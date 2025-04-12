using System.Collections.Generic;

namespace AutoSpecialistCommands
{
    public class SpecialistCommandDef
    {
        public string roleDefName;
        public string abilityDefName;
        public string label;
        public string description;
        public string texPathOn;
        public string texPathOff;

        public SpecialistCommandDef(string roleDefName, string abilityDefName, string label, string description, string texPathOn, string texPathOff)
        {
            this.roleDefName = roleDefName;
            this.abilityDefName = abilityDefName;
            this.label = label;
            this.description = description;
            this.texPathOn = texPathOn;
            this.texPathOff = texPathOff;
        }
    }

    public static class AutoSpecialistCommandDefs
    {
        public static readonly List<SpecialistCommandDef> Supported = new()
        {
            new SpecialistCommandDef(
                roleDefName: "IdeoRole_ProductionSpecialist",
                abilityDefName: "ProductionCommand",
                label: "Auto Production Cmd",
                description: "Automatically re-cast the Production Command when it's off cooldown.",
                texPathOn: "UI/Commands/AutoProductionOn",
                texPathOff: "UI/Commands/AutoProductionOff"
            ),
            new SpecialistCommandDef(
                roleDefName: "IdeoRole_PlantSpecialist",
                abilityDefName: "FarmingCommand",
                label: "Auto Farming Cmd",
                description: "Automatically re-cast the Farming Command when it's off cooldown.",
                texPathOn: "UI/Commands/AutoFarmOn",
                texPathOff: "UI/Commands/AutoFarmOff"
            ),
            new SpecialistCommandDef(
                roleDefName: "IdeoRole_MiningSpecialist",
                abilityDefName: "MiningCommand",
                label: "Auto Mining Cmd",
                description: "Automatically re-cast the Mining Command when it's off cooldown.",
                texPathOn: "UI/Commands/AutoMineOn",
                texPathOff: "UI/Commands/AutoMineOff"
            ),
            new SpecialistCommandDef(
                roleDefName: "IdeoRole_ResearchSpecialist",
                abilityDefName: "ResearchCommand",
                label: "Auto Research Cmd",
                description: "Automatically re-cast the Research Command when it's off cooldown.",
                texPathOn: "UI/Commands/AutoResearchOn",
                texPathOff: "UI/Commands/AutoResearchOff"
            )
        };
    }
}
