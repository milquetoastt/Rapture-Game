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

    [Header("Exits")]
    public bool supportsUp;
    public bool supportsDown;
    public bool supportsLeft;
    public bool supportsRight;
    private void OnDrawGizmos()
    {
        // --- existing color logic ---
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

        Gizmos.DrawCube(transform.position, new Vector3(4f, 3f, 0.2f));

        // --- existing white connection lines ---
        Gizmos.color = Color.white;

        if (upRoom != null) Gizmos.DrawLine(transform.position, upRoom.transform.position);
        if (downRoom != null) Gizmos.DrawLine(transform.position, downRoom.transform.position);
        if (leftRoom != null) Gizmos.DrawLine(transform.position, leftRoom.transform.position);
        if (rightRoom != null) Gizmos.DrawLine(transform.position, rightRoom.transform.position);

        // =========================
        // NEW: REQUIRED EXITS DEBUG
        // =========================

        float offset = 2.5f;

        // Use GREEN for "this exit is required"
        Gizmos.color = Color.green;

        if (upRoom != null)
            Gizmos.DrawSphere(transform.position + Vector3.up * offset, 0.4f);

        if (downRoom != null)
            Gizmos.DrawSphere(transform.position + Vector3.down * offset, 0.4f);

        if (leftRoom != null)
            Gizmos.DrawSphere(transform.position + Vector3.left * offset, 0.4f);

        if (rightRoom != null)
            Gizmos.DrawSphere(transform.position + Vector3.right * offset, 0.4f);

        // =========================
        // OPTIONAL: PREFAB SUPPORT DEBUG
        // =========================

        float innerOffset = 1.5f;

        // Use BLUE for "this prefab supports this exit"
        Gizmos.color = Color.cyan;

        if (supportsUp)
            Gizmos.DrawCube(transform.position + Vector3.up * innerOffset, Vector3.one * 0.3f);

        if (supportsDown)
            Gizmos.DrawCube(transform.position + Vector3.down * innerOffset, Vector3.one * 0.3f);

        if (supportsLeft)
            Gizmos.DrawCube(transform.position + Vector3.left * innerOffset, Vector3.one * 0.3f);

        if (supportsRight)
            Gizmos.DrawCube(transform.position + Vector3.right * innerOffset, Vector3.one * 0.3f);
    }

    public int ConnectionCount()
    {
        int count = 0;

        if (upRoom != null) count++;
        if (downRoom != null) count++;
        if (leftRoom != null) count++;
        if (rightRoom != null) count++;

        return count;
    }

    public bool IsLeaf()
    {
        return ConnectionCount() == 1;
    }

    public bool AddRoomRandomly(DungeonRoom newRoom)
    {
        int randomDirection = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            int dir = (randomDirection + i) % 4;

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
        }

        return false;
    }

    public bool MatchesRequiredExits(bool needUp, bool needDown, bool needLeft, bool needRight)
    {
        if (supportsUp != needUp) return false;
        if (supportsDown != needDown) return false;
        if (supportsLeft != needLeft) return false;
        if (supportsRight != needRight) return false;

        return true;
    }

    public void UpdateWorldPosition()
    {
        transform.position = new Vector3(
            xPos * generator.roomSpacing.x,
            yPos * generator.roomSpacing.y,
            0f
        );
    }
}