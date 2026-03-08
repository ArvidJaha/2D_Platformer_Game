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
    private HashSet<Room> path;
    private Room entrance;
    private Room exit;
    public HashSet<Room> exits = new HashSet<Room>();
    private int numExits = 5;

    private Vector3Int spawnPos;

    public Room[] Rooms { get => rooms; }
    public HashSet<Room> Path { get => path; }
    public Room Entrance { get => entrance; }
    public Room Exit { get => exit; }
    public Vector3Int SpawnPos { get => spawnPos; set => spawnPos = value; }

    public void Generate()
    {
        Initialize();
        GenerateRoomPath();
    }

    private void Initialize()
    {
        rooms = new Room[width * height];
        firstRooms = new Room[width * height];
        path = new HashSet<Room>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int id = GetRoomID(x, y);

                rooms[id] = new Room(id, x, y);
                firstRooms[id] = new Room(id, x, y); // ← missing line
            }
        }
    }

    //Room path generation algorithm
    private void FirstPath()
    {
        int startY = Random.Range(0, height);
        int startX = Random.Range(0, width);
        int x = startX, prevX = startX;
        int y = startY, prevY = startY;

        rooms[GetRoomID(x, y)].Type = 1;
        firstRooms[GetRoomID(x, y)].Type = 1;
        entrance = rooms[GetRoomID(x, y)];
        int steps = 0;
        int maxSteps = 15;

        while (steps < maxSteps)
        {
            steps++;
            prevX = x;
            prevY = y;
            switch (RandomDirection())
            {
                case Direction.RIGHT:
                    if (x < width - 1 && rooms[GetRoomID(x + 1, y)].Type == 0)
                    {
                        x++;
                        rooms[GetRoomID(x, y)].Type = 2;
                        firstRooms[GetRoomID(x, y)].Type = 2;
                    }
                    
                    break;

                case Direction.UP:
                    if (y > 0 && rooms[GetRoomID(x, y - 1)].Type == 0)
                    {
                        y--;
                        rooms[GetRoomID(x, y)].Type = 3;
                        firstRooms[GetRoomID(x, y)].Type = 3;
                    }
                        
                    break;
                case Direction.DOWN:
                    if (y < height - 1 && rooms[GetRoomID(x, y + 1)].Type == 0)
                    {
                        y++;
                        rooms[GetRoomID(x, y)].Type = 4;
                        firstRooms[GetRoomID(x, y)].Type = 4;
                    }
                        
                    break;
                case Direction.LEFT:
                    if (x > 0 && rooms[GetRoomID(x - 1, y)].Type == 0)
                    {
                        x--;
                        rooms[GetRoomID(x, y)].Type = 5;
                        firstRooms[GetRoomID(x, y)].Type = 5;
                    }
                    break;
            }
            //if (x == prevX && y == prevY) x++;

            //rooms[GetRoomID(x, y)].Type = 1;
            path.Add(rooms[GetRoomID(prevX, prevY)]);

            prevX = x;
            prevY = y;
        }

        exit = rooms[GetRoomID(x, y)];
        exits.Add(exit);
    }

    private void GenerateRoomPath()
    {
        FirstPath();

        for (int i = 0; i < numExits - 1; i++)
        {
            Room startRoom = new List<Room>(path)[Random.Range(0, path.Count)];

            int x = startRoom.X, prevX = startRoom.X;
            int y = startRoom.Y, prevY = startRoom.Y;

            int steps = 0;
            int maxSteps = 15;

            while (steps < maxSteps)
            {
                steps++;
                prevX = x;
                prevY = y;
                switch (RandomDirection())
                {
                    case Direction.RIGHT:
                        if (x < width - 1 && rooms[GetRoomID(x + 1, y)].Type == 0)
                        {
                            x++;
                            rooms[GetRoomID(x, y)].Type = 2;
                        }

                        break;

                    case Direction.UP:
                        if (y > 0 && rooms[GetRoomID(x, y - 1)].Type == 0)
                        {
                            y--;
                            rooms[GetRoomID(x, y)].Type = 3;
                        }

                        break;
                    case Direction.DOWN:
                        if (y < height - 1 && rooms[GetRoomID(x, y + 1)].Type == 0)
                        {
                            y++;
                            rooms[GetRoomID(x, y)].Type = 4;
                        }

                        break;
                    case Direction.LEFT:
                        if (x > 0 && rooms[GetRoomID(x - 1, y)].Type == 0)
                        {
                            x--;
                            rooms[GetRoomID(x, y)].Type = 5;
                        }
                        break;
                }
                //if (x == prevX && y == prevY) x++;

                //rooms[GetRoomID(x, y)].Type = 1;
                path.Add(rooms[GetRoomID(prevX, prevY)]);
                prevX = x;
                prevY = y;
            }

            exits.Add(rooms[GetRoomID(x, y)]);
        }
    }



    enum Direction
    {
        DOWN = 0,
        LEFT = 1,
        RIGHT = 2,
        UP = 3
    };

    //Pick random direction to go
    Direction RandomDirection()
    {
        int choice = Random.Range(0, 4); // 0,1,2
        switch (choice)
        {
            case 0: return Direction.RIGHT;  // 33% right
            case 1: return Direction.UP;              // 33% up
            case 2: return Direction.DOWN;
            case 3: return Direction.LEFT;// 33% down
            default: return Direction.RIGHT;
        }
    }

    private int GetRoomID(int x, int y)
    {
        return y * width + x;
    }
}