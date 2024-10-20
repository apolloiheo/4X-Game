using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public Sprite sprite;
    public Canvas canvas;
    public Camera playerCamera;

    public void Start()
    {
        playerCamera.transform.position = new Vector3(15, 14, -10);
    }


    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            playerCamera.transform.Translate(Vector3.left * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.D))
        {
            playerCamera.transform.Translate(Vector3.right * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.W))
        {
            playerCamera.transform.Translate(Vector3.up * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.S))
        {
            playerCamera.transform.Translate(Vector3.down * Time.deltaTime);
        }
    }


    // Scans the world provided by Game Manager

    // Instantiates UI overlaus
    /*
     * Puts Settlements UI overlay over Settlements
     * UPDATES player's main HUD
     * - Technology Tree
     * - Culture Tress
     * - Economy Tab
     * - ETC
     */

    // Displays Tile Yield Icons on Tiles

    // Displays Tile Resource Images on Tiles
}
