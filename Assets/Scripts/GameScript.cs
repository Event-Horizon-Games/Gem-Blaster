using UnityEngine;
using System.Collections.Generic;
//https://learntodroid.com/how-to-make-a-simple-2d-android-game-with-unity/

public class GameScript : MonoBehaviour
{
    [SerializeField] public static GameScript instance;

    [SerializeField] private int rows, columns;
    private GameObject[,] gems;

    [SerializeField] private List<Sprite> gemSprites = new List<Sprite>();
    [SerializeField] private GameObject gem;

    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<GameScript>();
        Vector2 gemDimensions = gem.GetComponent<SpriteRenderer>().bounds.size;
        GenerateGrid(gemDimensions.x, gemDimensions.y);
    }

    private void GenerateGrid(float gemWidth, float gemHeight)
    {
        gems = new GameObject[columns, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                float xPosition = transform.position.x + (gemWidth * x);
                float yPosition = transform.position.y + (gemHeight * y);

                GameObject newGem = Instantiate(
                    gem,
                    new Vector3(xPosition, yPosition, 0),
                    gem.transform.rotation
                );

                gems[x, y] = newGem;

                newGem.transform.parent = transform;
                newGem.GetComponent<SpriteRenderer>().sprite = gemSprites[Random.Range(0, gemSprites.Count)];
            }
        }
    }

    private Sprite RandomSpriteExcluding(List<Sprite> sprites)
    {
        List<Sprite> possibleSprites = new List<Sprite>();
        for (int i = 0; i < gemSprites.Count; i++)
        {
            if (!sprites.Contains(gemSprites[i]))
            {
                possibleSprites.Add(gemSprites[i]);
            }
        }

        return possibleSprites[Random.Range(0, possibleSprites.Count)];
    }

    public List<Vector2Int> GetDroppableGems()
    {
        List<Vector2Int> droppableGems = new List<Vector2Int>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 1; y < rows; y++)
            {
                if (gems[x, y].GetComponent<SpriteRenderer>().sprite != null && gems[x, y - 1].GetComponent<SpriteRenderer>().sprite == null)
                {
                    droppableGems.Add(new Vector2Int(x, y));
                    while (y < (rows - 1))
                    {
                        y++;
                        droppableGems.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return droppableGems;
    }

    public List<Vector2Int> GetSpawnableGems()
    {
        List<Vector2Int> spawnableGems = new List<Vector2Int>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 1; y < rows; y++)
            {
                if (gems[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    spawnableGems.Add(new Vector2Int(x, y));
                }
            }
        }
        return spawnableGems;
    }

    public void DropGems()
    {
        List<Vector2Int> droppableGems = GetDroppableGems();
        while (droppableGems.Count > 0)
        {
            for (int i = 0; i < droppableGems.Count; i++)
            {
                Vector2Int gemCoords = droppableGems[i];
                gems[gemCoords.x, gemCoords.y - 1].GetComponent<SpriteRenderer>().sprite = gems[gemCoords.x, gemCoords.y].GetComponent<SpriteRenderer>().sprite;
                gems[gemCoords.x, gemCoords.y].GetComponent<SpriteRenderer>().sprite = null;
            }
            droppableGems = GetDroppableGems();
        }

        List<Vector2Int> spawnableGems = GetSpawnableGems();
        for (int i = 0; i < spawnableGems.Count; i++)
        {
            Vector2Int sg = spawnableGems[i];

            List<Sprite> invalidSprites = new List<Sprite>();
            if (sg.x > 0)
            {
                invalidSprites.Add(gems[sg.x - 1, sg.y].GetComponent<SpriteRenderer>().sprite);
            }
            if (sg.x < columns - 1)
            {
                invalidSprites.Add(gems[sg.x + 1, sg.y].GetComponent<SpriteRenderer>().sprite);
            }
            if (sg.y > 0)
            {
                invalidSprites.Add(gems[sg.x, sg.y - 1].GetComponent<SpriteRenderer>().sprite);
            }
            if (sg.y < rows - 1)
            {
                invalidSprites.Add(gems[sg.x, sg.y + 1].GetComponent<SpriteRenderer>().sprite);
            }

            gems[sg.x, sg.y].GetComponent<SpriteRenderer>().sprite = RandomSpriteExcluding(invalidSprites);
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gems[x, y].GetComponent<Gem>().ClearMatches();
            }
        }
    }
}
