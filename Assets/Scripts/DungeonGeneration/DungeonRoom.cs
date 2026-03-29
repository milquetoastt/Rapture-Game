using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum RoomType
{
    Start,
    Combat,
    Treasure,
    Boss
}

public class DungeonRoom : MonoBehaviour
{
    public DungeonGenerator generator;

    public int xPos;
    public int yPos;

    public bool firstRoom = false;

    public DungeonRoom upRoom;
    public DungeonRoom downRoom;
    public DungeonRoom leftRoom;
    public DungeonRoom rightRoom;

    public RoomType roomType = RoomType.Combat;
    public int distanceFromStart = -1;

    [Header("Encounter")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public Portal[] portals;
    public bool encounterStarted = false;
    public bool roomCleared = false;
    private List<EnemyBase> aliveEnemies = new List<EnemyBase>();


    [Header("Exits")]
    public bool supportsUp;
    public bool supportsDown;
    public bool supportsLeft;
    public bool supportsRight;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("somthing");
        if (other.CompareTag("Player"))
        {
            Debug.Log("playerin here");
            PlayerEnteredRoom();
        }
    }
    public void PlayerEnteredRoom()
    {
        if (roomCleared)
            return;

        else if (!encounterStarted)
        {
            StartEncounter();
        }
    }

    private void StartEncounter()
    {
        Debug.Log("start encounter");
        encounterStarted = true;
        LockPortals();
        SpawnEnemies();
    }

    void EndEncounter()
    {
        roomCleared = true;
        UnlockPortals();
    }

    void LockPortals()
    {
        foreach (Portal portal in portals)
        {
            portal.Lock();
        }
    }

    void UnlockPortals()
    {
        foreach(Portal portal in portals)
        {
            portal.Unlock();
        }
    }

    void SpawnEnemies()
    {
        foreach (Transform place in spawnPoints)
        {
            Debug.Log("hereinspawnenemies");

            GameObject enemyOBJ = Instantiate(
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                place.position,
                Quaternion.identity
            );

            EnemyBase enemy = enemyOBJ.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.myRoom = this;
                aliveEnemies.Add(enemy);
            }
        }
    }

    public void EnemyDied(EnemyBase deadEnemy)
    {
        if (aliveEnemies.Contains(deadEnemy))
        {
            aliveEnemies.Remove(deadEnemy);
        }

        if (encounterStarted && aliveEnemies.Count == 0)
        {
            EndEncounter();
        }
    }

    private void OnDrawGizmos()
    {
        switch (roomType)
        {
            case RoomType.Start:
                Gizmos.color = Color.green;
                break;
            case RoomType.Boss:
                Gizmos.color = Color.red;
                break;
            case RoomType.Treasure:
                Gizmos.color = Color.yellow;
                break;
            default:
                Gizmos.color = Color.white;
                break;
        }

        Gizmos.DrawCube(transform.position, Vector3.one * 5);

        Gizmos.color = Color.white;
        if (upRoom != null) Gizmos.DrawLine(transform.position, upRoom.transform.position);
        if (downRoom != null) Gizmos.DrawLine(transform.position, downRoom.transform.position);
        if (leftRoom != null) Gizmos.DrawLine(transform.position, leftRoom.transform.position);
        if (rightRoom != null) Gizmos.DrawLine(transform.position, rightRoom.transform.position);
    }

    public void UpdateWorldPosition()
    {
        if (generator == null) return;

        transform.position = new Vector3(
            xPos * generator.roomSpacing.x,
            yPos * generator.roomSpacing.y,
            0f
        );
    }

    public bool MatchesRequiredExits(bool needUp, bool needDown, bool needLeft, bool needRight)
    {
        return
            supportsUp == needUp &&
            supportsDown == needDown &&
            supportsLeft == needLeft &&
            supportsRight == needRight;
    }

    private bool WouldMakeInvalidStartConnection(int dir)
    {
        if (generator == null) return false;

        DungeonRoom startRoom = generator.GetStartRoom();
        if (startRoom == null) return false;

        int targetX = xPos;
        int targetY = yPos;

        if (dir == 0) targetY += 1;
        else if (dir == 1) targetY -= 1;
        else if (dir == 2) targetX -= 1;
        else if (dir == 3) targetX += 1;

        DungeonRoom existingRoom = generator.GetRoomAtPosition(targetX, targetY);

        if (existingRoom == null) return false;

        if (existingRoom != startRoom) return false;

        bool isVerticalConnection = (dir == 0 || dir == 1);

        return isVerticalConnection;
    }

    public bool AddRoomRandomly(DungeonRoom newRoom)
    {
        if (firstRoom)
        {
            int firstDir = Random.Range(0, 2) == 0 ? 2 : 3;
            int secondDir = firstDir == 2 ? 3 : 2;

            if (TryAddRoomInDirection(newRoom, firstDir)) return true;
            if (TryAddRoomInDirection(newRoom, secondDir)) return true;

            return false;
        }

        int randomDirection = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            int dir = (randomDirection + i) % 4;

            if (TryAddRoomInDirection(newRoom, dir)) return true;
        }

        return false;
    }

    private bool TryAddRoomInDirection(DungeonRoom newRoom, int dir)
    {
        if (dir == 0)
        {
            if (upRoom == null && generator.GetRoomAtPosition(xPos, yPos + 1) == null)
            {
                upRoom = newRoom;
                newRoom.downRoom = this;
                newRoom.xPos = xPos;
                newRoom.yPos = yPos + 1;
                return true;
            }
        }
        else if (dir == 1)
        {
            if (downRoom == null && generator.GetRoomAtPosition(xPos, yPos - 1) == null)
            {
                downRoom = newRoom;
                newRoom.upRoom = this;
                newRoom.xPos = xPos;
                newRoom.yPos = yPos - 1;
                return true;
            }
        }
        else if (dir == 2)
        {
            if (leftRoom == null && generator.GetRoomAtPosition(xPos - 1, yPos) == null)
            {
                leftRoom = newRoom;
                newRoom.rightRoom = this;
                newRoom.xPos = xPos - 1;
                newRoom.yPos = yPos;
                return true;
            }
        }
        else if (dir == 3)
        {
            if (rightRoom == null && generator.GetRoomAtPosition(xPos + 1, yPos) == null)
            {
                rightRoom = newRoom;
                newRoom.leftRoom = this;
                newRoom.xPos = xPos + 1;
                newRoom.yPos = yPos;
                return true;
            }
        }

        return false;
    }
}