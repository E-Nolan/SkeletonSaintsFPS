using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FOR USE WITH STANDARD SHADER
public class FadeObjectsInWay : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _cam;
    [SerializeField] private float _fadedAlpha = 0.33f;
    [SerializeField] private FadeMode _fadeMode;

    [SerializeField] private float _checksPerSecond = 10f;
    [SerializeField] private int _fadeFps = 30;
    [SerializeField] private float _fadeSpeed = 1;

    [Header("Read Only Data")]
    [SerializeField] private List<FadeObject> _objectsBlockingView = new List<FadeObject>();
    private List<int> _indexesToClear = new List<int>();
    private Dictionary<FadeObject, Coroutine> _runningCoroutines = new Dictionary<FadeObject, Coroutine>();

    private RaycastHit[] _hits = new RaycastHit[10];

    private void Start()
    {
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        WaitForSeconds wait = new WaitForSeconds(1f / _checksPerSecond);

        while (true)
        {
            _target = GetComponent<Cinematic>().CurrentTarget.transform;
            int hits = Physics.RaycastNonAlloc(_cam.transform.position, (_target.transform.position - _cam.transform.position).normalized, _hits, Vector3.Distance(_cam.transform.position, _target.transform.position), _layerMask);
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    FadeObject fadeObject = GetFadeObjectFromHit(_hits[i]);
                    if (fadeObject != null && !_objectsBlockingView.Contains(fadeObject))
                    {
                        if (_runningCoroutines.ContainsKey(fadeObject))
                        {
                            if (_runningCoroutines[fadeObject] != null)
                            {
                                StopCoroutine(_runningCoroutines[fadeObject]);
                            }

                            _runningCoroutines.Remove(fadeObject);
                        }

                        _runningCoroutines.Add(fadeObject, StartCoroutine(FadeObjectOut(fadeObject)));
                        _objectsBlockingView.Add(fadeObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return wait;
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        for (int i = 0; i < _objectsBlockingView.Count; i++)
        {
            bool objectIsBeingHit = false;
            for (int j = 0; j < _hits.Length; j++)
            {
                FadeObject fadeObject = GetFadeObjectFromHit(_hits[j]);
                if (fadeObject != null && fadeObject == _objectsBlockingView[i])
                {
                    objectIsBeingHit = true;
                    break;
                }
            }

            if (!objectIsBeingHit)
            {
                if (_runningCoroutines.ContainsKey(_objectsBlockingView[i]))
                {
                    if (_runningCoroutines[_objectsBlockingView[i]] != null)
                    {
                        StopCoroutine(_runningCoroutines[_objectsBlockingView[i]]);
                    }
                    _runningCoroutines.Remove(_objectsBlockingView[i]);
                }

                _runningCoroutines.Add(_objectsBlockingView[i], StartCoroutine(FadeObjectIn(_objectsBlockingView[i])));
                _objectsBlockingView.RemoveAt(i);
            }
        }
    }

    private IEnumerator FadeObjectOut(FadeObject fadeObject)
    {
        float waitTime = 1f / _fadeFps;
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        int ticks = 1;

        for (int i = 0; i < fadeObject.Materials.Count; i++)
        {
            fadeObject.Materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            fadeObject.Materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha); 
            if (_fadeMode == FadeMode.Fade)
            {
                fadeObject.Materials[i].EnableKeyword("_ALPHABLEND_ON");
            }
            else
            {
                fadeObject.Materials[i].EnableKeyword("_ALPHAPREMULTIPLY_ON");
            }

            fadeObject.Materials[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        if (fadeObject.Materials[0].HasProperty("_Color"))
        {
            while (fadeObject.Materials[0].color.a > _fadedAlpha)
            {
                for (int i = 0; i < fadeObject.Materials.Count; i++)
                {
                    if (fadeObject.Materials[i].HasProperty("_Color"))
                    {
                        fadeObject.Materials[i].color = new Color(
                            fadeObject.Materials[i].color.r,
                            fadeObject.Materials[i].color.g,
                            fadeObject.Materials[i].color.b,
                            Mathf.Lerp(fadeObject.InitialAlpha, _fadedAlpha, waitTime * ticks * _fadeSpeed)
                        );
                    }
                }

                ticks++;
                yield return wait;
            }
        }

        if (_runningCoroutines.ContainsKey(fadeObject))
        {
            StopCoroutine(_runningCoroutines[fadeObject]);
            _runningCoroutines.Remove(fadeObject);
        }
    }

    private IEnumerator FadeObjectIn(FadeObject fadeObject)
    {
        float waitTime = 1f / _fadeFps;
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        int ticks = 1;

        if (fadeObject.Materials[0].HasProperty("_Color"))
        {
            while (fadeObject.Materials[0].color.a < fadeObject.InitialAlpha)
            {
                for (int i = 0; i < fadeObject.Materials.Count; i++)
                {
                    if (fadeObject.Materials[i].HasProperty("_Color"))
                    {
                        fadeObject.Materials[i].color = new Color(
                            fadeObject.Materials[i].color.r,
                            fadeObject.Materials[i].color.g,
                            fadeObject.Materials[i].color.b,
                            Mathf.Lerp(_fadedAlpha, fadeObject.InitialAlpha, waitTime * ticks * _fadeSpeed)
                        );
                    }
                }

                ticks++;
                yield return wait;
            }
        }

        for (int i = 0; i < fadeObject.Materials.Count; i++)
        {
            if (_fadeMode == FadeMode.Fade)
            {
                fadeObject.Materials[i].DisableKeyword("_ALPHABLEND_ON");
            }
            else
            {
                fadeObject.Materials[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            fadeObject.Materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            fadeObject.Materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            fadeObject.Materials[i].SetInt("_ZWrite", 1);
            fadeObject.Materials[i].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }

        if (_runningCoroutines.ContainsKey(fadeObject))
        {
            StopCoroutine(_runningCoroutines[fadeObject]);
            _runningCoroutines.Remove(fadeObject);
        }
    }

    private FadeObject GetFadeObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadeObject>() : null;
    }

    private void ClearHits()
    {
        RaycastHit hit = new RaycastHit();
        for (int i = 0; i < _hits.Length; i++)
        {
            _hits[i] = hit;
        }
    }

    public enum FadeMode
    {
        Transparent,
        Fade
    }
}
