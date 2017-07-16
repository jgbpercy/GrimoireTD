using System.Collections.Generic;

public interface IAoeProjectileTemplate : IProjectileTemplate {

    IEnumerable<AttackEffect> AoeAttackEffects { get; }

    float AoeRadius { get; }

    float AoeExpansionLerpFactor { get; }
}
