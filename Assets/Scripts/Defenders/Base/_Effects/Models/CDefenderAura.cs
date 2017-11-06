using System;
using System.Collections.Generic;

namespace GrimoireTD.Defenders.DefenderEffects
{
    public class CDefenderAura : CDefenderEffect, IDefenderAura
    {
        public class TemplateEqualityComparer : IEqualityComparer<IDefenderAura>
        {
            public bool Equals(IDefenderAura x, IDefenderAura y)
            {
                return x.DefenderAuraTemplate == y.DefenderAuraTemplate;
            }

            public int GetHashCode(IDefenderAura obj)
            {
                return obj.DefenderAuraTemplate.GetHashCode();
            }
        }

        public event EventHandler<EAOnClearDefenderAura> OnClearDefenderAura;

        public IDefenderAuraTemplate DefenderAuraTemplate { get; }

        public IDefender SourceDefender { get; }

        public int Range
        {
            get
            {
                return DefenderAuraTemplate.BaseRange;
            }
        }

        public CDefenderAura(IDefenderAuraTemplate defenderAuraTemplate, IDefender sourceDefender) : base(defenderAuraTemplate)
        {
            DefenderAuraTemplate = defenderAuraTemplate;
            SourceDefender = sourceDefender;
        }

        public void ClearAura()
        {
            OnClearDefenderAura?.Invoke(this, new EAOnClearDefenderAura(this));
        }

        public override string UIText()
        {
            return DefenderAuraTemplate.NameInGame;
        }
    }
}