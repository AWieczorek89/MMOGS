using BackgroundManagement.Measurement.Units;
using BackgroundManagement.Models.ClientExchangeData;
using System;
using UnityEngine;

public class TerrainSegmentHandler : MonoBehaviour
{
    public Material _terrainColliderBoxMaterial;
    public Material _obstacleColliderBoxMaterial;
    public Material _platformColliderBoxMaterial;

    private TerrainDetails _terrainDetails = null;
    private GameObject _serverCollisionCube = null;

    public void SetTerrainDetails(TerrainDetails terrainDetails)
    {
        _terrainDetails = terrainDetails;
    }

    public void ActivateServerColliderBox(bool active)
    {
        if (_serverCollisionCube != null)
            _serverCollisionCube.SetActive(active);
    }

    public void AssignServerCollisionCube()
    {
        if (_terrainDetails == null)
        {
            Debug.Log($"AssignServerCollisionCube() - terrain details not set");
            return;
        }

        if (_serverCollisionCube != null)
        {
            Destroy(_serverCollisionCube);
        }

        _serverCollisionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _serverCollisionCube.name = "CollisionBox";
        
        //MATERIAL ASSIGNMENT
        Renderer renderer = _serverCollisionCube.GetComponent<Renderer>();
        
        if (_terrainDetails.TodIsPlatform)
        {
            renderer.material = _platformColliderBoxMaterial;
        }
        else
        if (_terrainDetails.TodIsObstacle)
        {
            renderer.material = _obstacleColliderBoxMaterial;
        }
        else //terrain as default
        {
            renderer.material = _terrainColliderBoxMaterial;
        }

        //PHYSICAL COLLIDER REMOVING
        BoxCollider collider = _serverCollisionCube.GetComponent<BoxCollider>();
        if (collider != null)
            Destroy(collider);

        //POSITIONING & SCALING
        _serverCollisionCube.transform.parent = this.gameObject.transform;
        _serverCollisionCube.transform.position = new Vector3
        (
            Convert.ToSingle(_terrainDetails.TodCollision.X) / 2, 
            Convert.ToSingle(_terrainDetails.TodCollision.Z) / 2, 
            Convert.ToSingle(_terrainDetails.TodCollision.Y) / 2
        );
        _serverCollisionCube.transform.localScale = new Vector3(_terrainDetails.TodCollision.X, _terrainDetails.TodCollision.Z, _terrainDetails.TodCollision.Y);
    }
}
