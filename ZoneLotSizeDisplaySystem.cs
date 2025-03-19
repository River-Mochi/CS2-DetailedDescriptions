using Colossal.Localization;
using Game.Prefabs;
using Game.SceneFlow;
using Game;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace ZoneLotSizeDisplay
{
    public partial class ZoneLotSizeSystem : GameSystemBase
    {
        private PrefabSystem prefabSystem;
        private EntityQuery spawnableBuildings;
        private LocalizationManager localizationManager;
        public static Dictionary<string, List<(int, int)>> zoneLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            localizationManager.onActiveDictionaryChanged += OnActiveDictionaryChanged;
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            spawnableBuildings = GetEntityQuery(new EntityQueryDesc()
            {
                All = [ComponentType.ReadWrite<SpawnableBuildingData>()]
            });
            localizationManager = GameManager.instance.localizationManager;

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

                string LotSize = sortedLots.Count > 1
                    ? string.Join(", ", sortedLots.Take(sortedLots.Count - 1)) + " and " + sortedLots.Last()
                    : sortedLots.FirstOrDefault() ?? "";

                var old = localizationManager.activeDictionary.entries.FirstOrDefault(c => c.Key == string.Format("Assets.DESCRIPTION[{0}]", zoneName));
                if (old.Value == "") continue;
                string ZoneDescNew = $"{old.Value}\r\nLot Sizes: {LotSize}";
                localizationManager.activeDictionary.Add(string.Format("Assets.DESCRIPTION[{0}]", zoneName), ZoneDescNew);

            }
        }

        public void OnActiveDictionaryChanged()
        {
            
        }

        protected override void OnUpdate()
        {
        }
    }
}
