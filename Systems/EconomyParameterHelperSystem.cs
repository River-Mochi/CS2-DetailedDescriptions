using System;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Unity.Entities;


namespace DetailedDescriptions.Systems
{
    public partial class EconomyParameterHelperSystem : GameSystemBase
    {
        public static EconomyParameterData? _economyParameterData;
        private EntityQuery query;

        protected override void OnCreate()
        {
            base.OnCreate();
            //_prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            
            query = GetEntityQuery(ComponentType.ReadOnly<EconomyParameterData>());

            InitializeEconomyParameterData();
        }
        
        private void InitializeEconomyParameterData()
        {
            try
            {
                _economyParameterData = query.GetSingleton<EconomyParameterData>();

                Mod.log.Info("EconomyParameterData initialized");
                Enabled = true;
            }
            catch (Exception e)
            {
                Enabled = false;
            }
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            InitializeEconomyParameterData();
        }

        protected override void OnUpdate()
        {
        }
    }
}
