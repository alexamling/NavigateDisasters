using UnityEngine;

namespace KinematicVehicleSystem
{
    

    public class WaterProviderBasic : MonoBehaviour, IWaterProvider
    {

        public float waterLevel;
        public float offset;
        
        private float scrubber;
        private float bounceSpeed = 0.5f;
        private float minimum = -0.5f;
        private float maximum = 0.5f;

        void Start()
        {
            //waterLevel = 8f;
        }

        public float GetWaterLevel(float x, float z)
        {
            return waterLevel; // + offset;
        }

        public float GetStaticWaterLevel()
        {
            return waterLevel;
          
        }

        public void Update()
        {
            offset = Mathf.SmoothStep(minimum, maximum, 0.5f);
            scrubber += bounceSpeed * Time.deltaTime;
            if (scrubber > 1)
            {
                float temp = maximum;
                maximum = minimum;
                minimum = temp;

                scrubber = 0.0f;
            }
        }
    }
}
