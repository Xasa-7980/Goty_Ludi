using System;
using UnityEngine;

public class PlayerResizer : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AnimationCurve scaleCurve;

    private Vector3 originalScale;
    private float timeElapsed;
    [SerializeField] private float cycleDuration = 1;
    [SerializeField] private float scaleMultiplier = 1.2f;
    private float addedScaleMultiplier;

    public Action OnPlayerAbsorbs;
    bool startResizing = false;
    private void Start ( )
    {
        anim = GetComponent<Animator>();
        originalScale = transform.localScale;
        addedScaleMultiplier = scaleMultiplier;
        OnPlayerAbsorbs = ( ) => {
            startResizing = true;
            anim.SetTrigger("Resize");
        };
    }
    private void Update ( )
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayerAbsorbs?.Invoke();
        }
        ResizeWithAnimationProgress();
    }
    void ResizeWithAnimationProgress ( )
    {
        if ( startResizing )
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / cycleDuration);
            float curveValue = scaleCurve.Evaluate(t);
            float scaleFactor = Mathf.Lerp(transform.localScale.normalized.magnitude, addedScaleMultiplier, curveValue);
            scaleFactor = Mathf.Clamp(scaleFactor, 0.6f, 2f);

            Vector3 scale = originalScale * scaleFactor;

            scale.x = Mathf.Clamp(scale.x, 0f, 1.6f);
            scale.y = Mathf.Clamp(scale.y, 0f, 1.6f);
            scale.z = Mathf.Clamp(scale.z, 0f, 1.6f);

            transform.localScale = scale;

            if (t >= 1)
            {
                originalScale = transform.localScale;
                timeElapsed = 0;
                startResizing = false;
            }
        }
    }
}
