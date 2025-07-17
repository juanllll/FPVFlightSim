using UnityEngine;
using System.Collections.Generic;

namespace DUG
{
    public class GateController : MonoBehaviour
    {
        public GameObject gateFrame;
        private bool hasBeenPassed = false;
        private Renderer frameRenderer;

        void Awake()
        {
            frameRenderer = gateFrame.GetComponent<Renderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hasBeenPassed && other.CompareTag("Player"))
            {
                if (GateManager.Instance != null)
                {
                    GateManager.Instance.GateTriggered(this);
                }
            }
        }


        public void GatePassedVisuals()
        {
            if (!hasBeenPassed)
            {
                hasBeenPassed = true;
                if (frameRenderer != null)
                {
                    frameRenderer.material.SetColor("_EmissionColor", Color.blue);
                }
            }
        }
        public void SetGateActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            if (isActive && !hasBeenPassed)
            {
                if (frameRenderer != null)
                {
                    frameRenderer.material.SetColor("_EmissionColor", Color.red);
                }
            }
        }
    }
}