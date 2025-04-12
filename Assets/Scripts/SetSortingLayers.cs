using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayers : MonoBehaviour
{
    [SerializeField] GameObject[] players;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Update()
    {
        
        List<GameObject> playgroundObjects = GetPlaygroundObjects();
        List<GameObject> leftPlayerBody = playgroundObjects.FindAll(obj => obj.CompareTag("LeftPlayerBody"));
        SortObjectsByYPosition(playgroundObjects);
        ProcessBombLayers(playgroundObjects);
    }
    // Update is called once per frame
    private void Update_()
    {
        List<GameObject> playgroundObjects = GetPlaygroundObjects();
        SortObjectsByYPosition(playgroundObjects);
        ProcessBombLayers(playgroundObjects);
    }
    
    private List<GameObject> GetPlaygroundObjects()
    {
        List<GameObject> playgroundObjects = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingLayerName == "Playground")
            {
                playgroundObjects.Add(obj);
            }
        }
        return playgroundObjects;
    }

    private void SortObjectsByYPosition(List<GameObject> playgroundObjects)
    {
        playgroundObjects.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
        for (int i = 0; i < playgroundObjects.Count; i++)
        {
            SpriteRenderer renderer = playgroundObjects[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = i;
            }
        }
    }

    private void ProcessBombLayers(List<GameObject> playgroundObjects)
    {
        int baseOrder = 0;
        int skip = ProcessPlayerBombs(ref baseOrder);
        int bombBodySkip = ProcessBombBodies(playgroundObjects);
        AdjustRemainingLayers(playgroundObjects, baseOrder, skip, bombBodySkip);
    }

    private int ProcessPlayerBombs(ref int baseOrder)
    {
        int skip = 0;
        foreach (GameObject player in players)
        {
            PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (playerMovement != null && playerMovement.mBomb != null)
            {
                SpriteRenderer[] bombRenderer = playerMovement.mBomb.GetComponentsInChildren<SpriteRenderer>();
                SpriteRenderer playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
                baseOrder = playerRenderer.sortingOrder;
                foreach (SpriteRenderer renderer in bombRenderer)
                {
                    if (renderer.sortingLayerName == "Playground")
                    {
                        skip++;
                        renderer.sortingOrder = baseOrder + skip;
                    }
                }
            }
        }
        return skip;
    }

    private int ProcessBombBodies(List<GameObject> playgroundObjects)
    {
        int bombBodySkip = 0;
        foreach (GameObject obj in playgroundObjects)
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (obj.name == "BombBody")
            {
                int bombBodyOrder = renderer.sortingOrder;
                ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    ParticleSystemRenderer particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                    if (particleRenderer != null && particleRenderer.sortingLayerName == "Playground")
                    {
                        bombBodySkip++;
                        particleRenderer.sortingOrder = bombBodyOrder + bombBodySkip;
                    }
                }
            }
        }
        return bombBodySkip;
    }

    private void AdjustRemainingLayers(List<GameObject> playgroundObjects, int baseOrder, int skip, int bombBodySkip)
    {
        for (int i = baseOrder + 1; i < playgroundObjects.Count; i++)
        {
            SpriteRenderer renderer = playgroundObjects[i].GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.name != "Bomb" && renderer.name != "BombBody")
            {
                renderer.sortingOrder += skip + bombBodySkip;
            }
        }
    }
}
