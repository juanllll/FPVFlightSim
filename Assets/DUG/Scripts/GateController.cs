using UnityEngine;
using System.Collections.Generic;

namespace DUG
{
    public class GateController : MonoBehaviour
    {
        // 통과 시 비활성화되거나 색이 바뀌게 할 프레임 오브젝트
        public GameObject gateFrame;
        private bool hasBeenPassed = false;

        // OnTriggerEnter는 Is Trigger가 켜진 Collider에 다른 Collider가 들어왔을 때 호출됩니다.
        private void OnTriggerEnter(Collider other)
        {
            // 아직 통과하지 않았고, 들어온 오브젝트의 태그가 "Player" 라면
            if (!hasBeenPassed && other.CompareTag("Player"))
            {
                hasBeenPassed = true;
                Debug.Log("게이트 통과!");

                // 여기에 점수 추가, 다음 목표 게이트 활성화 등의 로직을 넣습니다.

                // 예시: 통과하면 게이트 프레임의 색을 회색으로 변경
                // 실제로는 머티리얼의 Emission 색을 바꾸는 것이 좋습니다.
                Renderer frameRenderer = gateFrame.GetComponent<Renderer>();
                if (frameRenderer != null)
                {
                    frameRenderer.material.SetColor("_EmissionColor", Color.blue);
                }

                // 또는 간단히 통과한 게이트를 비활성화 할 수도 있습니다.
                // gameObject.SetActive(false);
            }
        }
    }
}