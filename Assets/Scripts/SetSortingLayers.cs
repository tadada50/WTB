using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayers : MonoBehaviour
{
    [SerializeField] GameObject[] players;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> playgroundObjects = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        int baseOrder = 0;
        int skip = 0;
        foreach (GameObject obj in allObjects)
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingLayerName == "Playground")
            {
            playgroundObjects.Add(obj);
            }
        }
        playgroundObjects.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
        for (int i = 0; i < playgroundObjects.Count; i++)
        {
            SpriteRenderer renderer = playgroundObjects[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = i;
            }
        }
        foreach (GameObject player in players)
        {
            PlayerMovement playerMovement = player.GetComponentInChildren<PlayerMovement>();
            if (playerMovement != null && playerMovement.bomb!=null)
            {
                SpriteRenderer[] bombRenderer = playerMovement.bomb.GetComponentsInChildren<SpriteRenderer>();
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



                // ParticleSystem[] particleSystems = playerMovement.bomb.GetComponentsInChildren<ParticleSystem>();
                // foreach (ParticleSystem particleSystem in particleSystems)
                // {
                //     ParticleSystemRenderer particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                //     if (particleRenderer != null && particleRenderer.sortingLayerName == "Playground")
                //     {
                //         skip++;
                //         particleRenderer.sortingOrder = baseOrder + skip;
                //     }
                // }


                // Debug.Log($"BaseOrder: {baseOrder}, Skip: {skip}");
            }
        }

        int bombBodyOrder=0;
        int bombBodySkip=0;
        foreach (GameObject obj in playgroundObjects)
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if(obj.name=="BombBody")
            {
                bombBodyOrder = renderer.sortingOrder;
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
           // Debug.Log($"Object: {obj.name}, Order: {renderer.sortingOrder}, Y Position: {obj.transform.position.y}");
        }
        for(int i = baseOrder+1; i < playgroundObjects.Count; i++)
        {
            SpriteRenderer renderer = playgroundObjects[i].GetComponent<SpriteRenderer>();

            if (renderer != null && renderer.name!="Bomb" && renderer.name!="BombBody")
            {
                renderer.sortingOrder += skip + bombBodySkip;
            }
        }   

        foreach (GameObject obj in playgroundObjects)
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
           // Debug.Log($"Object: {obj.name}, Order: {renderer.sortingOrder}, Y Position: {obj.transform.position.y}");
        }

    }
}
