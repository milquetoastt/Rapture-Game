using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation")]
    public DungeonRoom roomPrefab;
    public int roomCount = 12;

    public int maxAttempts = 200;

    [Header("Layout")]
    public Vector2 roomSpacing = new Vector2(10f, 6f);

    [Header("Hierarchy")]
    public Transform roomParent;

    public List<DungeonRoom> rooms = new List<DungeonRoom>();

    [Header("Room Prefabs")]
    public DungeonRoom startRoomPrefab;
    public DungeonRoom[] combatRoomPrefabs;
    public DungeonRoom[] treasureRoomPrefabs;
    public DungeonRoom[] bossRoomPrefabs;

    private void Start()
    {
        GenerateDungeon();
    }

    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        ClearDungeon();
        CreateRooms();
        CalculateDistances();
        AssignSpecialRooms();
        ReplaceRoomPrefabs();
    }

    [ContextMenu("Clear Dungeon")]
    public void ClearDungeon()
    {
        rooms.Clear();

        Transform parentToClear = roomParent != null ? roomParent : transform;

        for (int i = parentToClear.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parentToClear.GetChild(i).gameObject);
        }
    }

    private void CreateRooms()
    {
        Transform parentToUse = roomParent != null ? roomParent : transform;

        DungeonRoom firstRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, parentToUse);
        firstRoom.generator = this;
        firstRoom.xPos = 0;
        firstRoom.yPos = 0;
        firstRoom.firstRoom = true;
        firstRoom.roomType = RoomType.Start;
        firstRoom.UpdateWorldPosition();

        rooms.Add(firstRoom);

        int attempts = 0;

        while (rooms.Count < roomCount && attempts < maxAttempts)
        {
            attempts++;
            //Debug.Log("rooms.count " + rooms.Count);
            DungeonRoom baseRoom = rooms[Random.Range(0, rooms.Count)];

            DungeonRoom newRoom = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, parentToUse);
            newRoom.generator = this;

            bool success = baseRoom.AddRoomRandomly(newRoom);

            if (success)
            {
                newRoom.UpdateWorldPosition();
                rooms.Add(newRoom);
                //Debug.Log("Added room" + rooms.Count);
            }
            else
            {
                DestroyImmediate(newRoom.gameObject);
            }
        }
    }

    private void CalculateDistances()
    {
        foreach (DungeonRoom room in rooms)
        {
            room.distanceFromStart = -1;
        }
        DungeonRoom startRoom = GetStartRoom();
        if (startRoom == null) return;

        //using BFS thank god for CS classes
        Queue<DungeonRoom> queue = new Queue<DungeonRoom>();
        startRoom.distanceFromStart = 0;
        queue.Enqueue(startRoom);
        while (queue.Count > 0)
        {
            DungeonRoom current = queue.Dequeue();

            TryVisitNeighbor(current.upRoom, current.distanceFromStart, queue);
            TryVisitNeighbor(current.downRoom, current.distanceFromStart, queue);
            TryVisitNeighbor(current.leftRoom, current.distanceFromStart, queue);
            TryVisitNeighbor(current.rightRoom, current.distanceFromStart, queue);
        }
    }

    private void TryVisitNeighbor(DungeonRoom neighbor, int currentDistance, Queue<DungeonRoom> queue)
    {
        if (neighbor == null) return;
        if (neighbor.distanceFromStart != -1) return;

        neighbor.distanceFromStart = currentDistance + 1;
        queue.Enqueue(neighbor);
    }

    private void AssignSpecialRooms()
    {
        DungeonRoom startRoom = GetStartRoom();
        if (startRoom == null) return;

        foreach (DungeonRoom room in rooms)
        {
            room.roomType = RoomType.Combat;
        }

        startRoom.roomType = RoomType.Start;
        DungeonRoom farthestRoom = null;
        int farthestDistance = -1;

        foreach (DungeonRoom room in rooms)
        {
            if (room == startRoom) continue;

            if (room.distanceFromStart > farthestDistance)
            {
                farthestDistance = room.distanceFromStart;
                farthestRoom = room;
            }
        }

        if (farthestRoom != null)
        {
            //Debug.Log("BossRoom @ + "furthestDistance" + away)
            farthestRoom.roomType = RoomType.Boss;
        }

        DungeonRoom treasureRoom = null;
        int bestTreasureDistance = -1;

        foreach (DungeonRoom room in rooms)
        {
            if (room == startRoom) continue;
            if (room == farthestRoom) continue;
            if (!room.IsLeaf()) continue;

            if (room.distanceFromStart > bestTreasureDistance)
            {
                bestTreasureDistance = room.distanceFromStart;
                treasureRoom = room;
            }
        }

        if (treasureRoom != null)
        {
            treasureRoom.roomType = RoomType.Treasure;
        }
    }

    public DungeonRoom GetRoomAtPosition(int x, int y)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].xPos == x && rooms[i].yPos == y)
            {
                return rooms[i];
            }
        }

        return null;
    }

    public DungeonRoom GetStartRoom()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].firstRoom)
            {
                return rooms[i];
            }
        }

        return null;
    }

    private DungeonRoom[] GetPrefabPoolForType(RoomType type)
    {
        switch (type)
        {
            case RoomType.Treasure:
                return treasureRoomPrefabs;

            case RoomType.Boss:
                return bossRoomPrefabs;

            default:
                return combatRoomPrefabs;
        }
    }

    private DungeonRoom GetMatchingPrefab(DungeonRoom room)
    {
        if (room.roomType == RoomType.Start)
        {
            return startRoomPrefab;
        }

        DungeonRoom[] pool = GetPrefabPoolForType(room.roomType);

        bool needUp = room.upRoom != null;
        bool needDown = room.downRoom != null;
        bool needLeft = room.leftRoom != null;
        bool needRight = room.rightRoom != null;

        Debug.Log($"Selecting prefab for room at ({room.xPos}, {room.yPos}) type {room.roomType} needs U:{room.upRoom != null} D:{room.downRoom != null} L:{room.leftRoom != null} R:{room.rightRoom != null}");
        for (int i = 0; i < pool.Length; i++)
        {
            Debug.Log($"Checking prefab {pool[i].name} supports U:{pool[i].supportsUp} D:{pool[i].supportsDown} L:{pool[i].supportsLeft} R:{pool[i].supportsRight}");
            if (pool[i].MatchesRequiredExits(needUp, needDown, needLeft, needRight))
            {
                return pool[i];
            }
        }

        Debug.LogWarning("No matching prefab found for room at (" + room.xPos + ", " + room.yPos + ")");
        return null;
    }

    public void ReplaceRoomPrefabs()
    {
        Dictionary<DungeonRoom, DungeonRoom> roomMap = new Dictionary<DungeonRoom, DungeonRoom>();
        List<DungeonRoom> newRooms = new List<DungeonRoom>();

        foreach (DungeonRoom oldRoom in rooms)
        {
            DungeonRoom prefabToSpawn = GetMatchingPrefab(oldRoom);

            if (prefabToSpawn == null)
            {
                Debug.LogError($"No matching prefab for {oldRoom.roomType} at ({oldRoom.xPos}, {oldRoom.yPos})");
                return;
            }

            DungeonRoom newRoom = Instantiate(prefabToSpawn, oldRoom.transform.position, Quaternion.identity, roomParent);

            newRoom.generator = this;
            newRoom.xPos = oldRoom.xPos;
            newRoom.yPos = oldRoom.yPos;
            newRoom.roomType = oldRoom.roomType;
            newRoom.distanceFromStart = oldRoom.distanceFromStart;
            newRoom.firstRoom = oldRoom.firstRoom;

            roomMap.Add(oldRoom, newRoom);
            newRooms.Add(newRoom);
        }

        foreach (DungeonRoom oldRoom in rooms)
        {
            DungeonRoom newRoom = roomMap[oldRoom];


            //room.upRoom = GetRoomAtPosition(room.xPos, room.yPos + 1);
            //room.downRoom = GetRoomAtPosition(room.xPos, room.yPos - 1);
            //room.leftRoom = GetRoomAtPosition(room.xPos - 1, room.yPos);
            //room.rightRoom = GetRoomAtPosition(room.xPos + 1, room.yPos);
            // switch comments to make adjacency = connection
            newRoom.upRoom = oldRoom.upRoom != null ? roomMap[oldRoom.upRoom] : null;
            newRoom.downRoom = oldRoom.downRoom != null ? roomMap[oldRoom.downRoom] : null;
            newRoom.leftRoom = oldRoom.leftRoom != null ? roomMap[oldRoom.leftRoom] : null;
            newRoom.rightRoom = oldRoom.rightRoom != null ? roomMap[oldRoom.rightRoom] : null;
        }

        foreach (DungeonRoom oldRoom in rooms)
        {
            Destroy(oldRoom.gameObject);
        }

        rooms = newRooms;
    }
}