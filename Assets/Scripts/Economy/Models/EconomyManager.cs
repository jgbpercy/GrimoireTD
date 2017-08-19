using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;
using GrimoireTD.Map;

namespace GrimoireTD.Economy
{
    public class EconomyManager : SingletonMonobehaviour<EconomyManager>
    {
        [SerializeField]
        private SoResourceTemplate[] resourceTemplates;

        private List<IResource> resources;

        private Dictionary<IResourceTemplate, IResource> resourceTemplateToResourceDict;

        public IReadOnlyCollection<IReadOnlyResource> Resources
        {
            get
            {
                return resources;
            }
        }

        //TODO: this is bit hacky/slow? remove?
        public IEconomyTransaction ResourcesAsTransaction
        {
            get
            {
                var resourceTransactions = resources.Select(x => new CResourceTransaction(x, x.AmountOwned));

                return new CEconomyTransaction(resourceTransactions);
            }
        }

        private Action<IResource, int, int> OnAnyResourceChangedCallback;

        private void Awake()
        {
            resources = new List<IResource>();
            resourceTemplateToResourceDict = new Dictionary<IResourceTemplate, IResource>();

            foreach (IResourceTemplate resourceTemplate in resourceTemplates)
            {
                IResource newResource = new CResource(resourceTemplate);

                resources.Add(newResource);
                resourceTemplateToResourceDict.Add(resourceTemplate, newResource);

                newResource.RegisterForOnResourceChangedCallback((int byAmount, int newAmount) => OnResourceChanged(newResource, byAmount, newAmount));
            }
        }

        private void Start()
        {
            CDebug.Log(CDebug.applicationLoading, "Econonmy Manager Start");

            MapGenerator.Instance.Level.StartingResources.DoTransaction();
        }

        public IResource GetResourceFromTemplate(IResourceTemplate resourceTemplate)
        {
            return resourceTemplateToResourceDict[resourceTemplate];
        }

        private void OnResourceChanged(IResource resource, int byAmount, int newAmount)
        {
            OnAnyResourceChangedCallback?.Invoke(resource, byAmount, newAmount);
        }

        public void RegisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback)
        {
            OnAnyResourceChangedCallback += callback;
        }

        public void DeregisterForOnAnyResourceChangedCallback(Action<IResource, int, int> callback)
        {
            OnAnyResourceChangedCallback -= callback;
        }
    }
}