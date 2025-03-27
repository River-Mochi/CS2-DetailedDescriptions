using System.Collections.Generic;
using System.Linq;
using Colossal.Entities;
using Colossal.Localization;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class BuildingLotSizeDisplaySystem : GameSystemBase
    {
        private PrefabSystem _prefabSystem;
        private EntityQuery _buildingsQuery;
        private LocalizationManager _localizationManager;
        private EntityManager _entityManager;
        private static readonly Dictionary<string, (int, int)> _buildingLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            _entityManager = World.EntityManager;
            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            _buildingsQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = [ComponentType.ReadWrite<BuildingData>()]
            });
            _localizationManager = GameManager.instance.localizationManager;

            var buildings = _buildingsQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in buildings)
            {
                if (_entityManager.TryGetComponent(entity, out BuildingData buildingData))
                {
                    string prefabName = _prefabSystem.GetPrefabName(entity);
                    _buildingLots.Add(prefabName, (buildingData.m_LotSize.x, buildingData.m_LotSize.y));
                }
            }

            AddTextToDescriptions();
            _localizationManager.onActiveDictionaryChanged += OnActiveDictionaryChanged;
            Mod.log.Info("ZoneLotSizeSystem initialized");
        }

        private void AddTextToDescriptions()
        {
            foreach (var item in _buildingLots)
            {
                if (_localizationManager.activeDictionary.TryGetValue($"Assets.DESCRIPTION[{item.Key}]", out var entry))
                {
                    if (string.IsNullOrEmpty(entry)) continue;
                    var lotText = "Lot Size: " + item.Value.Item1 + "x" + item.Value.Item2;
                    string newDescription = $"{entry}\r\n{lotText}";
                    if (entry.Contains(lotText)) continue;
                        _localizationManager.activeDictionary.Add($"Assets.DESCRIPTION[{item.Key}]", newDescription);
                }
            }
        }

        public void OnActiveDictionaryChanged()
        {
            AddTextToDescriptions();
        }

        protected override void OnUpdate()
        {
        }
    }
}
