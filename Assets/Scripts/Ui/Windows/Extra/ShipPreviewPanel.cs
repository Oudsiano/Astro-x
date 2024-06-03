using System;
using System.Collections;
using PlayerShips;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Ui.Windows.Extra
{
    public class ShipPreviewPanel : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _imageTransform;
        [SerializeField] private float _centerYPosition;
        [SerializeField] private float _enterYPosition;
        [SerializeField] private float _exitYPosition;
        [SerializeField] private float _moveTime;
        [SerializeField] private AnimationCurve _moveCurve;

        private Coroutine _moveRoutine;
        private Action _callback;

        public void SpawnShip(PlayerShipInfo ship)
        {
            _image.sprite = ship.Sprite;
        }

        public void Enter(Action callback = null)
        {
            if (_moveRoutine != null)
                StopCoroutine(_moveRoutine);

            _callback = callback;
            _moveRoutine = StartCoroutine(Move(_enterYPosition, _centerYPosition, false));
        }
        
        public void Exit(Action callback = null)
        {
            float from = _centerYPosition;
            if (_moveRoutine != null)
            {
                StopCoroutine(_moveRoutine);
                from = _imageTransform.anchoredPosition.y;
            }

            _callback = callback;
            _moveRoutine = StartCoroutine(Move(from, _exitYPosition, false));
        }

        private IEnumerator Move(float from, float to, bool inverseCurve)
        {
            float timer = 0f;
            while (timer < _moveTime)
            {
                timer += Time.deltaTime;
                float t = timer / _moveTime;
                if (inverseCurve) t = 1 - t;

                t = _moveCurve.Evaluate(t);

                _imageTransform.SetAnchoredYPosition(Mathf.Lerp(from, to, t));
                yield return null;
            }

            _imageTransform.SetAnchoredYPosition(to);
            _moveRoutine = null;
            _callback?.Invoke();
        }
    }
}