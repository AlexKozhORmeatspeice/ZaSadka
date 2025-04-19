using Godot;
using System;
using ZaSadka;
using DI;
using Market;
using System.Collections.Generic;
using GameEvents;
using System.Runtime.Serialization;
using System.Reflection.Metadata;

//TODO: проверка на дебафф, у дистрикта есть айди, у юнита (нужно дополнительно достать из json) дебафф дистрикт айди, в случае этого добавить бонусов
//TODO: пихать карты зданий в конкретно слоты, а карты юнитов скорее на районы, чтоб они где-то в углу прицеплялись. ну либо отдельно слоты для них сделать

namespace District
{
	public interface IDistrict
	{
		void ChangeInfluence(int bonus);
		event Action<int> OnChangedInfluence;
		void ChangeSuspicion(int bonus);
		event Action<int> OnChangedSuspicion;
		void AddUnitChance(float chance);
		ParamType CheckForEvent();
	}
	public partial class District : Node2D, IDistrict, ILateStartable
	{
		[Export] private Vector2 size = new(400, 400);
		[Export] private ColorRect BG;
		[Export] private SlotCardSpawner slotCardSpawner;
		public event Action<int> OnChangedInfluence;
		public event Action<int> OnChangedSuspicion;
		private DistrictInfo info;
		private bool isActivated = false;
		[Inject] ISupplyDemandManager supplyAndDemandManager;
		[Inject] IObjectResolver objectResolver;
		[Export] private int g_maxSuspicion = 10;
		[Export] private int g_maxInfluence = 10;
		private List<float> unitChances = [];

		public override void _Ready()
		{
			BG.Size = size;
		}

        void ILateStartable.LateStart()
        {
            //TODO: get district data, fix wtf is wrong with createcards
			objectResolver.Inject(slotCardSpawner);
			slotCardSpawner.CreateCards(size, info.slotCount);
        }

        public void ChangeInfluence(int bonus)
        {
            info.info.influence += bonus;
			OnChangedInfluence?.Invoke(info.info.influence);
			if (!isActivated)
			{
				supplyAndDemandManager.OnDemandChange(info.info.demand, ItemType.District);
				supplyAndDemandManager.OnSupplyChange(info.info.supply, ItemType.District);
				isActivated = true;
			}

			if (info.info.influence >= g_maxInfluence)
			{
				//TODO: чёт дополнительно делаем или просто в checkforevent убираем бандитов?
			}
        }

        public void ChangeSuspicion(int bonus)
        {
            info.info.suspicion += bonus;
			OnChangedSuspicion?.Invoke(info.info.suspicion);

			if (info.info.suspicion >= g_maxSuspicion)
			{
				//TODO: чё мы там должны были делать
			}
        }

        public void AddUnitChance(float chance)
        {
            unitChances.Add(chance);
        }

        public ParamType CheckForEvent()
        {
            float copsProbability = (float)info.info.suspicion / g_maxSuspicion * .5f;
			float banditProbability = (float)info.info.influence / g_maxInfluence * .5f;
			foreach (float unitChance in unitChances)
			{
				copsProbability += (1 - copsProbability)*unitChance;
				banditProbability += (1 - banditProbability)*unitChance;
			}

			float chance = GD.Randf();
			List<float> chances = [chance, copsProbability, banditProbability];
			chances.Sort();
			
			//TODO: не самый лучший метод в общем-то
			if (chances[2] <= copsProbability)
			{
				return ParamType.Suspicion;
			}
			else if (chances[2] <= banditProbability)
			{
				return ParamType.Influence;
			}
			else
			{
				return ParamType.None;
			}
        }

    }

}
