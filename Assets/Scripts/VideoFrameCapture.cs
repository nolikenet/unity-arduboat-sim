using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStream : MonoBehaviour {

    private int frameRate = 30;
    private float captureInterval;
    private bool isCapturing = false;
    private Queue<byte[]> frameBuffer = new Queue<byte[]>();
    private int maxBufferSize = 30; // Store up to 1 second of frames

    void Start() {
        captureInterval = 1.0f / frameRate;
        //StartCapture();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("[video frame] start capture");
            StartCapture();
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            Debug.Log("[video frame] end capture");
            StopCapture();
        }
    }

    public void StartCapture() {
        if (!isCapturing) {
            isCapturing = true;
            StartCoroutine(CaptureFrames());
        }
    }

    public void StopCapture() {
        isCapturing = false;
    }

    private IEnumerator CaptureFrames() {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (isCapturing) {
            yield return waitForEndOfFrame;

            // Create a render texture
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            Camera.main.targetTexture = rt;

            // Render the camera's view to the RenderTexture
            Camera.main.Render();

            // Create a new Texture2D and read the RenderTexture data into it
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            // Convert texture to byte array
            byte[] bytes = texture.EncodeToPNG();
            
            Debug.Log("[video capture] Captured Frame " + bytes.Length);

            // Add to buffer
            if (frameBuffer.Count >= maxBufferSize) {
                frameBuffer.Dequeue(); // Remove oldest frame if buffer is full
            }

            frameBuffer.Enqueue(bytes);

            // Clean up
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            Destroy(texture);

            // Wait for next frame interval
            yield return new WaitForSeconds(captureInterval);
        }
    }

    // Method to get the next frame from the buffer
    public byte[] GetNextFrame() {
        if (frameBuffer.Count > 0) {
            return frameBuffer.Dequeue();
        }

        return null;
    }

    // Method to check if frames are available
    public bool HasFrames() {
        return frameBuffer.Count > 0;
    }

    // Method to get current buffer size
    public int GetBufferSize() {
        return frameBuffer.Count;
    }

}