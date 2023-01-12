using System.Collections;
using System.Collections.Generic;
using KartGame.KartSystems;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private GameObject trackPath;

    public GameObject miniMapCam;

    public bool autoFindKarts = true;
    public ArcadeKart playerKart;

    ArcadeKart[] karts;
    private GameObject localPlayer;
    // Start is called before the first frame update
    void Start()
    {
        if (autoFindKarts)
        {
            karts = FindObjectsOfType<ArcadeKart>();
            if (karts.Length > 0)
            {
                if (!playerKart) playerKart = karts[0];
                localPlayer = karts[0].gameObject;
            }
            DebugUtility.HandleErrorIfNullFindObject<ArcadeKart, MiniMap>(playerKart, this);
            
        }


        lineRenderer = GetComponent<LineRenderer>();
        trackPath = this.gameObject;

        int num_of_path = trackPath.transform.childCount;
        lineRenderer.positionCount = num_of_path;

        for (int x = 0; x < num_of_path; x++)
        {
            lineRenderer.SetPosition(x, new Vector3(trackPath.transform.GetChild(x).transform.position.x,
            4,trackPath.transform.GetChild(x).transform.position.z));
        }

        lineRenderer.SetPosition(num_of_path, lineRenderer.GetPosition(0));
        lineRenderer.startWidth = 7f;
        lineRenderer.endWidth = 7f;
    }

    // Update is called once per frame
    void Update()
    {
        miniMapCam.transform.position = (new Vector3(localPlayer.transform.position.x,
        miniMapCam.transform.position.y,localPlayer.transform.position.z));
    }
}
