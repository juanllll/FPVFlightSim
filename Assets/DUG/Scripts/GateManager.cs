using UnityEngine;
using System.Collections.Generic;

namespace DUG
{
    public class GateManager : MonoBehaviour
    {
        public static GateManager Instance { get; private set; } // Singleton pattern for easy access

        public List<GateController> orderedGates; // Drag and drop your gates in the desired order in the Inspector
        private int nextGateIndex = 0;

        public delegate void GatePassedAction(int gateIndex);
        public static event GatePassedAction OnCorrectGatePassed;
        public static event GatePassedAction OnIncorrectGatePassed;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Ensures only one instance of GateManager exists
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            InitializeGates();
        }

        private void InitializeGates()
        {
            // Optionally, disable all gates except the first one at the start
            for (int i = 0; i < orderedGates.Count; i++)
            {
                if (i == 0)
                {
                    orderedGates[i].SetGateActive(true); // Activate the first gate
                }
                else
                {
                    orderedGates[i].SetGateActive(false); // Deactivate subsequent gates
                }
            }
        }

        // Called by individual GateController when it's triggered
        public void GateTriggered(GateController gate)
        {
            if (nextGateIndex < orderedGates.Count)
            {
                if (gate == orderedGates[nextGateIndex])
                {
                    Debug.Log($"Correct Gate {nextGateIndex + 1} Passed!");
                    gate.GatePassedVisuals(); // Apply visual change
                    OnCorrectGatePassed?.Invoke(nextGateIndex); // Notify listeners

                    nextGateIndex++; // Move to the next expected gate

                    if (nextGateIndex < orderedGates.Count)
                    {
                        orderedGates[nextGateIndex].SetGateActive(true); // Activate the next gate
                    }
                    else
                    {
                        Debug.Log("All gates passed! Race finished!");
                        // Add game end logic here (e.g., show results, load next level)
                    }
                }
                else
                {
                    Debug.Log($"Incorrect Gate Passed! Expected Gate {nextGateIndex + 1}.");
                    OnIncorrectGatePassed?.Invoke(nextGateIndex); // Notify listeners
                    // You might want to add a penalty or reset the player here
                }
            }
        }

        public GateController GetNextExpectedGate()
        {
            if (nextGateIndex < orderedGates.Count)
            {
                return orderedGates[nextGateIndex];
            }
            return null;
        }

        public int GetNextGateIndex()
        {
            return nextGateIndex;
        }
    }
}