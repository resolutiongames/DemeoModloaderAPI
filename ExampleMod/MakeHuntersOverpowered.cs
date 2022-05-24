using Boardgame.BoardEntities.Abilities;
using Boardgame.Modding;
using DataKeys;

namespace ExampleMod
{
    public class MakeHuntersOverpowered : DemeoMod
    {
        public override ModdingAPI.ModInformation ModInformation => new ModdingAPI.ModInformation()
        {
            name = "Make Hunters Overpowered",
            author = "Resolution",
            version = "0.1",
            description = "Sets the damage of Hunter Arrow to 99",
            isNetworkCompatible = false,
        };

        public override void Load()
        {
            Ability arrow = AbilityFactory.GetAbility(AbilityKey.Arrow);
            arrow.abilityDamage.targetDamage = 99;
            arrow.abilityDamage.critDamage = 99;
        }
    }
}