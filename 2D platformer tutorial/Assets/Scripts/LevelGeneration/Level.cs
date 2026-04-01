using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public Level(int w, int h)
    {
        width = w;
        height = h;
    }

    private int width;
    private int height;

    private Room[] rooms;
    private Room[] firstRooms;
    private List<Room> path;
    private HashSet<Room> firstPath;

    private Room entrance;
    public HashSet<Room> fishRooms = new HashSet<Room>();
    public int numFishes = 5;

    private Vector3Int spawnPos;

    public Room[] Rooms { get => rooms; }
    public List<Room> Path { get => path; }
    public Room Entrance { get => entrance; }
    public Vector3Int SpawnPos { get => spawnPos; set => spawnPos = value; }

    public void Generate()
    {
        Initialize();
        GenerateRoomPath();
        CalculateOpenings();
    }

    private void Initialize()
    {
        rooms = new Room[width * height];
        firstRooms = new Room[width * height];
        path = new List<Room>();
        firstPath = new HashSet<Room>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int id = GetRoomID(x, y);
                rooms[id] = new Room(id, x, y);
                firstRooms[id] = new Room(id, x, y);
            }
        }
    }

    private bool HasMultipleNeighbours(int x, int y, int fromX, int fromY)
    {
        int neighbours = 0;
        if (x > 0 && rooms[GetRoomID(x - 1, y)].Type != 0 && !(x - 1 == fromX && y == fromY)) neighbours++;
        if (x < width - 1 && rooms[GetRoomID(x + 1, y)].Type != 0 && !(x + 1 == fromX && y == fromY)) neighbours++;
        if (y > 0 && rooms[GetRoomID(x, y - 1)].Type != 0 && !(x == fromX && y - 1 == fromY)) neighbours++;
        if (y < height - 1 && rooms[GetRoomID(x, y + 1)].Type != 0 && !(x == fromX && y + 1 == fromY)) neighbours++;
        return neighbours > 0;
    }

    private void CalculateOpenings()
    {
        foreach (Room r in path)
        {
            if (r.X > 0 && rooms[GetRoomID(r.X - 1, r.Y)].Type != 0) r.Openings |= 1; // left
            if (r.X < width - 1 && rooms[GetRoomID(r.X + 1, r.Y)].Type != 0) r.Openings |= 2; // right
            if (r.Y < height - 1 && rooms[GetRoomID(r.X, r.Y + 1)].Type != 0) r.Openings |= 4; // up
            if (r.Y > 0 && rooms[GetRoomID(r.X, r.Y - 1)].Type != 0) r.Openings |= 8; // down
        }
    }

    private void FirstPath()
    {
        int startY = Random.Range(0, height);
        int startX = Random.Range(0, width);
        int x = startX, prevX = startX;
        int y = startY, prevY = startY;

        rooms[GetRoomID(x, y)].Type = 1;
        firstRooms[GetRoomID(x, y)].Type = 1;
        entrance = rooms[GetRoomID(x, y)];
        path.Add(entrance);
        firstPath.Add(entrance);

        int steps = 0;
        int maxSteps = 12;
        int stuckCount = 0;

        while (steps < maxSteps)
        {
            prevX = x;
            prevY = y;
            int attempts = 0;
            bool moved = false;

            while (!moved && attempts < 20)
            {
                attempts++;
                switch (RandomDirection())
                {
                    case Direction.RIGHT:
                        if (x < width - 1
                            && rooms[GetRoomID(x + 1, y)].Type == 0
                            && !HasMultipleNeighbours(x + 1, y, x, y))
                        { x++; rooms[GetRoomID(x, y)].Type = 2; firstRooms[GetRoomID(x, y)].Type = 2; moved = true; }
                        break;
                    case Direction.UP:
                        if (y > 0
                            && rooms[GetRoomID(x, y - 1)].Type == 0
                            && !HasMultipleNeighbours(x, y - 1, x, y))
                        { y--; rooms[GetRoomID(x, y)].Type = 3; firstRooms[GetRoomID(x, y)].Type = 3; moved = true; }
                        break;
                    case Direction.DOWN:
                        if (y < height - 1
                            && rooms[GetRoomID(x, y + 1)].Type == 0
                            && !HasMultipleNeighbours(x, y + 1, x, y))
                        { y++; rooms[GetRoomID(x, y)].Type = 4; firstRooms[GetRoomID(x, y)].Type = 4; moved = true; }
                        break;
                    case Direction.LEFT:
                        if (x > 0
                            && rooms[GetRoomID(x - 1, y)].Type == 0
                            && !HasMultipleNeighbours(x - 1, y, x, y))
                        { x--; rooms[GetRoomID(x, y)].Type = 5; firstRooms[GetRoomID(x, y)].Type = 5; moved = true; }
                        break;
                }
            }

            if (!moved && steps >= MinSteps()) break;
            else if (!moved)
            {
                stuckCount++;
                if (stuckCount > 50) break; // end path if stuck
                continue;
            }
            stuckCount = 0;

            Room current = rooms[GetRoomID(x, y)];
            if (!path.Contains(current))
                path.Add(current);
            firstPath.Add(current);
            steps++;
        }

        fishRooms.Add(rooms[GetRoomID(x, y)]);
    }

    private void GenerateRoomPath()
    {
        FirstPath();

        int attempts = 0;
        while (fishRooms.Count < numFishes && attempts < 100)
        {
            attempts++;
            Room startRoom = new List<Room>(firstPath)[Random.Range(0, firstPath.Count)];
            int x = startRoom.X, prevX = startRoom.X;
            int y = startRoom.Y, prevY = startRoom.Y;
            int steps = 0;
            int maxSteps = 7;

            int stuckCount = 0;


            while (steps < maxSteps)
            {
                prevX = x;
                prevY = y;
                int moveAttempts = 0;
                bool moved = false;

                while (!moved && moveAttempts < 20)
                {
                    moveAttempts++;
                    switch (RandomDirection())
                    {
                        case Direction.RIGHT:
                            if (x < width - 1
                                && rooms[GetRoomID(x + 1, y)].Type == 0
                                && !HasMultipleNeighbours(x + 1, y, x, y))
                            { x++; rooms[GetRoomID(x, y)].Type = 2; moved = true; }
                            break;
                        case Direction.UP:
                            if (y > 0
                                && rooms[GetRoomID(x, y - 1)].Type == 0
                                && !HasMultipleNeighbours(x, y - 1, x, y))
                            { y--; rooms[GetRoomID(x, y)].Type = 3; moved = true; }
                            break;
                        case Direction.DOWN:
                            if (y < height - 1
                                && rooms[GetRoomID(x, y + 1)].Type == 0
                                && !HasMultipleNeighbours(x, y + 1, x, y))
                            { y++; rooms[GetRoomID(x, y)].Type = 4; moved = true; }
                            break;
                        case Direction.LEFT:
                            if (x > 0
                                && rooms[GetRoomID(x - 1, y)].Type == 0
                                && !HasMultipleNeighbours(x - 1, y, x, y))
                            { x--; rooms[GetRoomID(x, y)].Type = 5; moved = true; }
                            break;
                    }
                }

                if (!moved && steps >= MinSteps()) break;
                else if (!moved)
                {
                    stuckCount++;
                    if (stuckCount > 50) break; // end path if stuck
                    continue;
                }
                stuckCount = 0;

                Room current = rooms[GetRoomID(x, y)];
                if (!path.Contains(current))
                    path.Add(current);
                steps++;
            }

            Room endRoom = rooms[GetRoomID(x, y)];
            if (endRoom != startRoom && !fishRooms.Contains(endRoom))
                fishRooms.Add(endRoom);
        }
    }

    enum Direction { DOWN = 0, LEFT = 1, RIGHT = 2, UP = 3 };

    Direction RandomDirection()
    {
        int choice = Random.Range(0, 4);
        switch (choice)
        {
            case 0: return Direction.RIGHT;
            case 1: return Direction.UP;
            case 2: return Direction.DOWN;
            case 3: return Direction.LEFT;
            default: return Direction.RIGHT;
        }
    }
    private int MinSteps()
    {
        return (width + height) / 2;
    }

    private int GetRoomID(int x, int y)
    {
        return y * width + x;
    }
}