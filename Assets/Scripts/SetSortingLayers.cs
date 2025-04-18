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
        // List<GameObject> leftPlayerBody = playgroundObjects.FindAll(obj => obj.CompareTag("LeftPlayerBody"));
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
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

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
        if (leftBodyObjects.Count > 0)
        {
            int lastLeftBodyIndex = playgroundObjects.FindLastIndex(obj => obj.CompareTag("LeftPlayerBody"));
            leftBodyObjects.Sort((a, b) => a.GetComponent<SpriteRenderer>().sortingOrder.CompareTo(b.GetComponent<SpriteRenderer>().sortingOrder));
            playgroundObjects.RemoveAll(obj => obj.CompareTag("LeftPlayerBody"));
            int insertIndex = lastLeftBodyIndex - leftBodyObjects.Count +1;
            playgroundObjects.InsertRange(insertIndex, leftBodyObjects);
        }
        
        var rightBodyObjects = playgroundObjects.Where(obj => obj.CompareTag("RightPlayerBody")).ToList();
        if (rightBodyObjects.Count > 0)
        {
            int lastRightBodyIndex = playgroundObjects.FindLastIndex(obj => obj.CompareTag("RightPlayerBody"));
            rightBodyObjects.Sort((a, b) => a.GetComponent<SpriteRenderer>().sortingOrder.CompareTo(b.GetComponent<SpriteRenderer>().sortingOrder));
            playgroundObjects.RemoveAll(obj => obj.CompareTag("RightPlayerBody"));
            int insertIndex = lastRightBodyIndex - rightBodyObjects.Count +1;
            playgroundObjects.InsertRange(insertIndex, rightBodyObjects);
        }

        var bombBodyObjects = playgroundObjects.Where(obj => obj.CompareTag("BombBody")).ToList();
        if( bombBodyObjects.Count > 0)
        {
                // if (Time.frameCount % 20 == 0)
                // {
                //     Debug.Log($"====>bombBodyObjects count: {bombBodyObjects.Count}");
                //     for (int i = 0; i < bombBodyObjects.Count; i++)
                //     {
                //         Debug.Log($"i:{i}  {bombBodyObjects[i].name}: {bombBodyObjects[i].GetComponent<SpriteRenderer>().sortingOrder}");
                //     }
                // }
            int lastBombBodyIndex = playgroundObjects.FindLastIndex(obj => obj.CompareTag("BombBody"));
            bombBodyObjects.Sort((a, b) => a.GetComponent<SpriteRenderer>().sortingOrder.CompareTo(b.GetComponent<SpriteRenderer>().sortingOrder));
            playgroundObjects.RemoveAll(obj => obj.CompareTag("BombBody"));
            int insertIndex = lastBombBodyIndex - bombBodyObjects.Count +1;
            playgroundObjects.InsertRange(insertIndex, bombBodyObjects);
        }

        for (int i = 0; i < playgroundObjects.Count; i++)
        {
            SpriteRenderer renderer = playgroundObjects[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = i;
            }
        }

        // if (Time.frameCount % 20 == 0)
        // {
        //     Debug.Log($"====>After   playgroundObjects count: {playgroundObjects.Count}");
        //     for (int i = 0; i < playgroundObjects.Count; i++)
        //     {
        //         Debug.Log($"i:{i}  {playgroundObjects[i].name}: {playgroundObjects[i].GetComponent<SpriteRenderer>().sortingOrder}");
        //     }

        // }
    }

    private void ProcessBombLayers(List<GameObject> playgroundObjects)
    {
        int baseOrder = 0;
        bool rightSideHasBomb=false;
        foreach (GameObject player in players)
        {
            PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (playerMovement != null && playerMovement.mBomb != null)
            {
                if(playerMovement.isRightSide){
                    rightSideHasBomb = true;
                }
            }
        }
        String sideTag = rightSideHasBomb ? "RightPlayerBody" : "LeftPlayerBody";

        var rightArm = playgroundObjects.FirstOrDefault(obj => obj.CompareTag(sideTag) &&  obj.name == "Right Arm");
        if (rightArm != null)
        {
            baseOrder = rightArm.GetComponent<SpriteRenderer>().sortingOrder;
        }
        int skip = ProcessPlayerBombs(baseOrder); // skip equals zero if the bomb is not being held
        int bombBodySkip = ProcessBombBodies(playgroundObjects);
        AdjustRemainingLayers(playgroundObjects, baseOrder, skip, bombBodySkip);

        playgroundObjects.Sort((a, b) => a.GetComponent<SpriteRenderer>().sortingOrder.CompareTo(b.GetComponent<SpriteRenderer>().sortingOrder));




        // if (Time.frameCount % 20 == 0)
        // {
        //     Debug.Log($"====>Before Swapping ProcessBombLayers  playgroundObjects count: {playgroundObjects.Count}");
        //     Debug.Log($"====>baseOrder: {baseOrder}  skip: {skip}  bombBodySkip: {bombBodySkip}");
        //     for (int i = 0; i < playgroundObjects.Count; i++)
        //     {
        //         Debug.Log($"i:{i}  tag:{playgroundObjects[i].tag}  {playgroundObjects[i].name}: {playgroundObjects[i].GetComponent<SpriteRenderer>().sortingOrder}");
        //     }
        // }





        int rightArmIndex = playgroundObjects.FindIndex(obj => obj.CompareTag(sideTag) && obj.name == "Right Arm");

     //   int rightArmIndex = playgroundObjects.FindIndex(obj => obj.name == "Right Arm");
        int bombBodyIndex = playgroundObjects.FindIndex(obj => obj.name == "BombBody");
        int bombSortingOrder = 0;
        if (Time.frameCount % 20 == 0)
        {
            Debug.Log($"====>rightArmIndex: {rightArmIndex}  bombBodyIndex: {bombBodyIndex}");
        }
        if (rightArmIndex >= 0 && bombBodyIndex>=0)
        {

            if (Time.frameCount % 20 == 0)
            {
                Debug.Log($"====>rightArmIndex: {rightArmIndex}  bombBodyIndex: {bombBodyIndex}");
            }
            
            //here is the problem
            playgroundObjects[rightArmIndex].GetComponent<SpriteRenderer>().sortingOrder = playgroundObjects[bombBodyIndex].GetComponent<SpriteRenderer>().sortingOrder + 1;
            bombSortingOrder = playgroundObjects[bombBodyIndex].GetComponent<SpriteRenderer>().sortingOrder;
        }
        //bombSortingOrder = playgroundObjects[bombBodyIndex].GetComponent<SpriteRenderer>().sortingOrder;
        for(int i=bombBodyIndex+1; i<playgroundObjects.Count; i++)
        {
                playgroundObjects[i].GetComponent<SpriteRenderer>().sortingOrder = bombSortingOrder + i;
        }
    }

    private int ProcessPlayerBombs(int baseOrder)
    {
        int skip = 0;
        foreach (GameObject player in players)
        {
            PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (playerMovement != null && playerMovement.mBomb != null)
            {
                SpriteRenderer[] bombRenderer = playerMovement.mBomb.GetComponentsInChildren<SpriteRenderer>();
                SpriteRenderer playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
                //baseOrder = playerRenderer.sortingOrder;
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
    #if UNITY_EDITOR
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
    #endif
}
