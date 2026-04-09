// Pol Lozano Llorens
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Code.Utils;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Size")]
    [Range(1, 32)]
    [SerializeField] private int levelHeight = 4;
    [Range(1, 32)]
    [SerializeField] private int levelWidth = 4;

    [SerializeField] private GameObject collectiblesParent;
    [SerializeField] private GameObject fishPrefab;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemiesParent;

    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private GameObject birdParent;

    [Header("Enemy Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float enemySpawnChance = 0.5f;
    [SerializeField] private int minEnemiesPerRoom = 0;

    //Keep track of level
    Level level;
    public Vector3 spawnPos;




    public enum TileID : uint
    {
        Ground,
        Wall,
        ENTRANCE,
        EXIT,
        Spike,
        ITEM,
        RANDOM,
        BACKGROUND,
        EMPTY,
        ENEMY,
        ICEBLOCK,
        BIRD
    }

    [Header("Tiles")]
    [SerializeField] TileBase[] tiles;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap doorTilemap;
    [SerializeField] private Tilemap itemTilemap;
    [SerializeField] private Tilemap spikeTilemap;
    [SerializeField] private Tilemap background;
    [SerializeField] private Tilemap wallTilemap;

    public Tilemap Tilemap { get => tilemap; }

    //Store room templates by their type
    [System.Serializable]
    public struct RoomTemplate
    {
        public Texture2D[] images;
        public int openings;
    }

    [Header("Room templates")] //0 random, 1 corridor, 2 drop from, 3 drop to
    [SerializeField] public RoomTemplate[] templates = new RoomTemplate[4];

    [Header("Color dictionary")]
    public Dictionary<Color32, TileID> byColor;

    void Awake()
    {
        //Store tiles by their color
        byColor = new Dictionary<Color32, TileID>()
        {
            [Color.black] = TileID.Ground,
            [Color.grey] = TileID.Ground,
            [Color.blue] = TileID.Wall,
            [Color.red] = TileID.Spike,
            [Color.green] = TileID.RANDOM,
            [Color.white] = TileID.EMPTY,
            [Color.clear] = TileID.EMPTY,
            [new Color32(255, 255, 0, 255)] = TileID.ENEMY, //YELLOW
            [new Color32(0, 0, 255, 255)] = TileID.BIRD, //BLUE
            [new Color32(255, 0, 0, 255)] = TileID.Spike, //RED
            [new Color32(203, 48, 48, 255)] = TileID.ICEBLOCK //DARK RED
        };


        GenerateLevel();
    }

    public bool doingSetup;

    public void GenerateLevel()
    {
        Debug.Log("GENERATING");
        doingSetup = true;
        var watch = System.Diagnostics.Stopwatch.StartNew();

        int attempts = 0;
        bool valid = false;

        while (!valid && attempts < 10)
        {
            attempts++;
            ClearTiles();
            GenerateBorder();
            level = new Level(levelWidth, levelHeight);
            FillBackground();
            level.Generate();

            if (ValidateLevel())
            {
                valid = true;
                Debug.Log($"Level valid after {attempts} attempt(s)");
            }
            else
            {
                Debug.LogWarning($"Attempt {attempts} failed validation, regenerating...");
            }
        }

        if (!valid)
            Debug.LogError("Could not generate valid level after 10 attempts, check your templates and config");
        else
            BuildRooms();

        watch.Stop();
        Debug.Log("Generation Time: " + watch.ElapsedMilliseconds + "ms");
        doingSetup = false;
    }

    //public void GenerateLevel()
    //{
    //    Debug.Log("GENERATING");
    //    doingSetup = true;
    //    //Keep track of time it takes to generate levels
    //    var watch = System.Diagnostics.Stopwatch.StartNew();

    //    ClearTiles();
    //    //GenerateBorder();
    //    level = new Level(levelWidth, levelHeight);
    //    FillBackground();
    //    level.Generate();
    //    BuildRooms();



    //    //Stop timer and print time elapsed
    //    watch.Stop();
    //    var elapsedMs = watch.ElapsedMilliseconds;
    //    Debug.Log("Generation Time: " + elapsedMs + "ms");
    //    doingSetup = false;
    //}

    //Clears tilemaps and rooms
    private void ClearTiles()
    {
        tilemap.ClearAllTiles();
        spikeTilemap.ClearAllTiles();
        itemTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        background.ClearAllTiles();

        foreach (Transform child in enemiesParent.transform)
            Destroy(child.gameObject);

        foreach (Transform child in collectiblesParent.transform)
            Destroy(child.gameObject);

        foreach (Transform child in birdParent.transform)
            Destroy(child.gameObject);

    }
    private void FillBackground()
    {
        for (int x = 0; x < levelWidth * Config.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < levelHeight * Config.ROOM_HEIGHT; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[(uint)TileID.BACKGROUND]);
            }
        }
    }
    //Places a border around the rooms and a background
    private void GenerateBorder()
    {
        int width = levelWidth * Config.ROOM_WIDTH;
        int height = levelHeight * Config.ROOM_HEIGHT;
        int borderSize = 5;

        for (int x = -borderSize; x <= width + borderSize; x++)
        {
            for (int y = -borderSize; y <= height + borderSize; y++)
            {
                bool isBorder =
                    x < 0 || x >= width ||
                    y < 0 || y >= height;

                Vector3Int pos = new Vector3Int(x, y, 0);
                if (isBorder)
                    tilemap.SetTile(pos, tiles[(uint)TileID.BACKGROUND]);
                else
                    background.SetTile(pos, tiles[(uint)TileID.BACKGROUND]);
            }
        }
    }

    private void BuildRooms()
    {
        foreach (Room r in level.Rooms)
        {
            if (r.Type == 0) continue;
            //Debug.Log($"Room {r.Id} openings: {r.Openings}");

            int offsetX = r.X * Config.ROOM_WIDTH; //Left to right
            int offsetY = r.Y * Config.ROOM_HEIGHT; //Top to bottom


            var valid = System.Array.FindAll(templates, t =>
                t.images.Length > 0 && t.openings == r.Openings);

            if (valid.Length == 0)
            {
                Debug.LogWarning($"No template found for openings {r.Openings} on room {r.Id}, skipping");
                continue;
            }

            RoomTemplate chosen = valid[Random.Range(0, valid.Length)];
            //Color32[] colors = chosen.images[Random.Range(0, chosen.images.Length)].GetPixels32();

            int imgIndex = Random.Range(0, chosen.images.Length);
            Texture2D img = chosen.images[imgIndex];
            Color32[] colors = img.GetPixels32();
            if (colors.Length != Config.ROOM_WIDTH * Config.ROOM_HEIGHT)
            {
                Debug.LogError($"Skipping bad image in template openings {r.Openings}");
                continue;
            }

            List<Vector3Int> enemyPositions = new List<Vector3Int>();
            List<Vector3Int> spikePositions = new List<Vector3Int>(); // collect spikes for second pass


            for (int y = 0; y < Config.ROOM_HEIGHT; y++)
            {
                for (int x = 0; x < Config.ROOM_WIDTH; x++)
                {
                    Vector3Int pos = new Vector3Int(x + offsetX, y + offsetY, 0); //Set position for tile
                    //Try to parse
                    if (byColor.TryGetValue(colors[y * Config.ROOM_WIDTH + x], out TileID id))
                    {
                        r.tiles[y * Config.ROOM_WIDTH + x].pos = pos;
                        r.tiles[y * Config.ROOM_WIDTH + x].id = id;
                        //Skip empty tiles
                        if (id == TileID.EMPTY)
                        {
                            tilemap.SetTile(pos, null); // carve out the ground
                            background.SetTile(pos, null);
                            continue;
                        }
                        switch (id)
                        {
                            case TileID.RANDOM:
                                if (Random.value <= .25f)
                                    tilemap.SetTile(pos, tiles[(uint)id]);
                                else if (Random.value <= .25f)
                                    tilemap.SetTile(pos, tiles[(uint)TileID.Ground]);
                                break;
                            case TileID.Spike:
                                // Place tile now, rotation handled in second pass
                                tilemap.SetTile(pos, null);
                                background.SetTile(pos, null);
                                spikeTilemap.SetTile(pos, tiles[(uint)TileID.Spike]);
                                spikePositions.Add(pos);
                                break;
                            case TileID.Wall:
                                wallTilemap.SetTile(pos, tiles[(uint)id]);
                                break;
                            case TileID.ENEMY:
                                tilemap.SetTile(pos, null);
                                background.SetTile(pos, null);
                                enemyPositions.Add(pos);
                                r.hasEnemy = true;
                                break;
                            case TileID.BIRD:
                                tilemap.SetTile(pos, null);
                                background.SetTile(pos, null);
                                SpawnBird(pos);
                                r.hasEnemy = true;
                                break;
                            default:
                                tilemap.SetTile(pos, tiles[(uint)id]);
                                break;
                        }
                    }
                    else //Debug.LogError("Error parsing image!");
                    {
                        Color32 unknown = colors[y * Config.ROOM_WIDTH + x];
                        Debug.LogError($"Error parsing image! Unknown color: R={unknown.r} G={unknown.g} B={unknown.b} A={unknown.a} at x={x} y={y}");
                    }
                }
            }
            foreach (Vector3Int pos in spikePositions)
            {
                bool hasBelow = tilemap.GetTile(pos + Vector3Int.down) != null;
                bool hasAbove = tilemap.GetTile(pos + Vector3Int.up) != null;
                bool hasLeft = tilemap.GetTile(pos + Vector3Int.left) != null;
                bool hasRight = tilemap.GetTile(pos + Vector3Int.right) != null;

                Matrix4x4 matrix = Matrix4x4.identity;

                if (hasAbove) matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
                else if (hasBelow) matrix = Matrix4x4.identity;
                else if (hasLeft) matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
                else if (hasRight) matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));

                spikeTilemap.SetTransformMatrix(pos, matrix);
            }
            spikePositions.Clear();

            SpawnEnemiesInRoom(r, enemyPositions);
            enemyPositions.Clear();
            //Place items down
            PlaceItems(r);
            //Place entrance, exit and set spawn pos
            if (r == level.Entrance)
            {
                if (r.hasEnemy)
                {
                    Room safeRoom = FindSafeRoom();
                    spawnPos = tilemap.GetCellCenterWorld(PlaceEntrance(safeRoom));
                }
                else
                {
                    spawnPos = tilemap.GetCellCenterWorld(PlaceEntrance(r));
                }
            }
            else if (level.fishRooms.Contains(r)) PlaceFish(r);
        }
    }

    private Room FindSafeRoom()
    {
        List<Room> validRooms = new List<Room>();

        foreach (Room room in level.Rooms)
        {
            if (!room.hasEnemy)
                validRooms.Add(room);
        }

        if (validRooms.Count == 0)
        {
            Debug.LogWarning("No safe rooms found, using default entrance room");
            return level.Entrance;
        }

        return validRooms[Random.Range(0, validRooms.Count)];
    }

    private void SpawnEnemiesInRoom(Room r, List<Vector3Int> enemyPositions)
    {
        if (enemyPositions.Count == 0) return;

        // Guarantee at least this many enemies based on config
        int guaranteed = Mathf.FloorToInt(enemyPositions.Count * enemySpawnChance);
        guaranteed = Mathf.Max(guaranteed, minEnemiesPerRoom);
        guaranteed = Mathf.Min(guaranteed, enemyPositions.Count);

        // Shuffle positions so guaranteed ones are random
        Shuffle(enemyPositions);

        for (int i = 0; i < enemyPositions.Count; i++)
        {
            if (i < guaranteed || Random.value <= enemySpawnChance)
                SpawnEnemy(enemyPositions[i]);
        }
    }

    private void Shuffle(List<Vector3Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void SpawnEnemy(Vector3Int pos)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
        Instantiate(enemyPrefab, worldPos, Quaternion.identity, enemiesParent.transform);
    }

    private void SpawnBird(Vector3Int pos)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
        Instantiate(birdPrefab, worldPos, Quaternion.identity, birdParent.transform);
    }

    //Place item in a room depending on surrounding walls 
    public void PlaceItems(Room r)
    {
        foreach (Room.Tile t in r.tiles)
        {
            var pos = t.pos;
            if (tilemap.GetTile(pos) == null && tilemap.GetTile(pos + Vector3Int.down) != null)
            {
                if (CheckWallsAroundTile(pos, tilemap) > 2 && Random.value < .5f)
                    itemTilemap.SetTile(pos, tiles[(uint)TileID.ITEM]);
                else if (Random.value < .2f)
                    itemTilemap.SetTile(pos, tiles[(uint)TileID.ITEM]);
            }
        }
    }

    //Check moore neighbourhood on a tile in a tilemap
    int CheckWallsAroundTile(Vector3Int pos, Tilemap tilemap)
    {
        int wallsAroundTile = 0;
        for (int checkX = -1; checkX <= 1; checkX++)
        {
            for (int checkY = -1; checkY <= 1; checkY++)
            {
                if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0)) continue; //skip center and corners
                if (tilemap.GetTile(new Vector3Int(pos.x + checkX, pos.y + checkY, 0)) != null) wallsAroundTile++;
            }
        }
        return wallsAroundTile;
    }
    public Vector3Int PlaceEntrance(Room r)
    {
        Vector3Int pos = RandomDoorPosition(r);


        itemTilemap.SetTile(pos, tiles[(uint)TileID.ENTRANCE]);
        return pos;
    }

    public void PlaceFish(Room r)
    {
        Vector3 pos = RandomDoorPosition(r);
        Vector3 fishPosOffset = new Vector3(0.5f, 0.5f, 0f);
        //doorTilemap.SetTile(pos, tiles[(uint)TileID.EXIT]);
        //exitPos = tilemap.GetCellCenterWorld(pos);

        Instantiate(fishPrefab, pos + fishPosOffset, Quaternion.identity, collectiblesParent.transform);
    }

    public Vector3Int RandomDoorPosition(Room r)
    {
        int minY = 1; // avoid bottom row touching border

        List<Vector3Int> availablePos = new List<Vector3Int>();
        foreach (Room.Tile t in r.tiles)
        {
            var pos = t.pos;
            if (pos.y < minY) continue; // skip bottom row
            if (tilemap.GetTile(pos) == null
                && tilemap.GetTile(pos + Vector3Int.down) != null
                && tilemap.GetTile(pos + Vector3Int.up) == null
                && spikeTilemap.GetTile(pos) == null)
                availablePos.Add(pos);
        }
        if (availablePos.Count == 0)
        {
            foreach (Room.Tile t in r.tiles)
            {
                if (t.pos.y < minY) continue;
                if (t.id != TileID.EMPTY && t.id != TileID.Ground && t.id != TileID.BACKGROUND)
                    availablePos.Add(t.pos);
            }
        }
        if (availablePos.Count == 0)
        {
            Debug.LogError($"RandomDoorPosition found no valid positions in room {r.Id}");
            return Vector3Int.zero;
        }

        return availablePos[Random.Range(0, availablePos.Count)];
    }

