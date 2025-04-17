using System.Collections.Generic;
using UnityEngine;

public class RandomGadgetSpawner : MonoBehaviour
{

    // [SerializeField] List<SpawnDestinationPair> leftSideSpawnDestinations;
    // [SerializeField] List<SpawnDestinationPair> rightSideSpawnDestinations;

    [SerializeField] float gadgetMoveSpeed = 30f;
    [SerializeField] List<GameObject> leftSideSpawnDestinations;
    [SerializeField] List<GameObject> rightSideSpawnDestinations;

    private Dictionary<GameObject,Transform> gadgets = new Dictionary<GameObject,Transform>();

    // public struct SpawnDestinationPair{
    //     public GameObject spawn;
    //     public GameObject destination;
    // }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update the position of each gadget towards its destination
        foreach (var gadgetPair in gadgets)
        {
            MoveGadget(gadgetPair.Key, gadgetPair.Value);
        }
    }

    public void SpawnGadget(GameObject gadgetPrefab, bool isRightSide)
    {
        //Debug.Log("$SpawnGadget called. From RandomGadgetSpawner isRightSide: " + isRightSide);
        // Check if the gadgetPrefab is null
        List<GameObject> spawnDestinations = isRightSide ? rightSideSpawnDestinations : leftSideSpawnDestinations;
        int randomIndex = Random.Range(0, spawnDestinations.Count);
        GameObject spawnDestination = spawnDestinations[randomIndex];
        Vector2 spawnPosition = spawnDestination.transform.position;
        float screenHeight = Camera.main.orthographicSize * 2;
        spawnPosition.y = screenHeight * 2;
        GameObject gadget = Instantiate(gadgetPrefab, spawnPosition, Quaternion.identity);
      //  GameObject gadget = Instantiate(gadgetPrefab, spawnDestination.transform.position, Quaternion.identity);
        gadgets.Add(gadget, spawnDestination.transform);
        Debug.Log($"Spawned gadget: {gadget.name} at position: {spawnPosition} to destination: {spawnDestination.transform.position}");
    //    MoveGadget(gadget, spawnDestination);
    }
    private void MoveGadget(GameObject gadget, Transform destination)
    {
        if(gadget.transform.position.y == destination.position.y){
            return;
        }
        Vector3 targetPosition = destination.position;
        float step = gadgetMoveSpeed * Time.deltaTime;
        gadget.transform.position = Vector3.MoveTowards(gadget.transform.position, targetPosition, step);
    }
    public void RemoveAllRandomGadgets()
    {
        foreach (var gadgetPair in gadgets)
        {
            Destroy(gadgetPair.Key);
        }
        gadgets.Clear();
    }
}
