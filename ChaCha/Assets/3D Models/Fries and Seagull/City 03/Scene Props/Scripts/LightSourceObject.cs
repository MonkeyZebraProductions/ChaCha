using System;
using System.Collections;
using System.Collections.Generic;
using Seagull.City_03.Inspector;
# if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;
using UnityEngine.Events;

namespace Seagull.City_03.SceneProps {
    [Serializable]
    public class String2GlowLight : KiiValuePair<string, GlowLight> {}

    public class LightSourceObject : MonoBehaviour {
        public bool isOn;
        public float TrafficLightSwitchTime = 5f;
        public Collider RedHitbox;
        public Collider GreenHitbox;
        public List<String2GlowLight> lights = new();
        private Dictionary<string, GlowLight> lightMap = new();
        
        [AButton("Turn On")] public UnityEvent onTurnOn;
        [AButton("Turn Off")] public UnityEvent onTurnOff;
        
        private void Start() {
            lights.ForEach(light => lightMap[light.key] = light.value);
            onTurnOn.AddListener(turnOnAll);
            onTurnOff.AddListener(turnOffAll);
            
            if (isOn) turnOnAll();

            turnOn("Red");
            turnOff("Yellow");
            turnOff("Green");
            GreenHitbox.enabled = false;
        }

        public void StartTraffic()
        {
            StartCoroutine(TrafficLightSwap());
        }
        IEnumerator TrafficLightSwap()
        {
            Collider currentHitbox = isOn ? GreenHitbox : RedHitbox;
            yield return new WaitForSeconds(TrafficLightSwitchTime);
            if(currentHitbox != null )
            {
                currentHitbox.enabled = false;
            }
            turnOn("Yellow");
            turnOff(isOn ? "Green" : "Red");
            yield return new WaitForSeconds(2f);
            turnOff("Yellow");
            isOn = !isOn;
            currentHitbox = isOn ? GreenHitbox : RedHitbox;
            turnOn(isOn ? "Green" : "Red");
            if (currentHitbox != null)
            {
                currentHitbox.enabled = true;
            }
        }
        public void turnOnAll() {
            foreach (var lightMapValue in lightMap.Values) lightMapValue.turnOn();
        }

        public void turnOffAll() {
            foreach (var light in lightMap.Values) light.turnOff();
        }

        public void turnOn(string key) {
            lightMap[key].turnOn();
        }

        public void turnOff(string key) {
            lightMap[key].turnOff();
        }
    }
    
# if UNITY_EDITOR
    [CustomEditor(typeof(LightSourceObject))]
    public class LightSourceObjectInspector : AnInspector { }
# endif
}