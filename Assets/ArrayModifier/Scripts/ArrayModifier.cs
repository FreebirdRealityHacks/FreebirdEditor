using UnityEngine;
using TMPro;

public enum HeirachySetting { ParentCurrent, CreateParent }
public enum FitType { FixedCount, FitLenght }
public enum OffsetType { Constant, Relative, Radial }
public enum RadialOffsetAxis { Right, Up, Forward }
public enum RotationType { CopyOriginal, AddUpRotation, FaceCenter, BackCenter, identity }

namespace GyroVectors
{
    [ExecuteInEditMode]
    public class ArrayModifier : MonoBehaviour
    {
        //[SerializeField][HideInInspector] private GameObject renderCanvas;
        private GameObject renderCanvas;

        [SerializeField][HideInInspector] private bool hasVisbleObject;
        [SerializeField][HideInInspector] private bool dropDown1, dropDown2 = true;
        [SerializeField][HideInInspector] private HeirachySetting heirachyArrangement;
        [Tooltip("The object to copy as an array")]
        [SerializeField][HideInInspector] private GameObject ArrayObject;
        [Tooltip("Determines how the number of copies obatained ")]
        [SerializeField][HideInInspector] private FitType fitType;
        [Tooltip("Determines how the objects are offset")]
        [SerializeField][HideInInspector] private OffsetType offsetType;
        [Tooltip("Determines the axis of rotation")]
        [SerializeField][HideInInspector] private RadialOffsetAxis radialOffsetAxis;
        [Tooltip("radius of the cirle")]
        [SerializeField][HideInInspector] private float Radius = 3;
        [Tooltip("Circumference of the circle in degree")]
        [SerializeField][HideInInspector][Range(0, 360)] private float ArcExtent = 360;
        [Tooltip("Determines the offset or distance between the objects")]
        [SerializeField][HideInInspector] private Vector3 OffSetAlongAxis;
        [Tooltip("Determines how the objects are oriented along the arc")]
        [SerializeField][HideInInspector] private RotationType rotationType;
        [Tooltip("Center of the objects, the current object is used by default")]
        [SerializeField][HideInInspector] Transform Center;
        [Tooltip("Determines whether to copy only the mesh (filter and renderer) or copy all the components of the original")]
        [SerializeField][HideInInspector] private bool CopyComponents = true;
        [Tooltip("Exact Number of duplicate")]
        [SerializeField][HideInInspector][Min(0)] private int Count = 1;
        [Tooltip("Lenght in unity units of the objects, the number of duplicates is automatically computed")]
        [SerializeField][HideInInspector][Min(0)] private int Lenght = 1;
        [Tooltip("Enabling randomization the position of the duplicates after offset")]
        [SerializeField][HideInInspector] private bool Randomize;
        [Tooltip("Threshold of randomization, the higer the values, the higher the range of randomization")]
        [SerializeField][HideInInspector] private Vector3 RandomThreshold;
        void OnEnable()
        {
            transform.TryGetComponent(out MeshFilter meshfilter);
            transform.TryGetComponent(out MeshRenderer meshRenderer);
            if (meshfilter != null && meshRenderer != null)
            {
                hasVisbleObject = true;
                ArrayObject = gameObject;
            }
            Center = transform;
            OffSetAlongAxis = new Vector3(1,0,0);
        }
        private void Start() {
            if(OffSetAlongAxis == Vector3.zero){
                OffSetAlongAxis = new Vector3(1,0,0);
            }

            renderCanvas = GameObject.Find("Canvas");
        }
        void CustomDestroy(GameObject g)
        {
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(g.gameObject);
                };
            }
            else
            {
                Destroy(g.gameObject);
            }
        }
        void DrawArray()
        {
            renderCanvas = GameObject.Find("Canvas");
            if (ArrayObject == null) return;
            if (Count < 0) Count = 0;
            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    CustomDestroy(transform.GetChild(i).gameObject);
                }
            }
            var arryObjectSpawn = ArrayedObject();
            if (arryObjectSpawn == null) return;
            var arrayParent = ArrayParent();
            var objectsCount = GetCount();
            for (int i = 0; i < objectsCount; i++)
            {
                var arrayObject = Instantiate(arryObjectSpawn, transform.position, arryObjectSpawn.transform.rotation, arrayParent);
                //arrayObject.GetComponent<TextMeshProUGUI>().text = i.ToString();
                //Debug.Log(renderCanvas.name);
                arrayObject.transform.SetParent(renderCanvas.transform, false);
                arrayObject.transform.localScale = NewScale(arrayObject.transform);
                arrayObject.transform.position += ArrayedPosition(i) + RandomPos();
                arrayObject.transform.rotation = AngleRotation(arrayObject.transform, i);
            }
            CustomDestroy(arryObjectSpawn);
        }
        GameObject ArrayedObject()
        {
            GameObject obj = null;
            if (CopyComponents)
            {
                var g = transform.childCount > 0 ? Instantiate(transform.GetChild(0).gameObject) : Instantiate(ArrayObject);
                g.name = ArrayObject.name;
                if(hasVisbleObject)
                    g.GetComponent<MeshRenderer>().enabled = true;
                else
                    g.GetComponentInChildren<MeshRenderer>().enabled = true;
                if (!Application.isPlaying)
                {
                    DestroyImmediate(g.GetComponent<ArrayModifier>());
                    obj = g;
                }
                else
                {
                    Destroy(g.GetComponent<ArrayModifier>());
                    obj = g;
                }

            }
            else
            {
                if(!hasVisbleObject){
                    Debug.Log("Object doesnt have a visible mesh");
                    return null;
                }
                obj = new GameObject(ArrayObject.name);
                obj.transform.localScale = ArrayObject.transform.localScale;
                ArrayObject.transform.TryGetComponent(out MeshFilter meshfilter);
                ArrayObject.transform.TryGetComponent(out MeshRenderer meshRenderer);
                if (!meshfilter || !meshRenderer)
                {
                    CopyComponents = true;
                    obj = gameObject;
                    Destroy(obj.GetComponent<ArrayModifier>());
                }
                else
                {
                    var filter = obj.AddComponent<MeshFilter>();
                    var renderer = obj.AddComponent<MeshRenderer>();
                    if (!Application.isPlaying)
                    {
                        filter.sharedMesh = meshfilter.sharedMesh;
                        renderer.sharedMaterials = meshRenderer.sharedMaterials;
                    }
                    else
                    {
                        filter.mesh = meshfilter.mesh;
                        renderer.materials = meshRenderer.materials;
                    }
                    //TODO: Copy All Components later
                }
            }
            return obj;
        }
        Vector3 ArrayedPosition(int index)
        {
            var pos = Vector3.zero;
            var countOffset = hasVisbleObject ? 1 : 0;
            switch (offsetType)
            {
                case OffsetType.Constant:
                    pos = OffSetAlongAxis * (index + countOffset);
                    break;

                case OffsetType.Relative:
                    pos = GetRelativePos(index + countOffset);
                    break;

                case OffsetType.Radial:
                    pos = GetRadialPos(index);
                    break;

                default:
                    break;
            }

            return pos;
        }

        Vector3 GetRelativePos(int index)
        {
            var pos = Vector3.zero;
            ArrayObject.TryGetComponent(out Renderer rend);
            if (!rend)
            {
                offsetType = OffsetType.Constant;
                pos = OffSetAlongAxis;
            }
            else
            {
                var bounds = rend.bounds;
                pos = new Vector3(bounds.size.x * OffSetAlongAxis.x,
                bounds.size.y * OffSetAlongAxis.y, bounds.size.z * OffSetAlongAxis.z);
            }
            return pos * (index);
        }

        Vector3 GetRadialPos(int i)
        {
            var pos = Vector3.zero;
            float angle = (((float)i / (float)Count) * ArcExtent);
            var xPos = (Radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            var zPos = (Radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            if (radialOffsetAxis == RadialOffsetAxis.Right)
                pos = new Vector3(xPos, 0, zPos);
            else if (radialOffsetAxis == RadialOffsetAxis.Up)
                pos = new Vector3(0, xPos, zPos);
            else if (radialOffsetAxis == RadialOffsetAxis.Forward)
                pos = new Vector3(xPos, zPos, 0);

            if (Center)
                pos += Center.position - transform.position;
            return pos;
        }
        bool changevalue;
        void OnValidate()
        {
            changevalue = true;
        }
        void Update()
        {
            if (changevalue)
            {
                DrawArray();
                changevalue = false;
            }
        }
        Transform ArrayParent()
        {
            Transform parent = null;
            if (heirachyArrangement == HeirachySetting.ParentCurrent)
            {
                parent = transform;
            }
            else
            {
                parent = new GameObject("ArrayParent").transform;
            }
            return parent;
        }

        Quaternion AngleRotation(Transform t, int i)
        {
            var quat = Quaternion.identity;
            if (offsetType != OffsetType.Radial)
            {
                quat = t.rotation;
            }
            else
            {
                switch (rotationType)
                {
                    case RotationType.CopyOriginal:
                        quat = t.rotation;
                        break;
                    case RotationType.AddUpRotation:
                        quat = Quaternion.AngleAxis((((float)i / (float)Count) * -360), Axis());
                        break;
                    case RotationType.FaceCenter:
                        quat = Quaternion.LookRotation(transform.position - t.position, Axis());
                        break;
                    case RotationType.BackCenter:
                        quat = Quaternion.LookRotation(-(transform.position - t.position), Axis());
                        break;
                    default:
                        break;
                }

            }
            return quat;
        }
        Vector3 Axis()
        {
            var ax = Vector3.zero;

            switch (radialOffsetAxis)
            {
                case RadialOffsetAxis.Up:
                    ax = Vector3.right;
                    break;
                case RadialOffsetAxis.Forward:
                    ax = Vector3.forward;
                    break;
                case RadialOffsetAxis.Right:
                    ax = Vector3.up;
                    break;
                default:
                    break;
            }
            return ax;
        }

        int GetCount()
        {
            int _count = 0;
            if (fitType == FitType.FixedCount)
            {
                _count = Count;
            }
            else
            {
                if (offsetType == OffsetType.Constant)
                {
                    var c = new Vector3(Divide(Lenght, OffSetAlongAxis.x),
                    Divide(Lenght, OffSetAlongAxis.y), Divide(Lenght, OffSetAlongAxis.z));
                    var floor = Mathf.Max(c.x, Mathf.Max(c.y, c.z));
                    _count = (int)floor;
                }
                else if (offsetType == OffsetType.Relative)
                {
                    //for now do the same calculation
                    offsetType = OffsetType.Constant;
                    var c = new Vector3(Divide(Lenght, OffSetAlongAxis.x),
                    Divide(Lenght, OffSetAlongAxis.y), Divide(Lenght, OffSetAlongAxis.z));
                    var floor = Mathf.Max(c.x, Mathf.Max(c.y, c.z));
                    _count = (int)floor;
                    // TODO: calculate this part properly
                }
                else if (offsetType == OffsetType.Radial)
                {
                    fitType = FitType.FixedCount;
                    _count = Count;
                }
            }

            return _count;
        }

        float Divide(float a, float b)
        {
            return b == 0 ? 0 : a / b;
        }
        Vector3 RandomPos()
        {
            RandomThreshold = new Vector3(Mathf.Abs(RandomThreshold.x), Mathf.Abs(RandomThreshold.y), Mathf.Abs(RandomThreshold.z));
            return !Randomize ? Vector3.zero : new Vector3(Random.Range(-RandomThreshold.x, RandomThreshold.x),
            Random.Range(-RandomThreshold.y, RandomThreshold.y), Random.Range(-RandomThreshold.z, RandomThreshold.z));
        }

        Vector3 NewScale(Transform t)
        {
            return !hasVisbleObject ? t.localScale : Vector3.one; //new Vector3(t.localScale.x / ArrayObject.transform.localScale.x,
            // t.localScale.y / ArrayObject.transform.localScale.y, t.localScale.z / ArrayObject.transform.localScale.z);
        }
    }
}