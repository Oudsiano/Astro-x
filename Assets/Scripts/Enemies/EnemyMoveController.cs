using UnityEngine;

public class EnemyMoveController : MonoBehaviour
{
    public Collider[] Colliders { get; private set; }
    
    [Header("Parameters")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pointOffset;
    [SerializeField] private bool isStatic;

    [Header("Move point")]
    [SerializeField] private Vector3 movePoint;
    [SerializeField] private bool drawGizmos;

    [Header("Move zone")]
    [SerializeField] private Vector3 _moveZoneCenter;
    [SerializeField] private Vector3 _moveZoneSize;

    private float _resultSpeed;
    
    private void Awake()
    {
        Colliders = GetComponentsInChildren<Collider>();
    }

    public void Init(Vector3 moveZoneCenter, Vector3 moveZoneSize, float speedMultiplier)
    {
        if (speedMultiplier == 0f) speedMultiplier = 1f;
        _resultSpeed = moveSpeed * speedMultiplier;
        
        _moveZoneCenter = moveZoneCenter;
        _moveZoneSize = moveZoneSize;

        movePoint = EnemyExtentions.CalculateNewMovePoint(_moveZoneCenter, _moveZoneSize);
    }

    private void Update()
    {
        if (isStatic) return;

        if (Vector3.Distance(transform.position, movePoint) > pointOffset)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                movePoint,
                _resultSpeed * Time.deltaTime);
        }
        else
        {
            movePoint = EnemyExtentions.CalculateNewMovePoint(
                _moveZoneCenter, _moveZoneSize);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(movePoint, 0.05f);

        Gizmos.color = new Color(0.8f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(_moveZoneCenter, _moveZoneSize);
    }
#endif
}