#if UNITY_EDITOR
    [Header("Gizmos")]
    public GUIStyle style;

    //Do not allow one room levels we need at least two rooms
    private void OnValidate()
    {
        levelWidth = (levelHeight == 1 && levelWidth < 2) ? 2 : levelWidth;
        levelHeight = (levelWidth == 1 && levelHeight < 2) ? 2 : levelHeight;
    }

    //Draw gizmos
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || doingSetup) return;
        DrawRooms();
        DrawPath();
    }

    void DrawRooms()
    {
        foreach (Room r in level.Rooms)
        {
            //Draw room ID and boundary
            Gizmos.color = new Color32(255, 253, 0, 128);
            Gizmos.DrawWireCube(r.Center(), new Vector2(Config.ROOM_WIDTH, Config.ROOM_HEIGHT));
            Handles.Label(r.Origin() + new Vector2(.5f, -.5f), r.Type.ToString(), style);

            if (r == level.Entrance) Gizmos.color = Color.green;
            else if (level.fishRooms.Contains(r)) Gizmos.color = Color.cyan;
            else continue;
            Gizmos.DrawWireCube(r.Center(), new Vector3(1, 1));
        }
    }

    void DrawPath()
    {
        Room previous = null;
        foreach (Room i in level.Path)
        {
            if (previous != null)
            {
                Handles.color = Color.white;
                Handles.DrawDottedLine(i.Center(), previous.Center(), 3);
                Handles.color = Color.magenta;
                Quaternion rot = Quaternion.LookRotation(i.Center() - previous.Center()).normalized;
                Handles.ConeHandleCap(0, (i.Center() + previous.Center()) / 2 + (previous.Center() - i.Center()).normalized, rot, 1f, EventType.Repaint);
            }
            previous = i;
        }
    }

    private bool ValidateLevel()
    {
        if (level.Entrance == null)
        {
            Debug.LogWarning("Validation failed: no entrance");
            return false;
        }
        if (level.fishRooms.Count != level.numFishes)
        {
            Debug.LogWarning("validation failed: no fish rooms");
            return false;
        }
        Debug.Log($"Path length: {level.Path.Count} rooms");
        return true;
    }

#endif
}

