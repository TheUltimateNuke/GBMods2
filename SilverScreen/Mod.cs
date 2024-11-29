using MelonLoader;
using UnityEngine;

namespace SilverScreen;
public class Mod : MelonMod
{
    public static class BuildInfo
    {
        public const string Name = nameof(SilverScreen);
        public const string Version = "1.0.0";
        public const string Description = "A mod for Gang Beasts that adds some cinematic-related tweaks to the game.";
        public const string Author = "TheUltimateNuke";
        public const string Company = "CementGB";
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        // credit to rafa for these settings

        switch (sceneName)
        {
            case "Rooftop":
                var lightObj = GameObject.Find("Lighting & Effects/Light_City_Directional_01");

                if (lightObj != null)
                {
                    var light = lightObj.GetComponent<Light>();

                    light.transform.rotation = Quaternion.Euler(43, 32, 23);
                    light.shadowStrength = 1;
                    light.color = new Color(0.9906f, 0.8025f, 0.6511f);
                    light.intensity = 3;
                }
                break;
            case "Incinerator":
                var lightObj2 = GameObject.Find("Lighting/Effects/Light Axle/Fire Pit Light long");

                if (lightObj2 != null)
                {
                    var light2 = lightObj2.GetComponent<Light>();

                    light2.shadowNearPlane = 0.07f;
                    light2.shadowNormalBias = 0.07f;
                    light2.range = 40;
                    light2.intensity = 3000f;
                    light2.color = new(0.9057f, 0.3458f, 0);
                }
                break;
            case "Trawler":
                var lightObj3 = GameObject.Find("Light_City_Directional01 (1)");

                if (lightObj3 != null)
                    lightObj3.SetActive(false);

                break;
            case "Trucks":
            case "Train":
                var lightObj4 = GameObject.Find("Lighting & Effects/Directional Light/Directional Light (1)");
                var lightObj5 = GameObject.Find("Lighting & Effects/Directional Light");

                if (lightObj4 != null)
                    lightObj4.SetActive(false);

                if (lightObj5 != null)
                {
                    var light5 = lightObj5.GetComponent<Light>();
                    
                    light5.shadowStrength = 1;
                }
                break;
            case "Girders":
                var lightObj6 = GameObject.Find("World/Light_City_Directional_01");
                
                if (lightObj6 != null)
                {
                    var light6 = lightObj6.GetComponent<Light>();

                    light6.transform.rotation = Quaternion.Euler(47.7282f, 131.4548f, 180.003f);
                    light6.intensity = 2;
                    light6.shadowNormalBias = 0.02f;
                    light6.shadowStrength = 1;
                }
                break;
        }
    }
}
