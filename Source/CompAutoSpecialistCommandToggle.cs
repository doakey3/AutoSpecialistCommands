using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AutoSpecialistCommands
{
    public class CompAutoSpecialistCommandToggle : ThingComp
    {
        private Dictionary<string, bool> autoEnabled = new();
        private string lastRoleDefName;
        private List<string> keysWorkingList;
        private List<bool> valuesWorkingList;

        public override void PostExposeData()
        {
            Scribe_Collections.Look(ref autoEnabled, "autoEnabled", LookMode.Value, LookMode.Value, ref keysWorkingList, ref valuesWorkingList);
            Scribe_Values.Look(ref lastRoleDefName, "lastRoleDefName");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent is not Pawn pawn || pawn.ideo?.Ideo == null)
                yield break;

            var role = pawn.ideo.Ideo.GetRole(pawn);
            if (role == null)
                yield break;

            foreach (var def in AutoSpecialistCommandDefs.Supported)
            {
                if (role.def.defName != def.roleDefName)
                    continue;

                bool isEnabled = autoEnabled.TryGetValue(def.abilityDefName, out var enabled) && enabled;

                yield return new Command_Toggle
                {
                    defaultLabel = def.label,
                    defaultDesc = def.description,
                    icon = ContentFinder<Texture2D>.Get(isEnabled ? def.texPathOn : def.texPathOff, reportFailure: false),
                    isActive = () => isEnabled,
                    toggleAction = () =>
                    {
                        autoEnabled[def.abilityDefName] = !isEnabled;
                        if (autoEnabled[def.abilityDefName])
                            TryImmediateCastIfReady(pawn, def.abilityDefName);
                    }
                };
            }
        }

        public override void CompTickRare()
        {
            if (parent is not Pawn pawn || pawn.DestroyedOrNull() || pawn.Dead || pawn.ideo?.Ideo == null)
                return;

            var role = pawn.ideo.Ideo.GetRole(pawn);

            if (role == null || role.def.defName != lastRoleDefName)
            {
                autoEnabled.Clear();
                lastRoleDefName = role?.def.defName;
                return;
            }

            if (pawn.InMentalState || pawn.Downed)
                return;

            foreach (var def in AutoSpecialistCommandDefs.Supported)
            {
                if (role.def.defName != def.roleDefName)
                    continue;

                if (autoEnabled.TryGetValue(def.abilityDefName, out bool enabled) && enabled)
                {
                    if (IsAbilityReady(pawn, role, def.abilityDefName))
                        TryImmediateCastIfReady(pawn, def.abilityDefName);
                }
            }
        }

        private bool IsAbilityReady(Pawn pawn, Precept_Role role, string abilityDefName)
        {
            List<Ability> abilities = role is Precept_RoleSingle single
                ? single.AbilitiesFor(pawn)
                : (role as Precept_RoleMulti)?.AbilitiesFor(pawn);

            if (abilities == null)
                return false;

            var ability = abilities.FirstOrDefault(a => a.def?.defName == abilityDefName);
            return ability != null && ability.CooldownTicksRemaining <= 0;
        }

        public void TryImmediateCastIfReady(Pawn pawn, string abilityDefName)
        {
            if (pawn.InMentalState || pawn.Downed)
                return;

            Precept_Role role = pawn.Ideo?.GetRole(pawn);
            if (role == null)
                return;

            List<Ability> abilities = role is Precept_RoleSingle single
                ? single.AbilitiesFor(pawn)
                : (role as Precept_RoleMulti)?.AbilitiesFor(pawn);

            if (abilities == null)
                return;

            var ability = abilities.FirstOrDefault(a => a.def?.defName == abilityDefName);
            if (ability == null || ability.CooldownTicksRemaining > 0)
                return;

            if (ability.verb is not Verb_CastAbility verbCast || !verbCast.Available())
                return;

            if (pawn.stances?.curStance is Stance_Warmup warm && warm.verb == verbCast)
                return;

            verbCast.TryStartCastOn(pawn, pawn,
                surpriseAttack: false,
                canHitNonTargetPawns: true,
                preventFriendlyFire: false,
                nonInterruptingSelfCast: false);
        }

        public bool IsAutoTriggerEnabled(string abilityDefName)
        {
            return autoEnabled.TryGetValue(abilityDefName, out bool enabled) && enabled;
        }
    }
}
