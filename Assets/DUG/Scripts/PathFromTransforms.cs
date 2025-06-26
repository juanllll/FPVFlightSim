using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DUG
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class PathFromTransforms : MonoBehaviour
    {
        [Header("Path Holder 부모 Transform")]
        public Transform routesTransform;

        [Header("곡선 설정")]
        [Tooltip("각 제어점 사이를 몇 개의 점으로 나눌지 결정합니다. 높을수록 부드럽습니다.")]
        [Range(2, 50)]
        public int curveResolution = 20;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (routesTransform == null)
            {
                if (lineRenderer != null)
                {
                    lineRenderer.positionCount = 0;
                }
                return;
            }

            DrawCurve();
        }

        // 곡선을 그리는 함수
        private void DrawCurve()
        {
            var airGates = routesTransform.GetComponentsInChildren<PathHolder>().Select(x => x.pathPoint).ToList();

            // Line Renderer의 총 점 개수를 계산합니다.
            lineRenderer.positionCount = (airGates.Count - 1) * curveResolution + 1;
            
            int pointIndex = 0;

            // 각 제어점 세그먼트(i -> i+1)를 순회합니다.
            for (int i = 0; i < airGates.Count - 1; i++)
            {
                // Catmull-Rom 계산에 필요한 4개의 점을 가져옵니다.
                // 배열 범위를 벗어나지 않도록 Mathf.Clamp를 사용합니다.
                Transform p0 = airGates[Mathf.Clamp(i - 1, 0, airGates.Count - 1)];
                Transform p1 = airGates[i];
                Transform p2 = airGates[i + 1];
                Transform p3 = airGates[Mathf.Clamp(i + 2, 0, airGates.Count - 1)];
                
                // 각 세그먼트 내에서 해상도(Resolution)만큼 점을 찍습니다.
                for (int j = 0; j < curveResolution; j++)
                {
                    float t = j / (float)curveResolution;
                    Vector3 position = CalculateCatmullRomPosition(t, p0.position, p1.position, p2.position, p3.position);
                    lineRenderer.SetPosition(pointIndex, position);
                    pointIndex++;
                }
            }
            
            // 마지막 점을 정확히 찍어줍니다.
            lineRenderer.SetPosition(pointIndex, airGates[airGates.Count - 1].position);
        }

        // Catmull-Rom 스플라인 위치 계산 함수
        private Vector3 CalculateCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return 0.5f * ((2f * p1) +
                    (-p0 + p2) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t);
        }
    }
}