using System;
using System.Collections.Generic;

namespace GrimoireTD.DefendingEntities.DefenderEffects
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

        private IDefenderAuraTemplate defenderAuraTemplate;

        private IDefendingEntity sourceDefendingEntity;

        private Action<IDefenderAura> OnClearAuraCallback;

        public IDefenderAuraTemplate DefenderAuraTemplate
        {
            get
            {
                return defenderAuraTemplate;
            }
        }

        public IDefendingEntity SourceDefendingEntity
        {
            get
            {
                return sourceDefendingEntity;
            }
        }

        public int Range
        {
            get
            {
                return defenderAuraTemplate.BaseRange;
            }
        }

        public CDefenderAura(IDefenderAuraTemplate defenderAuraTemplate, IDefendingEntity sourceDefendingEntity) : base(defenderAuraTemplate)
        {
            this.defenderAuraTemplate = defenderAuraTemplate;
            this.sourceDefendingEntity = sourceDefendingEntity;
        }

        public void ClearAura()
        {
            OnClearAuraCallback(this);
        }

        public void RegisterForOnClearAuraCallback(Action<IDefenderAura> callback)
        {
            OnClearAuraCallback += callback;
        }

        public void DeregisterForOnClearAuraCallback(Action<IDefenderAura> callback)
        {
            OnClearAuraCallback -= callback;
        }

        public override string UIText()
        {
            return defenderAuraTemplate.NameInGame;
        }
    }
}