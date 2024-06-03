using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Windows.Extra
{
    public class ContentSizeUpdater : MonoBehaviour
    {
        [SerializeField] private RectTransform _selfTransform;
        [SerializeField] private RectTransform _target;
        [SerializeField] private ScrollRect _scrollRect;

        private float _lastHeight;

        private void Start()
        {
            UpdateHeight();
        }

        private void Update()
        {
            float targetHeight = _target.rect.height;
            if (Math.Abs(targetHeight - _lastHeight) > Mathf.Epsilon)
                UpdateHeight();
        }

        [ContextMenu("Update height")]
        private void UpdateHeight()
        {
            _lastHeight = _target.rect.height;
            _selfTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _lastHeight);
            _scrollRect.normalizedPosition = Vector2.one;
        }
    }
}