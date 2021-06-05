using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NLTechTest.Map
{
    public class SquareTile : MonoBehaviour, IGenerateableTile, IPaintableTile, IClickable
    {
        public Vector3 tileSize;
        private Renderer _rd;

        private Canvas canvas;
        public Text text;
        private bool displayCanvas;
        private Animator animator;
        private Collider cldr;
        [SerializeField]
        private Color _color;
        private bool hasLabel;

        void Start()
        {
            canvas = GetComponentInChildren<Canvas>();
            animator = GetComponent<Animator>();
            _rd = GetComponent<Renderer>();
            cldr = GetComponent<Collider>();
            hasLabel = true;

            if (transform.parent && transform.parent.childCount == 1)
            {
                hasLabel = false;
            }
            ApplyColor();

            canvas.enabled = false;
            cldr.enabled = false;
        }

        public void StartShake(float duration)
        {
            animator.SetBool("isShaking", true);
            StartCoroutine(StopShake(duration));
        }

        private IEnumerator StopShake(float duration)
        {
            yield return new WaitForSeconds(duration);
            animator.SetBool("isShaking", false);
        }

        MaterialPropertyBlock _probBloc;
        public GameObject GetTileModel()
        {
            return gameObject;
        }

        public Vector3 GetTileSize()
        {
            return tileSize;
        }

        public void InitializeTile()
        {
        }

        public SquareTile(Vector3 size)
        {
            tileSize = size;
        }
        public SquareTile()
        {
            tileSize = Vector3.zero;
        }

        public void SetTileColor(Color color)
        {
            _color = color;
            ApplyColor();
            if (_probBloc != null)
                UpdateNameByColor(_probBloc.GetColor("_Color"));
        }

        private void ApplyColor()
        {
            if (_rd == null)
            {
                _rd = GetComponent<Renderer>();
                if (_rd == null)
                    return;
            }

            _probBloc = new MaterialPropertyBlock();

            _rd.GetPropertyBlock(_probBloc);
            _probBloc.SetColor("_Color", _color);
            _rd.SetPropertyBlock(_probBloc);
        }

        void UpdateNameByColor(Color color)
        {
            text.text = "Tile R" + color.r + " G" + color.g + " B" + color.b;
            if (color.grayscale < 0.16f)
                text.color = Color.white;
            else
                text.color = Color.black;
        }

        public void OnClick()
        {
            StartShake(1f);
        }

        void OnBecameVisible()
        {
            if (hasLabel)
                canvas.enabled = true;
            cldr.enabled = true;
            MapAnalitics.IncreaseVisibleTileCount();
        }

        void OnBecameInvisible()
        {
            canvas.enabled = false;
            cldr.enabled = false;
            MapAnalitics.DecreaseVisibleTileCount();
        }
    }
}