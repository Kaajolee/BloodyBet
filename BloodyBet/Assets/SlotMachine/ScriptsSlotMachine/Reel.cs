using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Reel : MonoBehaviour {
    [Header("Symbol Object (child)")]
    [SerializeField] private GameObject symbolObject;  // child objektas su SpriteRenderer

    private SpriteRenderer sr;
    private Coroutine spinRoutine;
    private bool spinning = false;

    void Awake() {
        if (symbolObject == null) {
            Debug.LogError("Reel: symbolObject not assigned!");
            return;
        }

        sr = symbolObject.GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogError("Reel: symbolObject must have a SpriteRenderer!");
    }

    public void StartSpin() {
        if (spinning) return;
        spinRoutine = StartCoroutine(SpinAnimation());
    }

    public void StopSpin() {
        if (!spinning) return;
        spinning = false;
        if (spinRoutine != null)
            StopCoroutine(spinRoutine);

        // Assign final random symbol
        sr.sprite = SlotMachine.Instance.GetRandomSymbol();
    }

    IEnumerator SpinAnimation() {
        spinning = true;
        while (spinning) {
            sr.sprite = SlotMachine.Instance.GetRandomSymbol();
            yield return new WaitForSeconds(0.05f); // speed of sprite change
        }
    }

    public Sprite GetCurrentSprite() {
        return sr.sprite;
    }
}