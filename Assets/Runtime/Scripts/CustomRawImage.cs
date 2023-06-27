using UnityEngine;
using UnityEngine.UI;

namespace KlakNDI_Test.Assets.Scripts.ObjectSerializationExtension
{

    [RequireComponent(typeof(AspectRatioFitter))]
    public class CustomRawImage : RawImage
    {
        public float deviceAspectRatio; // Property in the custom object class

        private void Start()
        {
            AspectRatioFitter aspectFitter = GetComponent<AspectRatioFitter>();
            if (aspectFitter != null)
            {
                aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                // Set the property of the required AspectRatioFitter component based on the custom object class property
                aspectFitter.aspectRatio = deviceAspectRatio;
                if (aspectFitter.aspectRatio == 0)
                {
                    aspectFitter.aspectRatio = .565f;    
                }    
            }
        }
    }
}





