using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    private static Color SELECTED_COLOR = new Color(.5f, .5f, .5f, 1.0f);
    private static Color UNSELECTED_COLOR = Color.white;

    private bool isSelected;
    private static Gem previousSelected = null;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isSelected = false;
    }

    private void Select()
    {
        isSelected = true;
        spriteRenderer.color = SELECTED_COLOR;
        previousSelected = gameObject.GetComponent<Gem>();
    }

    private void Unselect()
    {
        isSelected = false;
        spriteRenderer.color = UNSELECTED_COLOR;
        previousSelected = null;
    }

    private bool IsSelectedGemAdjacent()
    {
        Vector2[] adjacentDirections = new Vector2[] {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right
        };

        List<GameObject> adjacentGems = new List<GameObject>();

        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            RaycastHit2D collidedObject = Physics2D.Raycast(transform.position, adjacentDirections[i]);
            if (collidedObject.collider != null)
            {
                adjacentGems.Add(collidedObject.collider.gameObject);
            }
        }

        if (adjacentGems.Contains(previousSelected.gameObject))
        {
            return true;
        }
        return false;
    }

    public void SwapGem()
    {
        Sprite tempSprite = previousSelected.spriteRenderer.sprite;
        previousSelected.spriteRenderer.sprite = spriteRenderer.sprite;
        spriteRenderer.sprite = tempSprite;
    }

    private void OnMouseDown()
    {
        if (spriteRenderer.sprite == null)
        {
            return;
        }
        if (isSelected)
        {
            Unselect();
        }
        else
        {
            if (previousSelected == null)
            {
                Select();
            }
            else
            {
                if (IsSelectedGemAdjacent())
                {
                    SwapGem();
                    previousSelected.ClearMatches();
                    previousSelected.Unselect();
                    ClearMatches();
                }
                else
                {
                    previousSelected.GetComponent<Gem>().Unselect();
                    Select();
                }
            }
        }
    }

    private List<GameObject> FindHorizontalMatches()
    {
        List<GameObject> matchingGems = new List<GameObject>();

        Vector2[] horizontalDirections = new Vector2[] {
            Vector2.left, Vector2.right
        };

        for (int i = 0; i < horizontalDirections.Length; i++)
        {
            RaycastHit2D collidedObject = Physics2D.Raycast(transform.position, horizontalDirections[i]);
            while (collidedObject.collider != null && collidedObject.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite)
            {
                matchingGems.Add(collidedObject.collider.gameObject);
                collidedObject = Physics2D.Raycast(collidedObject.collider.transform.position, horizontalDirections[i]);
            }
        }

        return matchingGems;
    }

    private List<GameObject> FindVerticalMatches()
    {
        List<GameObject> matchingGems = new List<GameObject>();

        Vector2[] verticalDirections = new Vector2[] {
            Vector2.up, Vector2.down
        };

        for (int i = 0; i < verticalDirections.Length; i++)
        {
            RaycastHit2D collidedObject = Physics2D.Raycast(transform.position, verticalDirections[i]);
            while (collidedObject.collider != null && collidedObject.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite)
            {
                matchingGems.Add(collidedObject.collider.gameObject);
                collidedObject = Physics2D.Raycast(collidedObject.collider.transform.position, verticalDirections[i]);
            }
        }

        return matchingGems;
    }

    private void ClearMatches()
    {
        if (spriteRenderer.sprite == null)
        {
            return;
        }

        List<GameObject> horizontalMatches = FindHorizontalMatches();
        List<GameObject> verticalMatches = FindVerticalMatches();

        if (horizontalMatches.Count >= 2)
        {
            spriteRenderer.sprite = null;
            for (int i = 0; i < horizontalMatches.Count; i++)
            {
                horizontalMatches[i].GetComponent<SpriteRenderer>().sprite = null;
            }
        }

        if (verticalMatches.Count >= 2)
        {
            spriteRenderer.sprite = null;
            for (int i = 0; i < verticalMatches.Count; i++)
            {
                verticalMatches[i].GetComponent<SpriteRenderer>().sprite = null;
            }
        }

        if (horizontalMatches.Count >= 2 || verticalMatches.Count >= 2)
        {
            GameScript.instance.DropGems();
        }
    }
}