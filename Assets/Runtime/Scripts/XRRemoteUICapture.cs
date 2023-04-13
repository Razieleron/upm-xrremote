using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XRRemote
{
    public class OnUICapturedArgs : EventArgs
    {
        public int frameCount;
        public byte[] data;
    }

#if UNITY_EDITOR
    public class XRRemoteUICapture : MonoBehaviour
    {
        public event EventHandler OnUICaptured;

        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private RenderTexture targetTexture;
        [SerializeField] LayerMask layerToCapture;

        [SerializeField] private RawImage testImage;

        int canvasWidth = 1080;
        int canvasHeight = 1920;
        int width = 1080 / 15;
        int height = 1920 / 15;
        int depthBuffer = 0;

        private FragmentSender fragmentSender;

        private void Awake()
        {
            // Initialize targetTexture
            if (targetTexture == null) {
                //Create a new RenderTexture
                targetTexture = new RenderTexture(
                    width,
                    height,
                    depthBuffer,
                    RenderTextureFormat.ARGB32
                );
            }

            //If canvas is found, use its dimensions instead
            // if (uiCanvas != null) {
            //     RectTransform uiCanvasRectTransform = uiCanvas.GetComponent<RectTransform>();
            //     canvasWidth = Mathf.RoundToInt(uiCanvasRectTransform.rect.width);
            //     canvasHeight = Mathf.RoundToInt(uiCanvasRectTransform.rect.height);
            // }

            if (TryGetComponent<FragmentSender>(out FragmentSender fragmentSender)) {
                this.fragmentSender = fragmentSender;
            } else {
                if (DebugFlags.displayXRFragmentSender) {
                    Debug.LogWarningFormat("XRRemoteUICapture: required FragmentSender component not found.");
                }
            }
        }

        public void CaptureUiToRenderTexture()
        {
            //Save canvas's previous state
            Camera prevCam = uiCanvas.worldCamera;
            RenderMode prevRenderMode = uiCanvas.renderMode;

            //Enable UI Camera and ready canvas for capture
            uiCamera.gameObject.SetActive(true);
            uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            uiCanvas.worldCamera = uiCamera;
            uiCamera.targetTexture = targetTexture;
            uiCamera.cullingMask = layerToCapture;

            //Capture the UI layer to targetTexture
            uiCamera.Render();

            //Restore canvas's previous state and disable UI Camera
            uiCamera.targetTexture = null;
            uiCanvas.worldCamera = prevCam;
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCamera.gameObject.SetActive(false);

            //Save targetTexture as Texture2D
            Texture2D uiScreenShot = new Texture2D(canvasWidth / 2, canvasHeight / 2, TextureFormat.RGBA32, false);
            RenderTexture.active = targetTexture;
            uiScreenShot.ReadPixels(new Rect(0,0, targetTexture.width, targetTexture.height), 0, 0);
            RenderTexture.active = null;

            //Start sending uncompressed version of the UI capture
            byte[] textureData = uiScreenShot.EncodeToPNG();
            //Debug.Log($"Texture UNcompressed data is {textureData.Length} bytes");
            fragmentSender.SendBytesToClient(Time.frameCount, textureData);
            
            //Compress a version that can sent over in a single transmission as placeholder
            TextureScale.Point(uiScreenShot, width, height);
            textureData = uiScreenShot.EncodeToPNG();
            //Debug.Log($"Texture compressed data is {textureData.Length} bytes");

            //test: Show preview on Client
            if (testImage != null) {
                Texture2D remoteCanvasTexture = new Texture2D(canvasWidth, canvasHeight, TextureFormat.RGBA32, false);
                remoteCanvasTexture.LoadImage(textureData);
                remoteCanvasTexture.Apply();

                //Debug.Log($"OnUICaptureRecieved: {remoteCanvasTexture.width} x {remoteCanvasTexture.height}");
                testImage.texture = remoteCanvasTexture;
            }

            //Broadcast event
            OnUICaptured?.Invoke(this, new OnUICapturedArgs {
                frameCount = Time.frameCount,
                data = textureData,
            });
        }
    }
#endif
}
