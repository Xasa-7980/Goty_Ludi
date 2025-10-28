using System;
using UnityEngine;

public class PlayerResizer : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationCurve scaleCurve;

    private Vector3 originalScale;
    private float timeElapsed;
    [SerializeField] private float cycleDuration = 1f;
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float minScaleMultiplier = 0.8f;

    private float targetScaleMultiplier;
    private bool startResizing = false;
    private bool grow = true;

    public Action<bool> OnPlayerAbsorbs; // ahora recibe un bool para saber si crece o decrece

    private void Start ( )
    {
        anim = GetComponent<Animator>();
        originalScale = transform.localScale;

        // Inicializa el Action
        OnPlayerAbsorbs = ( shouldGrow ) =>
        {
            grow = shouldGrow;
            startResizing = true;
            timeElapsed = 0;
            targetScaleMultiplier = grow ? scaleMultiplier : minScaleMultiplier;
            anim.SetTrigger("Resize");
        };
    }

    private void Update ( )
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            OnPlayerAbsorbs?.Invoke( true );
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            
            OnPlayerAbsorbs?.Invoke( false );
        }
        ResizeWithAnimationProgress();
    }

    private void ResizeWithAnimationProgress ( )
    {
        if (!startResizing) return;

        timeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(timeElapsed / cycleDuration);
        float curveValue = scaleCurve.Evaluate(t);

        // Interpola entre el tamaño actual y el objetivo
        float scaleFactor = Mathf.Lerp(1f, targetScaleMultiplier, curveValue);
        scaleFactor = Mathf.Clamp(scaleFactor, 0.6f, 2f);

        Vector3 newScale = originalScale * scaleFactor;

        // Limita el tamaño máximo y mínimo
        newScale.x = Mathf.Clamp(newScale.x, 0f, 1.6f);
        newScale.y = Mathf.Clamp(newScale.y, 0f, 1.6f);
        newScale.z = Mathf.Clamp(newScale.z, 0f, 1.6f);

        transform.localScale = newScale;

        if (t >= 1f)
        {
            originalScale = transform.localScale;
            startResizing = false;
        }
    }
}