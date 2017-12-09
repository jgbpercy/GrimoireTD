using System;
using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Creeps
{
    public class CCreepManager : ICreepManager
    {
        private List<IWave> waveList;

        private int currentWaveIndex;

        private List<ICreep> creepList;

        private bool waveIsSpawning;

        private float waveTimeElapsed;
        private float timeSinceWaveStoppedSpawning;
        private float idleTimeToTrackAfterSpawnEnd;

        public event EventHandler<EAOnWaveOver> OnWaveOver;

        public event EventHandler<EAOnCreepSpawned> OnCreepSpawned;

        public IReadOnlyList<ICreep> CreepList
        {
            get
            {
                return creepList;
            }
        }

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
            DepsProv.TheGameModeManager.OnEnterDefendMode += StartNextWave;

            foreach (IWaveTemplate waveTemplate in waves)
            {
                waveList.Add(waveTemplate.GenerateWave());
            }

            this.idleTimeToTrackAfterSpawnEnd = idleTimeToTrackAfterSpawnEnd;

            DepsProv.TheModelObjectFrameUpdater().Register(ModelObjectFrameUpdate);
        }

        private void ModelObjectFrameUpdate(float deltaTime)
        {
            if (DepsProv.TheGameModeManager.CurrentGameMode == GameMode.BUILD)
            {
                return;
            }

            waveTimeElapsed += deltaTime;

            if (waveTimeElapsed >= waveList[currentWaveIndex].NextSpawnTime() && waveIsSpawning)
            {
                SpawnCreep(waveList[currentWaveIndex].DequeueNextCreep());
            }

            if (!waveIsSpawning)
            {
                timeSinceWaveStoppedSpawning += deltaTime;

                if(timeSinceWaveStoppedSpawning >= idleTimeToTrackAfterSpawnEnd && creepList.Count == 0)
                {
                    currentWaveIndex += 1;
                    OnWaveOver?.Invoke(this, new EAOnWaveOver());
                }
            } 
            else if (!waveList[currentWaveIndex].CreepsRemaining())
            {
                waveIsSpawning = false;
            }

            creepList.Sort((x, y) => x.DistanceFromEnd.CompareTo(y.DistanceFromEnd));
        }

        private void SpawnCreep(ICreepTemplate creepToSpawnTemplate)
        {
            //TODO unhardcode spawnposition at zero
            ICreep newCreep = creepToSpawnTemplate.GenerateCreep(Vector3.zero);

            creepList.Add(newCreep);

            newCreep.OnDied += (object sender, EventArgs args) => creepList.Remove(newCreep);

            OnCreepSpawned?.Invoke(this, new EAOnCreepSpawned(newCreep));
        }

        private void StartNextWave(object sender, EAOnEnterDefendMode args)
        {
            waveTimeElapsed = 0f;

            if (waveList.Count <= currentWaveIndex)
            {
                return;
            }

            waveIsSpawning = true;
        }
    }
}