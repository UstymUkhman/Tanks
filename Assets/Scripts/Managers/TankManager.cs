using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color playerColor;            
    public Transform spawnPoint;

    [HideInInspector] public int wins;
    [HideInInspector] public int playerNumber;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public string coloredPlayerText;

    private TankShooting shooting;
    private TankMovement movement;
    private GameObject canvasGameObject;


    public void Setup()
    {
        movement = instance.GetComponent<TankMovement>();
        shooting = instance.GetComponent<TankShooting>();

        movement.playerNumber = playerNumber;
        shooting.playerNumber = playerNumber;

        canvasGameObject  = instance.GetComponentInChildren<Canvas>().gameObject;
        coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER " + playerNumber + "</color>";

        MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = playerColor;
        }
    }
    
    public void DisableControl()
    {
        movement.enabled = false;
        shooting.enabled = false;

        canvasGameObject.SetActive(false);
    }

    public void EnableControl()
    {
        movement.enabled = true;
        shooting.enabled = true;

        canvasGameObject.SetActive(true);
    }

    public void Reset()
    {
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;

        instance.SetActive(false);
        instance.SetActive(true);
    }
}
