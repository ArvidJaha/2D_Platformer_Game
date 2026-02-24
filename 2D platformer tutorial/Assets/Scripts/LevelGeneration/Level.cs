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
    private HashSet<Room> path;
    private Room entrance;
    private Room exit;

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
        path = new HashSet<Room>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
                rooms[GetRoomID(x, y)] = new Room(GetRoomID(x, y), x, y);
        }
    }

    //Room path generation algorithm
    private void GenerateRoomPath()
    {
        int start = Random.Range(0, height);
        int x = 0, prevX = 0;
        int y = start, prevY = start;

        rooms[GetRoomID(x, y)].Type = 1;
        entrance = rooms[GetRoomID(x, y)];

        while (x < width - 1)
        {
            prevX = x;
            prevY = y;
            switch (RandomDirection())
            {
                case Direction.RIGHT:
                    if (x < width - 1 && rooms[GetRoomID(x + 1, y)].Type == 0)
                    {
                        x++;
                        rooms[GetRoomID(x, y)].Type = 1;
                    }
                    
                    break;

                case Direction.UP:
                    if (y > 0 && rooms[GetRoomID(x, y - 1)].Type == 0)
                    {
                        y--;
                        rooms[GetRoomID(x, y)].Type = 2;
                    }
                        
                    break;
                case Direction.DOWN:
                    if (y < height - 1 && rooms[GetRoomID(x, y + 1)].Type == 0)
                    {
                        y++;
                        rooms[GetRoomID(x, y)].Type = 3;
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
        int choice = Mathf.FloorToInt(Random.value * 4.99f);
        switch (choice)
        {
            case 0: case 1: return Direction.RIGHT;  // 40% right
            case 2: return Direction.UP;              // 20% up
            case 3: return Direction.DOWN;            // 20% down
            default: return Direction.RIGHT;
        }
    }

    private int GetRoomID(int x, int y)
    {
        return y * width + x;
    }
}