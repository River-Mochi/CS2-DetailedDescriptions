using System.Collections.Generic;
using System.Linq;
using Colossal.Localization;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class ZoneLotSizeSystem : GameSystemBase
    {
        private PrefabSystem prefabSystem;
        private EntityQuery spawnableBuildings;
        private LocalizationManager _localizationManager;
        public static Dictionary<string, List<(int, int)>> zoneLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            spawnableBuildings = GetEntityQuery(new EntityQueryDesc()
            {
                All = [ComponentType.ReadWrite<SpawnableBuildingData>()]
            });
            _localizationManager = GameManager.instance.localizationManager;

            var allSpawnableBuildings = spawnableBuildings.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in allSpawnableBuildings)
            {
                prefabSystem.TryGetPrefab(entity, out PrefabBase bldgPrefab);
                BuildingPrefab buildingPrefab = (BuildingPrefab)bldgPrefab;
                if (bldgPrefab.TryGet(out SpawnableBuilding sbd) && buildingPrefab is not null)
                {
                    string zoneName = sbd.m_ZoneType?.GetPrefabID().ToString() ?? "";
                    if (!zoneLots.ContainsKey(zoneName))
                    {
                        zoneLots[zoneName] = new List<(int, int)>();
                    }
                    zoneLots[zoneName].Add((buildingPrefab.m_LotWidth, buildingPrefab.m_LotDepth));
                }
            }

            AddTextToDescriptions();
            _localizationManager.onActiveDictionaryChanged += OnActiveDictionaryChanged;
        }

        private void AddTextToDescriptions()
        {
            foreach (var item in zoneLots)
            {
                string zoneName = item.Key.Replace("ZonePrefab:","");
                if (zoneName == "") continue;
                
                var sortedLots = item.Value
                    .OrderBy(lot => lot.Item1)
                    .ThenBy(lot => lot.Item2)
                    .Select(lot => $"{lot.Item1}x{lot.Item2}")
                    .Distinct()
                    .ToList();

                string lotSize = sortedLots.Count > 1
                    ? string.Join(", ", sortedLots.Take(sortedLots.Count - 1)) + " " + LocalizationProvider.GetLocalizedAnd(_localizationManager.activeLocaleId) + " " + sortedLots.Last()
                    : sortedLots.FirstOrDefault() ?? "";

                var old = _localizationManager.activeDictionary.entries.FirstOrDefault(c => c.Key == string.Format("Assets.DESCRIPTION[{0}]", zoneName));
                if (string.IsNullOrEmpty(old.Value)) continue;
                string localizedText = LocalizationProvider.GetLocalizedText(_localizationManager.activeLocaleId).Replace("%data%", lotSize);
                string zoneDescNew = $"{old.Value}\r\n{localizedText}";
                if (old.Value.Contains(localizedText)) continue;
                _localizationManager.activeDictionary.Add(string.Format("Assets.DESCRIPTION[{0}]", zoneName), zoneDescNew);

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
