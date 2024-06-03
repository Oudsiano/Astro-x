using UnityEngine;
// using UnityEngine.EventSystems;

namespace Player.NewPlayer
{
    public class NewPlayerMovement : MonoBehaviour
    {
        [SerializeField] private MovingBorders _borders;
        
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
            ResizeBorders();
        }

        private void Update()
        {
            Vector3 targetPosition = transform.position;
#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = transform.position.z;
                targetPosition = mousePosition;
            }
#endif

#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount == 1)
            {
                Touch touch = Input.touches[0];
                if (touch.phase is TouchPhase.Canceled or TouchPhase.Ended)
                    return;
                
                Vector3 touchPosition = _mainCamera.ScreenToWorldPoint(touch.position);
                touchPosition.z = transform.position.z;
                targetPosition = touchPosition;
            }
#endif
            
            if (targetPosition.x < _borders.MinX || targetPosition.x > _borders.MaxX
                || targetPosition.y < _borders.MinY || targetPosition.y > _borders.MaxY)
                return;

// #if UNITY_EDITOR
//             if (EventSystem.current.IsPointerOverGameObject())
//                 return;
// #elif UNITY_IOS || UNITY_ANDROID
//             if (EventSystem.current.IsPointerOverGameObject(0))
//                 return;
// #endif
            
            targetPosition = Vector3.MoveTowards(transform.position, targetPosition, 30 * Time.deltaTime);
            targetPosition.z = 0f;

            transform.position = targetPosition;
        }

        private void ResizeBorders() 
        {
            _borders.MinX = _mainCamera.ViewportToWorldPoint(Vector2.zero).x + _borders.MinXOffset;
            _borders.MinY = _mainCamera.ViewportToWorldPoint(Vector2.zero).y + _borders.MinYOffset;
            _borders.MaxX = _mainCamera.ViewportToWorldPoint(Vector2.right).x - _borders.MaxXOffset;
            _borders.MaxY = _mainCamera.ViewportToWorldPoint(Vector2.up).y - _borders.MaxYOffset;
        }
    }
}