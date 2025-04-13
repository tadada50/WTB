using System;
using System.Collections.Generic;
using System.Linq;
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
    private void OnDrawGizmos()
        {
            var objects = GetPlaygroundObjects();
            foreach (var obj in objects)
            {
                var renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Vector3 position = obj.transform.position;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(position, 0.1f);
                    UnityEditor.Handles.Label(position + Vector3.right * 0.2f, 
                        $" {obj.name}: {renderer.sortingOrder}");
                }
            }
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
        var leftBodyObjects = playgroundObjects.Where(obj => obj.CompareTag("LeftPlayerBody")).ToList();
        // if (Time.frameCount % 20 == 0)
        // {
        //     Debug.Log($"Left body objects count: {leftBodyObjects.Count}");
        //     foreach (var obj in leftBodyObjects)
        //     {
        //         Debug.Log($"Left body object: {obj.name} at Y: {obj.transform.position.y}");
        //     }
        // }
        if (leftBodyObjects.Count > 0)
        {
            int lastLeftBodyIndex = playgroundObjects.FindLastIndex(obj => obj.CompareTag("LeftPlayerBody"));
            if (Time.frameCount % 20 == 0)
            {
                Debug.Log($"===>Last left body index: {lastLeftBodyIndex}, Object: {playgroundObjects[lastLeftBodyIndex].name}");
            }
            leftBodyObjects.Sort((a, b) => a.GetComponent<SpriteRenderer>().sortingOrder.CompareTo(b.GetComponent<SpriteRenderer>().sortingOrder));
            // if (Time.frameCount % 20 == 0)
            // {
            //     Debug.Log($"====>Left body objects count: {leftBodyObjects.Count}");
            //     foreach (var obj in leftBodyObjects)
            //     {
            //         Debug.Log($"Left body object: {obj.name}, Sorting Order: {obj.GetComponent<SpriteRenderer>().sortingOrder}");
            //     }
            // }


            // for(int i=0;i<leftBodyObjects.Count;i++){
            //     leftBodyObjects[i].GetComponent<SpriteRenderer>().sortingOrder = lastLeftBodyIndex + i + 1;
            // }


            // int adjustedIndex = Math.Min(lastLeftBodyIndex, playgroundObjects.Count);
            playgroundObjects.RemoveAll(obj => obj.CompareTag("LeftPlayerBody"));

            if (Time.frameCount % 20 == 0)
            {
                Debug.Log($"====>BeforeInsert: insert index {lastLeftBodyIndex}   playgroundObjects count: {playgroundObjects.Count}");
                for (int i = 0; i < playgroundObjects.Count; i++)
                {
                    Debug.Log($"{playgroundObjects[i].name}: {playgroundObjects[i].GetComponent<SpriteRenderer>().sortingOrder}");
                }

            }


            int insertIndex = lastLeftBodyIndex - leftBodyObjects.Count +1;
            playgroundObjects.InsertRange(insertIndex, leftBodyObjects);
            if (Time.frameCount % 20 == 0)
            {
                Debug.Log($"====>playgroundObjects.InsertRange: insert index {insertIndex}   playgroundObjects count: {playgroundObjects.Count}");
                // for (int i = 0; i < playgroundObjects.Count; i++)
                // {
                //     Debug.Log($"{playgroundObjects[i].name}: {playgroundObjects[i].GetComponent<SpriteRenderer>().sortingOrder}");
                // }

            }
            // Debug.Log($"Last left body index: {lastLeftBodyIndex}");
            // Debug.Log($"Left body objects count: {leftBodyObjects.Count}");



            // float averageY = leftBodyObjects.Average(obj => obj.transform.position.y);
            // int insertIndex = playgroundObjects.FindIndex(obj => obj.transform.position.y < averageY);
            // if (insertIndex < 0) insertIndex = playgroundObjects.Count;
            
            // playgroundObjects.RemoveAll(obj => obj.CompareTag("LeftPlayerBody"));
            // playgroundObjects.InsertRange(insertIndex, leftBodyObjects);
        }


        
        for (int i = 0; i < playgroundObjects.Count; i++)
        {
            SpriteRenderer renderer = playgroundObjects[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = i;
            }
        }

        if (Time.frameCount % 20 == 0)
        {
            Debug.Log($"====>Sorted  playgroundObjects count: {playgroundObjects.Count}");
            for (int i = 0; i < playgroundObjects.Count; i++)
            {
                Debug.Log($"{playgroundObjects[i].name}: {playgroundObjects[i].GetComponent<SpriteRenderer>().sortingOrder}");
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
