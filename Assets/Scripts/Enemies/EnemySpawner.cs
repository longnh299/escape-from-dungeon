using System.Collections;
using System.Collections.Generic;
using System.Linq;
using kcp2k;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    public AllEnemySO allEnemy;
    [HideInInspector] public int enemiesToSpawn;
    [HideInInspector] public int currentEnemyCount;
    [HideInInspector] public int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;
    [HideInInspector] public Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;
    NetPlayer player;

    private void OnEnable()
    {
        player = C_Data.Instance.player;
        // subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    // Process a change in room
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // Update music for room
        MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);

        // if the room is a corridor or the entrance then return
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        // if the room has already been defeated then return
        if (currentRoom.isClearedOfEnemies) return;

        // Get random number of enemies to spawn
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        // Get room enemy spawn parameters
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        // If no enemies to spawn return
        if (enemiesToSpawn == 0)
        {
            // Mark the room as cleared
            currentRoom.isClearedOfEnemies = true;

            return;
        }

        // Get concurrent number of enemies to spawn
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        // Update music for room
        MusicManager.Instance.PlayMusic(currentRoom.battleMusic, 0.2f, 0.5f);

        // Lock doors
        currentRoom.instantiatedRoom.LockDoors();

        // Spawn enemies
        SpawnEnemies();
    }

    // Spawn the enemies
    private void SpawnEnemies()
    {
        // Set gamestate engaging boss
        if (GameManager.Instance.gameState == GameState.bossStage)
        {
            GameManager.Instance.previousGameState = GameState.bossStage;
            GameManager.Instance.gameState = GameState.engagingBoss;
        }

        // Set gamestate engaging enemies
        else if(GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    // Spawn the enemies coroutine
    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        // Create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        // Check we have somewhere to spawn the enemies
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            // Loop through to create all the enemeies
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // wait until current enemy count is less than max concurrent enemies
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                // Create Enemy - Get next enemy type to spawn 
                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    // Get a random spawn interval between the minimum and maximum values
    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }

    // Get a random number of concurrent enemies between the minimum and maximum values
    private int GetConcurrentEnemies()
    {
        return Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies);
    }

    // Create an enemy in the specified position
    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        // keep track of the number of enemies spawned so far 
        // enemiesSpawnedSoFar++;

        // // Add one to the current enemy count - this is reduced when an enemy is destroyed
        // currentEnemyCount++;

        // // Get current dungeon level
        // DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // Instantiate enemy
        // GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        // // Initialize Enemy
        // enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        // // subscribe to enemy destroyed event
        // enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;

        //Fix
        int index = 0;
        foreach(var i in allEnemy.enemyList)
        {
            if(i == enemyDetails) break;

            index ++;
        }

        player.SpawnBot(index, position);

    }

    public void Subscribe_DestroyEvent(DestroyedEvent e)
    {
        e.OnDestroyed += Enemy_OnDestroyed;
    }

    public void OpenDoor(Room room)
    {
        room.isClearedOfEnemies = true;

        // Set game state
        if (GameManager.Instance.gameState == GameState.engagingEnemies)
        {
            GameManager.Instance.gameState = GameState.playingLevel;
            GameManager.Instance.previousGameState = GameState.engagingEnemies;
        }

        else if (GameManager.Instance.gameState == GameState.engagingBoss)
        {
            GameManager.Instance.gameState = GameState.bossStage;
            GameManager.Instance.previousGameState = GameState.engagingBoss;
        }

        // unlock doors
        room.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

        // Update music for room
        MusicManager.Instance.PlayMusic(room.ambientMusic, 0.2f, 2f);

        // Trigger room enemies defeated event
        StaticEventHandler.CallRoomEnemiesDefeatedEvent(room);
    }

    // Process enemy destroyed
    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        // Unsubscribe from event
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        // reduce current enemy count
        currentEnemyCount--;

        // Score points - call points scored event
        StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar >= enemiesToSpawn)
        {
            C_Data.Instance.character.OpenDoor(currentRoom.id);
        }
    }

}