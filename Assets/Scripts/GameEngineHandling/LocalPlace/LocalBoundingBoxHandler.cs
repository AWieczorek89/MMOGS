using UnityEngine;

public class LocalBoundingBoxHandler : MonoBehaviour
{
    private enum BoundingQuadType
    {
        Lower,
        Upper,
        Northern,
        Southern,
        Western,
        Eastern
    }

    public GameObject _quadSection;
    public Material _boundingBoxMaterial;
    public float _boundingBoxMargin = 0f;
    

    private void ClearAllBoundingQuads()
    {
        foreach (Transform child in _quadSection.transform)
            Destroy(child.gameObject);
    }

    private void MakeBoundingQuad(BoundingQuadType type, float size)
    {
        GameObject boundingQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        boundingQuad.GetComponent<Renderer>().material = _boundingBoxMaterial;
        Transform quadTransform = boundingQuad.transform;
        quadTransform.parent = _quadSection.transform;
        quadTransform.localScale = new Vector3(size + _boundingBoxMargin * 2, size + _boundingBoxMargin * 2, 1f);
        string quadName = "BoundingBoxQuad";

        switch (type)
        {
            case BoundingQuadType.Lower:
                quadName += "L";
                quadTransform.eulerAngles = new Vector3(90f, 0f, 0f);
                quadTransform.position = new Vector3
                (
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2), 
                    -1 * _boundingBoxMargin, 
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2)
                );
                break;
            case BoundingQuadType.Upper:
                quadName += "U";
                quadTransform.eulerAngles = new Vector3(-90f, 0f, 0f);
                quadTransform.position = new Vector3
                (
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2),
                    size + _boundingBoxMargin,
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2)
                );
                break;
            case BoundingQuadType.Northern:
                quadName += "N";
                quadTransform.eulerAngles = new Vector3(0f, 90f, 0f);
                quadTransform.position = new Vector3
                (
                    size + _boundingBoxMargin,
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2),
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2)
                );
                break;
            case BoundingQuadType.Southern:
                quadName += "S";
                quadTransform.eulerAngles = new Vector3(0f, -90f, 0f);
                quadTransform.position = new Vector3
                (
                    -1 * _boundingBoxMargin,
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2),
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2)
                );
                break;
            case BoundingQuadType.Western:
                quadName += "W";
                quadTransform.eulerAngles = new Vector3(0f, 0f, 0f);
                quadTransform.position = new Vector3
                (
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2),
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2),
                    size + _boundingBoxMargin
                );
                break;
            case BoundingQuadType.Eastern:
                quadName += "E";
                quadTransform.eulerAngles = new Vector3(0f, -180f, 0f);
                quadTransform.position = new Vector3
                (
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2),
                    ((size + _boundingBoxMargin) / 2) - (_boundingBoxMargin / 2),
                    -1 * _boundingBoxMargin
                );
                break;
            default:
                UpdateLog($"MakeBoundingQuad() - unknown type [{type.ToString()}]");
                break;
        }

        boundingQuad.name = quadName;
        boundingQuad.tag = "BoundingBox";
        boundingQuad.layer = 11;
        Rigidbody rb = boundingQuad.AddComponent<Rigidbody>() as Rigidbody;
        rb.useGravity = false;
        rb.isKinematic = true;
        MeshRenderer renderer = boundingQuad.GetComponent<MeshRenderer>() as MeshRenderer;
        renderer.enabled = false;
    }

    public void ReloadBoundingQuads(int boundSize)
    {
        if (boundSize < 1)
        {
            UpdateLog($"Cannot reload bounding quads, wrong bounding value [{boundSize}]");
            return;
        }

        ClearAllBoundingQuads();
        MakeBoundingQuad(BoundingQuadType.Upper, boundSize);
        MakeBoundingQuad(BoundingQuadType.Lower, boundSize);
        MakeBoundingQuad(BoundingQuadType.Northern, boundSize);
        MakeBoundingQuad(BoundingQuadType.Southern, boundSize);
        MakeBoundingQuad(BoundingQuadType.Western, boundSize);
        MakeBoundingQuad(BoundingQuadType.Eastern, boundSize);
    }

    private void UpdateLog(string text)
    {
        MainGameHandler.GetChatHandler().UpdateLog(text);
    }
}
