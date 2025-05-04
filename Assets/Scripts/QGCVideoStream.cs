using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;

public class QGCVideoStream : MonoBehaviour {

    [Header("Stream Settings")] public string udpHost = "127.0.0.1";
    public int udpPort = 5600;
    public int width = 640;
    public int height = 480;
    public int frameRate = 25;
    public int bitrate = 500000; // 500 kbps

    [Header("H264 Settings")]
    private byte[] sps = new byte[] {
        /* Your H264 SPS NALU */
    };
    private byte[] pps = new byte[] {
        /* Your H264 PPS NALU */
    };

    private float captureInterval;
    private bool isStreaming = false;
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private byte[] h264Buffer = new byte[1024 * 1024]; // 1MB buffer
    private int h264BufferLength = 0;

    // NALU start code
    private readonly byte[] startCode = new byte[] { 0x00, 0x00, 0x00, 0x01 };

    void Start() {
        captureInterval = 1.0f / frameRate;
        //StartStreaming();
    }

    void OnDestroy() {
        StopStreaming();
    }

    public void StartStreaming() {
        if (!isStreaming) {
            try {
                udpClient = new UdpClient();
                endPoint = new IPEndPoint(IPAddress.Parse(udpHost), udpPort);
                isStreaming = true;

                // Send H264 configuration (SPS and PPS NALUs)
                SendNALU(sps);
                SendNALU(pps);

                // Start capture coroutine
                StartCoroutine(CaptureFrames());
                Debug.Log($"Started streaming to {udpHost}:{udpPort}");
            } catch (Exception e) {
                Debug.LogError($"Failed to start streaming: {e.Message}");
                StopStreaming();
            }
        }
    }

    public void StopStreaming() {
        isStreaming = false;
        if (udpClient != null) {
            udpClient.Close();
            udpClient = null;
        }

        Debug.Log("Stopped streaming");
    }

    private IEnumerator CaptureFrames() {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (isStreaming) {
            yield return waitForEndOfFrame;

            RenderTexture rt = new RenderTexture(width, height, 24);
            Camera.main.targetTexture = rt;
            Camera.main.Render();

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            byte[] rawBytes = texture.GetRawTextureData();

            EncodeH264Frame(rawBytes);

            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            Destroy(texture);

            yield return new WaitForSeconds(captureInterval);
        }
    }

    private void EncodeH264Frame(byte[] rawImageData) {
        // TODO: Implement H264 encoding
        // You'll need to use a native plugin or library for H264 encoding
        // Popular options include:
        // - FFmpeg
        // - x264
        // - MediaFoundation (Windows)
        // - VideoToolbox (iOS/macOS)

        // For now, this is a placeholder
        // The actual implementation would encode the frame and call SendNALU
        // with the resulting H264 NAL units
    }

    private void SendNALU(byte[] naluData) {
        try {
            // Add start code
            byte[] packet = new byte[startCode.Length + naluData.Length];
            Buffer.BlockCopy(startCode, 0, packet, 0, startCode.Length);
            Buffer.BlockCopy(naluData, 0, packet, startCode.Length, naluData.Length);

            // Send over UDP
            udpClient.Send(packet, packet.Length, endPoint);
        } catch (Exception e) {
            Debug.LogError($"Error sending NALU: {e.Message}");
        }
    }

    // Helper method to handle fragmentation if needed
    private void SendFragmentedNALU(byte[] naluData, int maxPacketSize = 1400) {
        // TODO: Implement FU-A fragmentation for large NAL units
        // This would split the NALU into fragments according to RFC 6184
        // Each fragment would be sent as a separate UDP packet
    }

}