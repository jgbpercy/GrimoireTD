﻿using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.Creeps
{
    public class CCreepManager : ICreepManager
    {
        private IReadOnlyGameStateManager gameStateManager;

        private List<IWave> waveList;

        private int currentWaveIndex;

        private List<ICreep> creepList;

        private bool waveIsSpawning;

        private float waveTimeElapsed;
        private float timeSinceWaveStoppedSpawning;
        private float idleTimeToTrackAfterSpawnEnd;

        private Action OnWaveIsOverCallback;

        private Action<ICreep> OnCreepSpawnedCallback;

        public CCreepManager()
        {
            creepList = new List<ICreep>();

            waveList = new List<IWave>();

            currentWaveIndex = 0;
            waveIsSpawning = false;

            waveTimeElapsed = 0f;
            timeSinceWaveStoppedSpawning = 0f;
        }

        public void SetUp(IEnumerable<IWaveTemplate> waves, float idleTimeToTrackAfterSpawnEnd)
        {
            gameStateManager = GameModels.Models[0].GameStateManager;

            gameStateManager.RegisterForOnEnterDefendModeCallback(StartNextWave);

            foreach (IWaveTemplate waveTemplate in waves)
            {
                waveList.Add(waveTemplate.GenerateWave());
            }

            this.idleTimeToTrackAfterSpawnEnd = idleTimeToTrackAfterSpawnEnd;
        }

        public void ModelObjectFrameUpdate()
        {
            if (gameStateManager.CurrentGameMode == GameMode.BUILD)
            {
                return;
            }

            waveTimeElapsed += Time.deltaTime;

            if (waveTimeElapsed >= waveList[currentWaveIndex].NextSpawnTime())
            {
                SpawnCreep(waveList[currentWaveIndex].DequeueNextCreep());
            }

            if (!waveIsSpawning)
            {
                timeSinceWaveStoppedSpawning += Time.deltaTime;

                if(timeSinceWaveStoppedSpawning >= idleTimeToTrackAfterSpawnEnd && creepList.Count == 0)
                {
                    OnWaveIsOverCallback?.Invoke();
                }
            } 

            else if (!waveList[currentWaveIndex].CreepsRemaining())
            {
                waveIsSpawning = false;
            }

            creepList.Sort((x, y) => x.DistanceFromEnd.CompareTo(y.DistanceFromEnd));
        }

        public IDefendModeTargetable CreepInRangeNearestToEnd(Vector3 fromPosition, float range)
        {
            float creepDistanceFromPosition;

            for (int i = 0; i < creepList.Count; i++)
            {
                creepDistanceFromPosition = Vector3.Magnitude(creepList[i].Position - fromPosition);
                if (creepDistanceFromPosition < range)
                {
                    return creepList[i];
                }
            }

            return null;
        }

        private void SpawnCreep(ICreepTemplate creepToSpawn)
        {
            CDebug.Log(CDebug.creepSpawning, "SpawnCreep called for " + creepToSpawn.NameInGame);

            //TODO unhardcode spawnposition at zero
            ICreep newCreep = creepToSpawn.GenerateCreep(Vector3.zero);

            creepList.Add(newCreep);

            newCreep.RegisterForOnDiedCallback(() => creepList.Remove(newCreep));

            OnCreepSpawnedCallback?.Invoke(newCreep);
        }

        private void StartNextWave()
        {
            CDebug.Log(CDebug.creepSpawning, "Start Next Wave called, currentWave = " + currentWaveIndex);

            waveTimeElapsed = 0f;

            if (waveList.Count <= currentWaveIndex)
            {
                return;
            }

            waveIsSpawning = true;
        }

        public void RegisterForOnWaveIsOverCallback(Action callback)
        {
            OnWaveIsOverCallback += callback;
        }

        public void DeregisterForOnWaveIsOverCallback(Action callback)
        {
            OnWaveIsOverCallback -= callback;
        }

        public void RegisterForCreepSpawnedCallback(Action<ICreep> callback)
        {
            OnCreepSpawnedCallback += callback;
        }

        public void DeregisterForCreepSpawnedCallback(Action<ICreep> callback)
        {
            OnCreepSpawnedCallback -= callback;
        }
    }
}