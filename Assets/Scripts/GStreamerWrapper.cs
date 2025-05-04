using System;
using System.Collections;
using System.Collections.Generic;
using Gst;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GStreamerWrapper : MonoBehaviour {

    private bool _isInitialized;

    // Start is called before the first frame update
    void Start() {
        InitializeGStreamer();
    }
    
    private void InitializeGStreamer()
    {
        // Initialize GStreamer
            Gst.Application.Init();
            /*
        try
        {
            _isInitialized = true;
            Debug.Log("GStreamer initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize GStreamer: {e.Message}");
            return;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
