using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class CameraController
{
    [Inject]
    private CinemachineCamera _camera;

    [Inject]
    private CinemachineConfiner2D _confiner2D;

    public void SetPosition(Vector2 pos)
    {
        _camera.ForceCameraPosition(pos, quaternion.identity);
    }

    public void SetNewBoundingShape(PolygonCollider2D bounds)
    {
        _confiner2D.BoundingShape2D = bounds;
    }
}